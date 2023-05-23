namespace OrderServer.Model.Response
{
    public class GetItemResponse
    {
        public int ItemId { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public float Price { get; set; }
    }
}
