using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Presentation.Controllers;

namespace UnitTest.ProductApi.Controllers
{
    public class ProductControllerTest
    {
        private readonly IProduct productInterface;
        private readonly ProductsController productsController;
        public ProductControllerTest()
        {
            productInterface = A.Fake<IProduct>();
            productsController = new ProductsController(productInterface);
        }
        [Fact]
        public async Task GetProduct_WhenProductExists_ReturnOkResponseWithProducts()
        {
            var products = new List<Product>()
            {
                new(){Id = 1, Name = "Product 1", Price = 100.20m, Quantity = 15},
                new(){Id = 2, Name = "Product 2", Price = 1600.60m, Quantity = 12},
            };
            // fake response
            A.CallTo(() => productInterface.GetAllAsync()).Returns(products);


            //Act
            var result = await productsController.GetProducts();

            // assert
            var okResult = result.Result as OkObjectResult;

            okResult.Should().NotBeNull();

            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedProducts = okResult.Value as IEnumerable<ProductDTO>;
            returnedProducts.Should().NotBeNull();
            returnedProducts.Should().HaveCount(2);
            returnedProducts!.First().Id.Should().Be(1);
            returnedProducts!.Last().Id.Should().Be(2);
        }
        [Fact]
        public async Task GetProducts_WhenNoProductsExist_ReturnNotFoundResponse()
        {
            //arrange
            var products = new List<Product>();
            //fake set up
            A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

            // act 
            var result = await productsController.GetProducts();

            //assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            var message = notFoundResult.Value as string;
            message.Should().Be("No products detected.");

        }
    }
}
