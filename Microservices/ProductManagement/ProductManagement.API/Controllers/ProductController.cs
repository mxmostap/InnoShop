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
    
    // GET: api/Product/AllProducts
    [HttpGet("AllProducts")]
    public async Task<IActionResult> GetAllProducts()
    {
        return Ok(await _mediator.Send(new GetAllProductsQuery()));
    }
    
    // GET: api/Product/AllAvailableProducts
    [HttpGet("AllAvailableProducts")]
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

    // GET: api/Product/ProductsByUserId?id=
    [HttpGet("ProductsByUserId")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> GetProductsByUserId([FromQuery]int userId)
    {
        return Ok(await _mediator.Send(new GetProductsByUserIdQuery(userId)));
    }
    
    // POST: api/Product
    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CreateProduct(
        [FromForm] CreateProductCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
    
    // PATCH: api/Product/SoftDelete
    [HttpPatch("SoftDelete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SoftDelete([FromQuery] int userId)
    {
        await _mediator.Send(new SoftDeleteProductsCommand { UserId = userId });
        return Ok();
    }
    
    // PUT: api/Product/Update
    [HttpPut("Update")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> UpdateProduct(UpdateProductCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
    
    // DELETE: api/Product/Delete?id=
    [HttpDelete("Delete")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> DeleteProduct([FromQuery] int productId)
    {
        await _mediator.Send(new DeleteProductCommand { ProductId = productId });
        return Ok();
    }
    
    // DELETE: api/Product/DeleteByUserId?id=
    [HttpDelete("DeleteByUserId")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProductsByUserId([FromQuery] int userId)
    {
        await _mediator.Send(new DeleteProductsByUserIdCommand { UserId = userId });
        return Ok();
    }
}