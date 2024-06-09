using Moq;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.ViewModels;
using ShoppingCart.Models;
using ShoppingCart.Utility;
using ShoppingCart.Web.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace ShoppingCart.Tests
{
    public class OrderControllerTests
    {
        [Fact]
        public void OrderDetails_ReturnsOrderVM()
        {
            // Arrange
            var orderId = 1;
            var orderHeader = new OrderHeader { Id = orderId };
            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail { Id = 1, OrderHeaderId = orderId },
                new OrderDetail { Id = 2, OrderHeaderId = orderId }
            };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.OrderHeader.GetT(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>())).Returns(orderHeader);
            mockUnitOfWork.Setup(uow => uow.OrderDetail.GetAll(It.IsAny<string>())).Returns(orderDetails.AsQueryable());

            var controller = new OrderController(mockUnitOfWork.Object);

            // Act
            var result = controller.OrderDetails(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderHeader, result.OrderHeader);
            Assert.Equal(orderDetails, result.OrderDetails);
        }

        [Fact]
        public void SetToInProcess_UpdatesOrderStatusToInProcess()
        {
            // Arrange
            var orderId = 1;
            var orderHeader = new OrderHeader { Id = orderId, OrderStatus = OrderStatus.StatusPending };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.OrderHeader.GetT(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>())).Returns(orderHeader);

            var controller = new OrderController(mockUnitOfWork.Object);
            var orderVM = new OrderVM { OrderHeader = orderHeader };

            // Act
            controller.SetToInProcess(orderVM);

            // Assert
            Assert.Equal(OrderStatus.StatusPending, orderHeader.OrderStatus);
        }

    }
}