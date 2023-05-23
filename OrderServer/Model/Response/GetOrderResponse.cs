using System.Text.Json.Serialization;

namespace OrderServer.Model.Response
{
    public class GetOrderResponse
    {
        public int OrderId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerEmail { get; set; }

        // Limitation: We are not checking the client's region, so we will just be returning the UTC time.
        // We are also not keeping track of when the order is updated or keeping past states of the order. 
        public DateTime OrderTimeUTC { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatus Status { get; set; }

        public ICollection<ResponseOrderItem> OrderItems { get; set; }
    }
}
