using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagement.API.Controllers;
using ProductManagement.Application.Commands;
using ProductManagement.Application.Exceptions;
using ProductManagement.Application.Queries;
using ProductManagement.Microservice.Domain.Entities;

namespace ProductManagement.Tests.Unit;

public class ProductControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ProductController _productController;

    public ProductControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _productController = new ProductController(_mediatorMock.Object);
    }

    public class GetMethods : ProductControllerTests
    {
        [Fact]
        public async Task GetAllProducts_ReturnsProductList()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Product1",
                    Description = "Description1",
                    Price = 100.50m,
                    Availability = true,
                    CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                    UserId = 1,
                    IsDeleted = false
                },
                new Product
                {
                    Id = 2,
                    Name = "Product2",
                    Description = "Description2",
                    Price = 200.00m,
                    Availability = true,
                    CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                    UserId = 2,
                    IsDeleted = false
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            var result = await _productController.GetAllProducts();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, returnedProducts.Count());
            Assert.Equal("Product1", returnedProducts.First().Name);
            Assert.Equal(100.50m, returnedProducts.First().Price);
        }

        [Fact]
        public async Task GetAllProducts_EmptyList_ReturnsEmptyList()
        {
            var emptyProducts = new List<Product>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyProducts);

            var result = await _productController.GetAllProducts();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Empty(returnedProducts);
        }

        [Fact]
        public async Task GetAllProducts_DatabaseError_ThrowsException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Ошибка БД."));

            await Assert.ThrowsAsync<Exception>(() => _productController.GetAllProducts());
        }

        [Fact]
        public async Task GetAllAvailableProducts_ReturnsAvailableProductsList()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "AvailableProduct1",
                    Price = 100.50m,
                    Availability = true,
                    IsDeleted = false
                },
                new Product
                {
                    Id = 2,
                    Name = "AvailableProduct2",
                    Price = 200.00m,
                    Availability = true,
                    IsDeleted = false
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllAvailableProductsQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            var result = await _productController.GetAllAvailableProducts();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, returnedProducts.Count());
            Assert.All(returnedProducts, p => Assert.True(p.Availability));
            Assert.All(returnedProducts, p => Assert.False(p.IsDeleted));
        }

        [Fact]
        public async Task GetAllAvailableProducts_NoAvailableProducts_ReturnsEmptyList()
        {
            var emptyProducts = new List<Product>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllAvailableProductsQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyProducts);

            var result = await _productController.GetAllAvailableProducts();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Empty(returnedProducts);
        }

        [Fact]
        public async Task GetAllAvailableProducts_DatabaseError_ThrowsException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllAvailableProductsQuery>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Ошибка БД."));

            await Assert.ThrowsAsync<Exception>(() => _productController.GetAllAvailableProducts());
        }

        [Fact]
        public async Task GetProductById_ValidId_ReturnsProduct()
        {
            var productId = 1;
            var product = new Product
            {
                Id = productId,
                Name = "TestProduct",
                Price = 100.00m,
                Availability = true
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetProductByIdQuery>(q => q.Id == productId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var result = await _productController.GetProductById(productId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(productId, returnedProduct.Id);
            Assert.Equal("TestProduct", returnedProduct.Name);
        }

        [Fact]
        public async Task GetProductById_ProductNotFound_ReturnsNull()
        {
            var productId = 123;
            _mediatorMock.Setup(m => m.Send(It.Is<GetProductByIdQuery>(q => q.Id == productId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product)null);

            var result = await _productController.GetProductById(productId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Null(okResult.Value);
        }

        [Fact]
        public async Task GetProductById_DatabaseError_ThrowsException()
        {
            var productId = 1;
            _mediatorMock.Setup(m =>
                    m.Send(It.Is<GetProductByIdQuery>(q => q.Id == productId), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Ошибка БД."));

            await Assert.ThrowsAsync<Exception>(() => _productController.GetProductById(productId));
        }

        [Fact]
        public async Task GetProductsByUserId_ValidUserId_ReturnsProducts()
        {
            var userId = 1;
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product1", UserId = userId, Price = 100.00m },
                new Product { Id = 2, Name = "Product2", UserId = userId, Price = 200.00m }
            };

            _mediatorMock.Setup(m =>
                    m.Send(It.Is<GetProductsByUserIdQuery>(q => q.UserId == userId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            var result = await _productController.GetProductsByUserId(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, returnedProducts.Count());
            Assert.All(returnedProducts, p => Assert.Equal(userId, p.UserId));
        }

        [Fact]
        public async Task GetProductsByUserId_NoProducts_ReturnsEmptyList()
        {
            var userId = 123;
            var emptyProducts = new List<Product>();

            _mediatorMock.Setup(m =>
                    m.Send(It.Is<GetProductsByUserIdQuery>(q => q.UserId == userId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyProducts);

            var result = await _productController.GetProductsByUserId(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Empty(returnedProducts);
        }

        [Fact]
        public async Task GetProductsByUserId_DatabaseError_ThrowsException()
        {
            var userId = 1;
            _mediatorMock.Setup(m =>
                    m.Send(It.Is<GetProductsByUserIdQuery>(q => q.UserId == userId), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Ошибка БД."));

            await Assert.ThrowsAsync<Exception>(() => _productController.GetProductsByUserId(userId));
        }
    }

    public class PostMethods : ProductControllerTests
    {
        [Fact]
        public async Task CreateProduct_ValidCommand_ReturnsCreatedProduct()
        {
            var command = new CreateProductCommand
            {
                Name = "NewProduct",
                Description = "Description",
                Price = 100.00m,
                Availability = true
            };
            var createdProduct = new Product
            {
                Id = 1,
                Name = "NewProduct",
                Description = "Description",
                Price = 100.00m,
                UserId = 1
            };

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdProduct);

            var result = await _productController.CreateProduct(command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(1, returnedProduct.Id);
            Assert.Equal("NewProduct", returnedProduct.Name);
        }

        [Fact]
        public async Task CreateProduct_ProductExists_ThrowsDataExistsException()
        {
            var command = new CreateProductCommand
            {
                Name = "ExistingProduct",
                Description = "Description",
                Price = 100.00m
            };

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DataExistsException($"Продукт с названием {command.Name} уже существует."));

            var exception =
                await Assert.ThrowsAsync<DataExistsException>(() => _productController.CreateProduct(command));
            Assert.Contains("Продукт с названием", exception.Message);
        }

        [Fact]
        public async Task CreateProduct_UnauthorizedUser_ThrowsUnauthorizedException()
        {
            var command = new CreateProductCommand
            {
                Name = "NewProduct",
                Description = "Description",
                Price = 100.00m
            };

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedException("Пользователь не прошел аутентификацию."));

            var exception =
                await Assert.ThrowsAsync<UnauthorizedException>(() => _productController.CreateProduct(command));
            Assert.Equal("Пользователь не прошел аутентификацию.", exception.Message);
        }
    }

    public class PutPatchMethods : ProductControllerTests
    {
        [Fact]
        public async Task UpdateProduct_ValidCommand_ReturnsUpdatedProduct()
        {
            var command = new UpdateProductCommand
            {
                Id = 1,
                Name = "UpdatedProduct",
                Description = "UpdatedDescription",
                Price = 150.00m,
                Availability = false
            };
            var updatedProduct = new Product
            {
                Id = 1,
                Name = "UpdatedProduct",
                Description = "UpdatedDescription",
                Price = 150.00m,
                Availability = false
            };

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedProduct);

            var result = await _productController.UpdateProduct(command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(command.Name, returnedProduct.Name);
            Assert.Equal(command.Description, returnedProduct.Description);
            Assert.Equal(command.Price, returnedProduct.Price);
            Assert.Equal(command.Availability, returnedProduct.Availability);
        }

        [Fact]
        public async Task UpdateProduct_UnauthorizedUser_ThrowsUnauthorizedException()
        {
            var command = new UpdateProductCommand
            {
                Id = 1,
                Name = "UpdatedProduct"
            };

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedException("Пользователь не прошел аутентификацию."));

            var exception =
                await Assert.ThrowsAsync<UnauthorizedException>(() => _productController.UpdateProduct(command));
            Assert.Equal("Пользователь не прошел аутентификацию.", exception.Message);
        }

        [Fact]
        public async Task UpdateProduct_ProductNotFound_ThrowsNotFoundException()
        {
            var command = new UpdateProductCommand
            {
                Id = 123,
                Name = "NonexistentProduct"
            };

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException($"Продукт с ID \"{command.Id}\" не найден " +
                                                   $"или у вас нет права на его удаление."));

            var exception =
                await Assert.ThrowsAsync<NotFoundException>(() => _productController.UpdateProduct(command));
            Assert.Contains($"Продукт с ID \"{command.Id}\" не найден", exception.Message);
        }

        [Fact]
        public async Task SoftDelete_ValidUserId_ReturnsOk()
        {
            var userId = 1;
            _mediatorMock.Setup(m => m.Send(
                    It.Is<SoftDeleteProductsCommand>(c => c.Id == userId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(MediatR.Unit.Value);

            var result = await _productController.SoftDelete(userId);

            Assert.IsType<OkResult>(result);
            _mediatorMock.Verify(m => m.Send(
                It.Is<SoftDeleteProductsCommand>(c => c.Id == userId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SoftDelete_DatabaseError_ThrowsException()
        {
            var userId = 1;
            _mediatorMock.Setup(m => m.Send(
                    It.Is<SoftDeleteProductsCommand>(c => c.Id == userId),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Ошибка БД."));

            await Assert.ThrowsAsync<Exception>(() => _productController.SoftDelete(userId));
        }
    }

    public class DeleteMethods : ProductControllerTests
    {
        [Fact]
        public async Task DeleteProduct_ValidId_ReturnsOk()
        {
            var productId = 1;
            _mediatorMock.Setup(m => m.Send(
                    It.Is<DeleteProductCommand>(c => c.Id == productId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(MediatR.Unit.Value);

            var result = await _productController.DeleteProduct(productId);

            Assert.IsType<OkResult>(result);
            _mediatorMock.Verify(m => m.Send(
                It.Is<DeleteProductCommand>(c => c.Id == productId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_UnauthorizedUser_ThrowsUnauthorizedException()
        {
            var productId = 1;
            _mediatorMock.Setup(m => m.Send(
                    It.Is<DeleteProductCommand>(c => c.Id == productId),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedException("Пользователь не прошел аутентификацию."));

            var exception =
                await Assert.ThrowsAsync<UnauthorizedException>(() => _productController.DeleteProduct(productId));
            Assert.Equal("Пользователь не прошел аутентификацию.", exception.Message);
        }

        [Fact]
        public async Task DeleteProduct_ProductNotFound_ThrowsNotFoundException()
        {
            var productId = 123;
            _mediatorMock.Setup(m => m.Send(
                    It.Is<DeleteProductCommand>(c => c.Id == productId),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException(
                    $"Продукт с ID \"{productId}\" не найден или у вас нет права на его удаление."));

            var exception =
                await Assert.ThrowsAsync<NotFoundException>(() => _productController.DeleteProduct(productId));
            Assert.Contains($"Продукт с ID \"{productId}\" не найден", exception.Message);
        }

        [Fact]
        public async Task DeleteProductsByUserId_ValidUserId_ReturnsOk()
        {
            var userId = 1;
            _mediatorMock.Setup(m => m.Send(
                    It.Is<DeleteProductsByUserIdCommand>(c => c.Id == userId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(MediatR.Unit.Value);

            var result = await _productController.DeleteProductsByUserId(userId);

            Assert.IsType<OkResult>(result);
            _mediatorMock.Verify(m => m.Send(
                It.Is<DeleteProductsByUserIdCommand>(c => c.Id == userId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProductsByUserId_DatabaseError_ThrowsException()
        {
            var userId = 1;
            _mediatorMock.Setup(m => m.Send(
                    It.Is<DeleteProductsByUserIdCommand>(c => c.Id == userId),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Ошибка БД."));

            await Assert.ThrowsAsync<Exception>(() => _productController.DeleteProductsByUserId(userId));
        }
    }
}