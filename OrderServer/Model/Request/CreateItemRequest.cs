namespace OrderServer.Model.Request
{
    public class CreateItemRequest
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public float PriceSEK { get; set; }
    }
}
