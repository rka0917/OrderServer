namespace OrderServer.Model.Request
{
    public class CreateOrderRequest
    {
        public string CustomerName { get; set; }

        public string CustomerEmail { get; set; }

        public OrderStatus Status { get; set; }

        public ICollection<RequestOrderItem> Items { get; set; }
    }
}
