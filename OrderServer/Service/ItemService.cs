using System.Data;
using Microsoft.EntityFrameworkCore;
using OrderServer.Model;
using OrderServer.Model.Request;

namespace OrderServer.Service
{
    public class ItemService : IItemService
    {
        private readonly OrderContext _orderContext;

        public ItemService(OrderContext orderContext) 
        {
            _orderContext = orderContext;
        }

        public async Task<Item> CreateItem(CreateItemRequest createdItem)
        {
            Item item = new()
            {
                Description = createdItem.Description,
                Name = createdItem.Name,
                Price = createdItem.PriceSEK
            };

            Item? duplicateItem = await _orderContext.Items.FirstOrDefaultAsync(i => i.Name == item.Name);
            if (duplicateItem != null)
            {
                throw new DuplicateNameException("Provided item name already exists in the database!");
            }

            await _orderContext.Items.AddAsync(item);
            await _orderContext.SaveChangesAsync();
  

            return item;
        }

        public async Task DeleteItem(int id)
        {
            Item? item = await _orderContext.Items.FindAsync(id);
            if (item == null)
            {
                throw new InvalidOperationException($"Item with Id {id} was not found");
            }
            else 
            {
                _orderContext.Items.Remove(item);
                await _orderContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Item>> GetAllItems()
        {
            return await _orderContext.Items.ToListAsync();
        }

        public async Task<Item> GetItemById(int id)
        {
            Item? item = await _orderContext.Items.FindAsync(id);

            if (item == null)
            {
                throw new InvalidOperationException($"Item with id {id} was not found");
            }
            else 
            {
                return item;
            }
        }
    }
}
