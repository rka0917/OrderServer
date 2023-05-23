namespace OrderServer.Model
{
    public class Order
    {
        public int OrderId { get; set; }

        // Limitation: A customer should be its own enitity, with name, address, email and whatnot, but for the sake of 
        // simplicity in this implementation, we just add a name and email string.
        public required string CustomerName { get; set; }

        public required string CustomerEmail { get; set; }

        public DateTime OrderTimeUTC { get; set; }

        public OrderStatus Status { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
