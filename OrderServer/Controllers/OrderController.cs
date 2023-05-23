using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OrderServer.Model;
using OrderServer.Model.Request;
using OrderServer.Model.Response;
using OrderServer.Service;

namespace OrderServer.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrderController : ControllerBase
    {

        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService) 
        { 
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetOrderResponse>>> GetOrders() 
        {
            try 
            {
                IEnumerable<Order> orders = await _orderService.GetAllOrders();
                return Ok(orders.Select(o => new GetOrderResponse 
                {
                    OrderId = o.OrderId,
                    OrderItems = o.OrderItems.Select(o => new ResponseOrderItem
                    {
                        Quantity = o.Quantity,
                        Item = new GetItemResponse
                        {
                            Description = o.Item.Description,
                            Name = o.Item.Name,
                            Price = o.Item.Price,
                            ItemId = o.ItemId

                        }
                    }).ToList(),
                    OrderTimeUTC = o.OrderTimeUTC,
                    CustomerEmail = o.CustomerEmail,
                    CustomerName = o.CustomerName,
                    Status = o.Status
                }));
            } 
            catch (Exception ex) 
            {
                return Problem(ex.Message);
            }
            
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetOrderResponse>> GetOrder(int id) 
        {
            try
            {
                var order = await _orderService.GetOrderById(id);

                if (order == null)
                {
                    return NotFound();
                }

                ICollection<ResponseOrderItem> orderItems = order.OrderItems.Select(o => new ResponseOrderItem
                {
                    Quantity = o.Quantity,
                    Item = new GetItemResponse
                    {
                        Description = o.Item.Description,
                        Name = o.Item.Name,
                        Price = o.Item.Price,
                        ItemId = o.ItemId

                    }
                }).ToList();

            
                var response = new GetOrderResponse
                {
                    OrderId = order.OrderId,
                    OrderItems = orderItems,
                    OrderTimeUTC = order.OrderTimeUTC,
                    CustomerEmail = order.CustomerEmail,
                    CustomerName = order.CustomerName,
                    Status = order.Status
                };

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id) 
        {
            try
            {
                await _orderService.DeleteOrder(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest order) 
        {
            if (order == null) 
            {
                return BadRequest("Provided order cannot be null");
            }

            if (order.Items == null || !order.Items.Any()) 
            {
                return BadRequest("Please provide some items included in the order");
            }

            if (order.CustomerEmail == null) 
            {
                return BadRequest("Please provide the Customer's email");
            }

            if (!Enum.IsDefined(typeof(OrderStatus), order.Status)) 
            {
                return BadRequest("Order status not recognized");
            }

            if(order.Items.ToList().Exists(oi => oi.Quantity <= 0))
            {
                return BadRequest("Cannot add <= 0 quatities of items");
            }

            try
            {
                Order addedOrder = await _orderService.CreateOrder(order);
                ICollection<ResponseOrderItem> orderItems = addedOrder.OrderItems.Select(o => new ResponseOrderItem
                {
                    Quantity = o.Quantity,
                    Item = new GetItemResponse
                    {
                        Description = o.Item.Description,
                        Name = o.Item.Name,
                        Price = o.Item.Price,
                        ItemId = o.ItemId

                    }
                }).ToList();
                GetOrderResponse response = new()
                {
                    CustomerEmail = addedOrder.CustomerEmail,
                    CustomerName = addedOrder.CustomerName,
                    OrderTimeUTC = addedOrder.OrderTimeUTC,
                    OrderId = addedOrder.OrderId,
                    OrderItems = orderItems,
                    Status = addedOrder.Status
                };

                return CreatedAtAction(nameof(GetOrder), new { id = response.OrderId }, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex) 
            {
                return Problem(ex.Message);
            }
        }

        // If any field is left out (== null), then we just dont bother to update that field.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, UpdateOrderRequest order) 
        {
            if (order == null || (order.Status == null && order.Items == null && order.CustomerEmail.IsNullOrEmpty() && order.CustomerName.IsNullOrEmpty()))
            {
                return BadRequest("Provided update of order cannot be null");
            }

            if (order.Status != null && !Enum.IsDefined(typeof(OrderStatus), order.Status))
            {
                return BadRequest("Order status not recognized");
            }

            if (order.Items != null && order.Items.ToList().Exists(oi => oi.Quantity <= 0))
            {
                return BadRequest("Cannot add <= 0 quatities of items");
            }

            try
            {
                await _orderService.UpdateOrder(id, order);
                return NoContent();
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
               
        }
    }
}
