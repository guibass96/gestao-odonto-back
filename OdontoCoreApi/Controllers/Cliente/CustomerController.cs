using Gbarber.Application.Commands.Customer.AddCustomer;
using GBarber.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GBarber.WebApi.Controllers.Clientes
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("")]
        public async Task<IActionResult> CadastroCliente([FromBody] CustomerCommand customer)
        {
            try
            {
                await _mediator.Send(customer);
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }

        }
    }
}
