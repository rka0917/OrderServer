using OrderServer.Model;
using OrderServer.Model.Request;

namespace OrderServer.Service
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrders(); // Get all orders in the system GET
        Task<Order> GetOrderById(int id); // Get order specified by Id GET 

        Task<Order> CreateOrder(CreateOrderRequest newOrder); // Create a new order POST

        Task UpdateOrder(int id, UpdateOrderRequest updatedOrder); // Update resource through PUT
                                                       // request, return 204
        Task DeleteOrder(int id); // Delete an order. DELETE

        

    }
}
