using System.Security.Claims;
using EterationCaseStudy.Application.Features.Orders.Commands.CancelOrder;
using EterationCaseStudy.Application.Features.Orders.Commands.CreateOrder;
using EterationCaseStudy.Application.Features.Orders.Queries;
using EterationCaseStudy.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EterationCaseStudy.Api.Controllers.v1
{
    [ApiController]
    [Authorize]
    [Route("api/v1/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrdersController(IMediator mediator) { _mediator = mediator; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> Get()
        {
            var result = await _mediator.Send(new GetOrdersQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById([FromRoute] int id)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateOrderCommand body)
        {
            var id = await _mediator.Send(body);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel([FromRoute] int id)
        {
            var ok = await _mediator.Send(new CancelOrderCommand(id));
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
