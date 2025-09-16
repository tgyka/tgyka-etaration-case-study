using EterationCaseStudy.Application.Features.Auth.Commands.Login;
using EterationCaseStudy.Application.Features.Auth.Commands.RegisterUser;
using EterationCaseStudy.Application.Features.Auth.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EterationCaseStudy.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) { _mediator = mediator; }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterResultDto>> Register([FromBody] RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(Register), new { id = result.Id }, result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResultDto>> Login([FromBody] LoginCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch
            {
                return Unauthorized();
            }
        }
    }
}