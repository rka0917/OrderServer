namespace OrderServer.Model
{
    public class Item
    {

        public int ItemId { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public required float Price { get; set; }

    }
}
