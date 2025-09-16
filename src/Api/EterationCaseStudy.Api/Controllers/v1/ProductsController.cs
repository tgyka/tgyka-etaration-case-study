using EterationCaseStudy.Application.Features.Products.Commands.CreateProduct;
using EterationCaseStudy.Application.Features.Products.Commands.DeleteProduct;
using EterationCaseStudy.Application.Features.Products.Commands.UpdateProduct;
using EterationCaseStudy.Application.Features.Products.Queries;
using EterationCaseStudy.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EterationCaseStudy.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator) { _mediator = mediator; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Product>>> Get([FromQuery] GetProductsQuery query)
        {
            var result = await _mediator.Send(query);
            Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
            return Ok(result.Items);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById([FromRoute] int id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<int>> Create([FromBody] CreateProductCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductCommand body)
        {
            if (id != body.Id) return BadRequest();
            var ok = await _mediator.Send(body);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var ok = await _mediator.Send(new DeleteProductCommand(id));
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
