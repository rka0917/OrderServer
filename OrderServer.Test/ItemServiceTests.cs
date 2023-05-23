using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OrderServer.Controllers;
using OrderServer.Model;
using OrderServer.Model.Request;
using OrderServer.Model.Response;
using OrderServer.Service;

namespace OrderServer.Test
{
    // Further improvement, make more extensive test cases. These ones relly only test the controller responses. 
    [TestClass]
    public class ItemServiceTests
    {

        private readonly ItemController _controller;
        private readonly Mock<IItemService> _itemService;

        private IEnumerable<Item> TestItems = new List<Item>()
        {
            new Item
            {
                Description = "description1",
                Name = "TestProduct",
                ItemId = 1,
                Price = 200
            },
            new Item
            {
                Description = "description2",
                Name = "TestProduct The Sequel",
                ItemId = 2,
                Price = 350
            },
        };

        public ItemServiceTests()
        {
            _itemService = new Mock<IItemService>();
            _controller = new ItemController(_itemService.Object);
        }

        [TestMethod]
        public async Task Get_Returns_All_Items()
        {
            _itemService.Setup(service => service.GetAllItems()).ReturnsAsync(TestItems);
            List<GetItemResponse> oracle = new List<GetItemResponse>()
            {
                new GetItemResponse
                {
                    ItemId = 1,
                    Price = 200,
                    Name = "TestProduct",
                    Description = "description1"
                },
                new GetItemResponse 
                {
                    Description = "description2",
                    Name = "TestProduct The Sequel",
                    ItemId = 2,
                    Price = 350
                }
            }.ToList();
            var result = await _controller.GetItems();
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result.Result;
            Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<GetItemResponse>));
            List<GetItemResponse> actual = ((IEnumerable<GetItemResponse>)okResult.Value).ToList();

            Assert.AreEqual(oracle.Count(), actual.Count());
            Assert.AreEqual(oracle.First().ItemId, actual.First().ItemId);
        }

        [TestMethod]
        public async Task Get_Item_By_Id() 
        {
            _itemService.Setup(service => service.GetItemById(1)).ReturnsAsync(TestItems.First());
            GetItemResponse oracle = new GetItemResponse()
            {
                ItemId = 1,
                Price = 200,
                Name = "TestProduct",
                Description = "description1"
            };

            var result = await _controller.GetItem(1);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result.Result;
            Assert.IsInstanceOfType(okResult.Value, typeof(GetItemResponse));
            GetItemResponse actual = (GetItemResponse)okResult.Value;

            Assert.AreEqual(oracle.ItemId, actual.ItemId);
            Assert.AreEqual(oracle.Description, actual.Description);
            Assert.AreEqual(oracle.Name, actual.Name);
        }

        [TestMethod]
        public async Task Get_Item_By_Wrong_Id()
        {
            _itemService.Setup(service => service.GetItemById(3)).Throws<InvalidOperationException>();
            var result = await _controller.GetItem(3);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Delete_Item_Returns_No_Content()
        {
            _itemService.Setup(service => service.DeleteItem(It.IsAny<int>()));

            var result = await _controller.DeleteItem(1);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_NonExisting_Item_Returns_Bad_Request()
        {
            _itemService.Setup(service => service.DeleteItem(It.IsAny<int>())).Throws<InvalidOperationException>();

            var result = await _controller.DeleteItem(3);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Create_Item_Returns_Created()
        {
            _itemService.Setup(service => service.CreateItem(It.IsAny<CreateItemRequest>())).ReturnsAsync(TestItems.First());

            var result = await _controller.CreateItem(new CreateItemRequest
            {
                Description = TestItems.First().Description,
                Name = TestItems.First().Name,
                PriceSEK = TestItems.First().Price
            });

            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
        }

        [TestMethod]
        public async Task Create_Item_With_Negative_Price_Returns_BadRequest()
        {
            _itemService.Setup(service => service.CreateItem(It.IsAny<CreateItemRequest>())).ReturnsAsync(TestItems.First());

            var result = await _controller.CreateItem(new CreateItemRequest
            {
                Description = TestItems.First().Description,
                Name = TestItems.First().Name,
                PriceSEK = -2.0f
            });

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }
    }
}
