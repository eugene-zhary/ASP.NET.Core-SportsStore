using Microsoft.AspNetCore.Mvc;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SportsStore.Tests
{
    public class ProductControllerTests
    {

        [Fact]
        public void CanUseRepository()
        {
            // Arrange
            Mock<IStoreRepository> mock = new();

            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product{ProductID = 1, Name = "P1"},
                new Product{ProductID = 2, Name = "P2"}
            }).AsQueryable());

            HomeController controller = new(mock.Object);

            // Act
            ProductListViewModel result = controller.Index(null).ViewData.Model as ProductListViewModel;

            // Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("P1", prodArray[0].Name);
            Assert.Equal("P2", prodArray[1].Name);
        }

        [Fact]
        public void CanPaginate()
        {
            // Arrange
            Mock<IStoreRepository> mock = new();

            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product{ProductID =1, Name="P1"},
                new Product{ProductID =2, Name="P2"},
                new Product{ProductID =3, Name="P3"},
                new Product{ProductID =4, Name="P4"},
                new Product{ProductID =5, Name="P5"}
            }).AsQueryable());

            HomeController controller = new(mock.Object) {
                PageSize = 3
            };

            // Act
            ProductListViewModel result = controller.Index(null, 2).ViewData.Model as ProductListViewModel;

            // Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("P4", prodArray[0].Name);
            Assert.Equal("P5", prodArray[1].Name);
        }

        [Fact]
        public void CanSendPaginationViewModel()
        {
            // Arrange
            Mock<IStoreRepository> mock = new();
            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product{ProductID =1, Name="P1"},
                new Product{ProductID =2, Name="P2"},
                new Product{ProductID =3, Name="P3"},
                new Product{ProductID =4, Name="P4"},
                new Product{ProductID =5, Name="P5"}
            }).AsQueryable());

            HomeController controller = new(mock.Object) {
                PageSize = 3
            };

            // Act
            ProductListViewModel result = controller.Index(null, 2).ViewData.Model as ProductListViewModel;

            // Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.Equal(2, pageInfo.CurrentPage);
            Assert.Equal(3, pageInfo.ItemsPerPage);
            Assert.Equal(5, pageInfo.TotalItems);
            Assert.Equal(2, pageInfo.TotalPages);
        }

        [Fact]
        public void CanFilterProducts()
        {
            // Arrange
            // - create the mock repository
            Mock<IStoreRepository> mock = new();
            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
            }).AsQueryable());

            // Arrange - create a controller and make the page size 3 items
            HomeController controller = new(mock.Object);
            controller.PageSize = 3;

            // Action
            Product[] result = (controller.Index("Cat2", 1).ViewData.Model as ProductListViewModel)
                                    .Products.ToArray();

            // Assert
            Assert.Equal(2, result.Length);
            Assert.True(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.True(result[1].Name == "P4" && result[0].Category == "Cat2");
        }

        [Fact]
        public void GenerateCategorySpecificProductCount()
        {
            // Arrange
            Mock<IStoreRepository> mock = new();
            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
            }).AsQueryable());

            HomeController target = new(mock.Object);
            target.PageSize = 3;

            Func<ViewResult, ProductListViewModel> GetModel = reuslt =>
                reuslt?.ViewData?.Model as ProductListViewModel;

            // Act
            int? res1 = GetModel(target.Index("Cat1"))?.PagingInfo.TotalItems;
            int? res2 = GetModel(target.Index("Cat2"))?.PagingInfo.TotalItems;
            int? res3 = GetModel(target.Index("Cat3"))?.PagingInfo.TotalItems;
            int? resAll = GetModel(target.Index(null))?.PagingInfo.TotalItems;

            // Assert
            Assert.Equal(2, res1);
            Assert.Equal(2, res2);
            Assert.Equal(1, res3);
            Assert.Equal(5, resAll);
        }
    }
}
