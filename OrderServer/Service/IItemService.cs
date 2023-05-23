using OrderServer.Model;
using OrderServer.Model.Request;

namespace OrderServer.Service
{
    public interface IItemService
    {

        Task<IEnumerable<Item>> GetAllItems();

        Task<Item> GetItemById(int id);

        Task<Item> CreateItem(CreateItemRequest createdItem);

        Task DeleteItem(int id);

    }
}
