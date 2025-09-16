using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using EterationCaseStudy.Application.Features.Users.Commands.SetUserRole;
using EterationCaseStudy.Application.Features.Users.Commands.UpdateUser;

namespace EterationCaseStudy.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator) { _mediator = mediator; }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserCommand body, CancellationToken ct)
        {
            if (id != body.Id) return BadRequest();
            var ok = await _mediator.Send(body, ct);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/setrole")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> SetRole([FromRoute] int id, [FromBody] SetUserRoleCommand body, CancellationToken ct)
        {
            if (id != body.Id) return BadRequest();
            var ok = await _mediator.Send(body, ct);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
