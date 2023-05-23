using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OrderServer.Model;
using OrderServer.Model.Request;
using OrderServer.Model.Response;
using OrderServer.Service;

namespace OrderServer.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {

        private readonly IItemService _itemService;
        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetItemResponse>>> GetItems()
        {
            try 
            {
                IEnumerable<Item> items = await _itemService.GetAllItems();


                return Ok(items.Select(item => new GetItemResponse
                {
                    Name = item.Name,
                    Price = item.Price,
                    Description = item.Description,
                    ItemId = item.ItemId
                }));
            } catch (Exception ex) 
            {
                return Problem(ex.Message);
            }
            
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetItemResponse>> GetItem(int id)
        {
            try
            {
                var item = await _itemService.GetItemById(id);
            
                if (item == null)
                {
                    return NotFound();
                }

            
                var response = new GetItemResponse
                {
                    Name = item.Name,
                    Price = item.Price,
                    Description = item.Description,
                    ItemId = item.ItemId
                };

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            
        }

        [HttpPost]
        public async Task<ActionResult<GetItemResponse>> CreateItem(CreateItemRequest item)
        {
            if (item == null) 
            {
                return BadRequest("'Item' is null");
            }

            if (item.Name.IsNullOrEmpty()) 
            {
                return BadRequest("'Item.Name' cannot be empty string or null");
            }

            if (item.PriceSEK < 0.0) 
            {
                return BadRequest("'Item.Price' cannot be lower than 0");
            }

            try
            {
                Item addedItem = await _itemService.CreateItem(item);
                GetItemResponse response = new GetItemResponse
                {
                    Name = addedItem.Name,
                    Price = addedItem.Price,
                    Description = addedItem.Description,
                    ItemId = addedItem.ItemId
                };

                return CreatedAtAction(nameof(GetItem), new { id = response.ItemId }, response);
            } catch (DuplicateNameException) 
            {
                return BadRequest("Provided item name already exists");
            } catch (Exception ex) 
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                await _itemService.DeleteItem(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
