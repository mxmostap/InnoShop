using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.Commands;
using ProductManagement.Application.Queries;

namespace ProductManagement.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    // GET: api/Product/all-products
    [HttpGet("all-products")]
    public async Task<IActionResult> GetAllProducts()
    {
        return Ok(await _mediator.Send(new GetAllProductsQuery()));
    }
    
    // GET: api/Product/all-available-products
    [HttpGet("all-available-products")]
    public async Task<IActionResult> GetAllAvailableProducts()
    {
        return Ok(await _mediator.Send(new GetAllAvailableProductsQuery()));
    }
    
    // GET: api/Product?id=
    [HttpGet]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> GetProductById([FromQuery]int productId)
    {
        return Ok(await _mediator.Send(new GetProductByIdQuery(productId)));
    }

    // GET: api/Product/products-by-user-id?id=
    [HttpGet("products-by-user-id")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> GetProductsByUserId([FromQuery]int userId)
    {
        return Ok(await _mediator.Send(new GetProductsByUserIdQuery(userId)));
    }
    
    // POST: api/Product/create-product
    [HttpPost("create-product")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CreateProduct(
        [FromForm] CreateProductCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
    
    // PATCH: api/Product/soft-delete
    [HttpPatch("soft-delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SoftDelete([FromQuery] int userId)
    {
        await _mediator.Send(new SoftDeleteProductsCommand { Id = userId });
        return Ok();
    }
    
    // PUT: api/Product/update
    [HttpPut("update")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> UpdateProduct(UpdateProductCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
    
    // DELETE: api/Product/delete?id=
    [HttpDelete("delete")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> DeleteProduct([FromQuery] int productId)
    {
        await _mediator.Send(new DeleteProductCommand { Id = productId });
        return Ok();
    }
    
    // DELETE: api/Product/delete-by-user-id?id=
    [HttpDelete("delete-by-user-id")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProductsByUserId([FromQuery] int userId)
    {
        await _mediator.Send(new DeleteProductsByUserIdCommand { Id = userId });
        return Ok();
    }
}