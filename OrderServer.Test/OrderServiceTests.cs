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
    public class OrderServiceTests
    {

        private readonly OrderController _controller; 
        private readonly Mock<IOrderService> _orderService;

        private IEnumerable<Order> TestOrders = new List<Order>()
        {
            new Order
            {
                OrderId = 1,
                CustomerEmail = "Test@test.com",
                CustomerName = "Suleiman",
                OrderTimeUTC = DateTime.MinValue,
                Status = OrderStatus.Registered,
                OrderItems = new List<OrderItem>()
                {
                    new OrderItem()
                    {
                        OrderId = 1,
                        ItemId = 1,
                        Quantity = 2,
                        Item = new Item
                        {
                            Description = "description1",
                            Name = "TestProduct",
                            ItemId = 1,
                            Price = 200
                        }
                    }
                }
            },

            new Order 
            {
                OrderId = 2,
                CustomerEmail = "Test2@test.com",
                CustomerName = "Sulenman",
                OrderTimeUTC = DateTime.MinValue,
                Status = OrderStatus.Registered,
                OrderItems = new List<OrderItem>()
                {
                    new OrderItem()
                    {
                        OrderId = 1,
                        ItemId = 1,
                        Quantity = 2,
                        Item = new Item
                        {
                            Description = "description1",
                            Name = "TestProduct",
                            ItemId = 1,
                            Price = 200
                        }
                    },

                    new OrderItem() {
                        OrderId = 1,
                        ItemId = 2,
                        Quantity = 3,
                        Item = new Item
                        {
                            Description = "description2",
                            Name = "TestProduct The Sequel",
                            ItemId = 2,
                            Price = 350
                        }
                    }
                }
            }
        };

        public OrderServiceTests() 
        {
            _orderService = new Mock<IOrderService>();
            _controller = new OrderController(_orderService.Object);
        }

        [TestMethod]
        public async Task Get_Returns_All_Orders_Returns_OK()
        {
            _orderService.Setup(service => service.GetAllOrders()).ReturnsAsync(TestOrders);
            List<GetOrderResponse> oracle = new List<GetOrderResponse>()
            {
                new GetOrderResponse
                {
                    OrderId = 1,
                    CustomerEmail = "Test@test.com",
                    CustomerName = "Suleiman",
                    OrderTimeUTC = DateTime.MinValue,
                    OrderItems = new List<ResponseOrderItem>()
                    {
                        new ResponseOrderItem()
                        {
                            Quantity = 2,
                            Item = new GetItemResponse
                            {
                                Description = "description1",
                                Name = "TestProduct",
                                ItemId = 1,
                                Price = 200
                            }
                        }
                    }
                },
                new GetOrderResponse
                {
                    OrderId = 2,
                    CustomerEmail = "Test2@test.com",
                    CustomerName = "Sulenman",
                    OrderTimeUTC = DateTime.MinValue,
                    OrderItems = new List<ResponseOrderItem>()
                    {
                        new ResponseOrderItem()
                        {
                            Quantity = 2,
                            Item = new GetItemResponse
                            {
                                Description = "description1",
                                Name = "TestProduct",
                                ItemId = 1,
                                Price = 200
                            }
                        },

                        new ResponseOrderItem() {
                            Quantity = 3,
                            Item = new GetItemResponse
                            {
                                Description = "description2",
                                Name = "TestProduct The Sequel",
                                ItemId = 2,
                                Price = 350
                            }
                        }
                    }
                }
            }.ToList();

            var result = await _controller.GetOrders();
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result.Result;
            Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<GetOrderResponse>));

            List<GetOrderResponse> actual = ((IEnumerable<GetOrderResponse>)okResult.Value).ToList();

            Assert.AreEqual(oracle.Count(), actual.Count());
            Assert.AreEqual(oracle.First().OrderId, actual.First().OrderId);
            Assert.AreEqual(oracle.First().OrderItems.Count(), actual.First().OrderItems.Count());
        }

        [TestMethod]
        public async Task Get_Order_By_Id_Return_OK() 
        {
            _orderService.Setup(service => service.GetOrderById(1)).ReturnsAsync(TestOrders.First());

            var result = await _controller.GetOrder(1);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Get_Order_By_Wrong_Id_Returns_Bad_Request() 
        {
            _orderService.Setup(service => service.GetOrderById(3)).Throws<InvalidOperationException>();

            var result = await _controller.GetOrder(3);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Delete_Order_Returns_No_Content() 
        {
            _orderService.Setup(service => service.DeleteOrder(It.IsAny<int>()));

            var result = await _controller.DeleteOrder(1);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_NonExisting_Order_Returns_Bad_Request()
        {
            _orderService.Setup(service => service.DeleteOrder(It.IsAny<int>())).Throws<InvalidOperationException>();

            var result = await _controller.DeleteOrder(3);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Create_Order_Returns_Created()
        {
            _orderService.Setup(service => service.CreateOrder(It.IsAny<CreateOrderRequest>())).ReturnsAsync(TestOrders.First());

            var result = await _controller.CreateOrder(new CreateOrderRequest
            {
                CustomerEmail = TestOrders.First().CustomerEmail,
                CustomerName = TestOrders.First().CustomerName,
                Status = TestOrders.First().Status,
                Items = TestOrders.First().OrderItems.Select(x => new RequestOrderItem
                {
                    ItemId = x.ItemId,
                    Quantity = x.Quantity,
                }).ToList()

            });

            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        }

        [TestMethod]
        public async Task Create_Order_Without_Items_Returns_BadRequest()
        {
            _orderService.Setup(service => service.CreateOrder(It.IsAny<CreateOrderRequest>())).ReturnsAsync(TestOrders.First());

            var result = await _controller.CreateOrder(new CreateOrderRequest
            {
                CustomerEmail = TestOrders.First().CustomerEmail,
                CustomerName = TestOrders.First().CustomerName,
                Status = TestOrders.First().Status,

            });

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Update_Order_Returns_No_Content()
        {
            _orderService.Setup(service => service.UpdateOrder(It.IsAny<int>(),It.IsAny<UpdateOrderRequest>()));

            var result = await _controller.UpdateOrder(1, new UpdateOrderRequest
            {
                CustomerEmail = "Chungus"
            });

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Empty_Update_Order_Returns_BadRequest()
        {
            _orderService.Setup(service => service.UpdateOrder(It.IsAny<int>(), It.IsAny<UpdateOrderRequest>()));

            var result = await _controller.UpdateOrder(1, new UpdateOrderRequest
            {
            });

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
    }
}