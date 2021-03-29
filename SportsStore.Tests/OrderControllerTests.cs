using Microsoft.AspNetCore.Mvc;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SportsStore.Tests
{
    public class OrderControllerTests
    {
        [Fact]
        public void Cannot_Checkout_Empty_Cart()
        {
            // Arrange
            // - create a mock repository
            Mock<IOrderRepository> mock = new();
            // - create an empty cart
            Cart cart = new();
            // - create the order
            Order oreder = new();
            // - create an instatnce of the controller
            OrderController targer = new(mock.Object, cart);

            // Act
            ViewResult result = targer.Checkout(oreder) as ViewResult;

            // Assert
            // - check that the order hasn't been stored
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            // - check that the method is returning the default view
            Assert.True(string.IsNullOrEmpty(result.ViewName));
            // - check that I am passing an invalid model to the view
            Assert.False(result.ViewData.ModelState.IsValid);
        }

        [Fact]
        public void Cannot_Checkout_Invalid_ShoppingDetails()
        {
            // Arrange
            // - create a mock repository
            Mock<IOrderRepository> mock = new();
            // - create a cart with one item
            Cart cart = new();
            cart.AddItem(new Product(), 1);
            // - create an instatnce of the controller
            OrderController targer = new(mock.Object, cart);
            // - add an error to the model
            targer.ModelState.AddModelError("error", "error");

            // Act - try to checkout
            ViewResult result = targer.Checkout(new Order()) as ViewResult;

            // Assert
            // - check that the order hasn't been stored
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            // - check that the method is returning the default view
            Assert.True(string.IsNullOrEmpty(result.ViewName));
            // - check that I am passing an invalid model to the view
            Assert.False(result.ViewData.ModelState.IsValid);
        }

        [Fact]
        public void Can_Checkout_And_Submit_Order()
        {
            // Arrange
            // - crate a mock order repository
            Mock<IOrderRepository> mock = new();
            // - create a cart with one item
            Cart cart = new();
            cart.AddItem(new Product(), 1);
            // - create an instance of the controller
            OrderController targe = new(mock.Object, cart);

            // Act - try to checkout
            RedirectToPageResult result = targe.Checkout(new Order()) as RedirectToPageResult;

            // Assert
            // - check that the order has been stored 
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Once);
            // - check that the method is redirectiong to the Completed action
            Assert.Equal("/Completed", result.PageName);
        }
    }
}
