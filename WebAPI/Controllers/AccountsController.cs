using CQRS.Accounts.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        readonly IMediator Mediator;                
        public AccountsController(IMediator mediator)
        {
            Mediator = mediator;
        }

        /// <summary>
        /// Crea una cuenta con el saldo indicado
        /// </summary>
        /// <param name="command">cash: saldo a agregar en la cuenta</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> AddAccount(AddAccountCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
                         
    }
}