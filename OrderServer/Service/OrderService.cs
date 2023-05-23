using Microsoft.EntityFrameworkCore;
using OrderServer.Model;
using OrderServer.Model.Request;

namespace OrderServer.Service
{
    public class OrderService : IOrderService
    {
        private readonly OrderContext _orderContext;

        public OrderService(OrderContext orderContext)
        {
            _orderContext = orderContext;
        }

        public async Task<Order> CreateOrder(CreateOrderRequest newOrder)
        {
            //Validate that all orders exist
            bool allIdsValid = newOrder.Items.All(i => _orderContext.Items.Any(entity => entity.ItemId == i.ItemId));
            if (!allIdsValid) 
            {
                throw new ArgumentException("One or more provided ItemIds does not exist in database");
            }

            Order order = new()
            {
                CustomerEmail = newOrder.CustomerEmail,
                OrderItems = new List<OrderItem>(),
                OrderTimeUTC = DateTime.UtcNow,
                Status = newOrder.Status,
                CustomerName = newOrder.CustomerName,
            };

            await _orderContext.Orders.AddAsync(order);
            await _orderContext.SaveChangesAsync();

            order.OrderItems = newOrder.Items.Select(orderItem => new OrderItem
            {
                ItemId = orderItem.ItemId,
                Item = _orderContext.Items.Find(orderItem.ItemId),
                OrderId = order.OrderId,
                Quantity = orderItem.Quantity
            }).ToList();

            await _orderContext.SaveChangesAsync();

            return order;
        }

        public async Task DeleteOrder(int id)
        {
            Order? order = await _orderContext.Orders.FindAsync(id);
            if (order == null)
            {
                throw new InvalidOperationException($"Order with Id {id} was not found");
            }
            else
            {
                _orderContext.Orders.Remove(order);
                await _orderContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _orderContext.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Item).ToListAsync();
        }

        public async Task<Order> GetOrderById(int id)
        {
            Order? order = await _orderContext.Orders.FindAsync(id);

            if (order == null)
            {
                throw new InvalidOperationException($"Order with id {id} was not found");
            }
            else
            {
                _orderContext.Entry(order)
                    .Collection(o => o.OrderItems)
                    .Query()
                    .Include(oi => oi.Item)
                    .Load();

                return order;
            }
        }

        public async Task UpdateOrder(int id, UpdateOrderRequest updatedOrder)
        {
            Order? orderToUpdate = await _orderContext.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Item).FirstOrDefaultAsync(o => o.OrderId == id);
            if (orderToUpdate == null)
            {
                throw new InvalidOperationException($"Order with Id {id} was not found");
            }
            else 
            {
                // Only update fields that have been populated in the request

                if (updatedOrder.Items != null && updatedOrder.Items.Any()) 
                {
                    //Validate that all orders exist
                    bool allIdsValid = updatedOrder.Items.All(i => _orderContext.Items.Any(entity => entity.ItemId == i.ItemId));
                    if (!allIdsValid)
                    {
                        throw new ArgumentException("One or more provided ItemIds does not exist in database");
                    }
                    _orderContext.RemoveRange(orderToUpdate.OrderItems);
                    ICollection<OrderItem> newOrderItems = updatedOrder.Items.Select(orderItem => new OrderItem
                    {
                        ItemId = orderItem.ItemId,
                        Item = _orderContext.Items.Find(orderItem.ItemId),
                        OrderId = id,
                        Quantity = orderItem.Quantity
                    }).ToList();

                    orderToUpdate.OrderItems = newOrderItems;
                }
                
                orderToUpdate.Status = updatedOrder.Status ?? orderToUpdate.Status;
                orderToUpdate.CustomerEmail = updatedOrder.CustomerEmail ?? orderToUpdate.CustomerEmail;
                orderToUpdate.CustomerName = updatedOrder.CustomerName ?? orderToUpdate.CustomerName;
                _orderContext.Update(orderToUpdate);

                await _orderContext.SaveChangesAsync();
            }
        }
    }
}
