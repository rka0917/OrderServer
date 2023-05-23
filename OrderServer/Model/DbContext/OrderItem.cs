namespace OrderServer.Model
{
    public class OrderItem
    {
        public required int OrderId { get; set; }
        public required int ItemId { get; set; }
        public virtual required Item Item { get; set; }
        public required int Quantity { get; set; }

    }
}
