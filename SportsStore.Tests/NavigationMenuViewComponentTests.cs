using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using SportsStore.Components;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SportsStore.Tests
{
    public class NavigationMenuViewComponentTests
    {
        [Fact]
        public void CanSelectCategories()
        {
            // Arrange
            Mock<IStoreRepository> mock = new();
            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                new Product {ProductID = 2, Name = "P2", Category = "Apples"},
                new Product {ProductID = 3, Name = "P3", Category = "Plums"},
                new Product {ProductID = 4, Name = "P4", Category = "Oranges"},
            }).AsQueryable());

            NavigationMenuViewComponent target = new(mock.Object);

            // Act - get the set of categories
            string[] results = ((IEnumerable<string>)(target.Invoke() as ViewViewComponentResult)
                                    .ViewData.Model).ToArray();

            // Assert
            Assert.True(Enumerable.SequenceEqual(new string[] { "Apples", "Oranges", "Plums" }, results));
        }

        [Fact]
        public void IndicatesSelectedCategory()
        {
            // Arrange
            string categoryToSelect = "Apples";
            Mock<IStoreRepository> mock = new();
            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product{ProductID = 1, Name = "P1", Category = "Apples" },
                new Product{ProductID = 4, Name = "P2", Category = "Oranges" },
            }).AsQueryable());

            NavigationMenuViewComponent target = new(mock.Object);
            target.ViewComponentContext = new() {
                ViewContext = new() {
                    RouteData = new()
                }
            };

            target.RouteData.Values["category"] = categoryToSelect;

            // Act
            string result = (target.Invoke() as ViewViewComponentResult)
                                .ViewData["SelectedCategory"].ToString();

            // Assert
            Assert.Equal(categoryToSelect, result);
        }
    }
}
