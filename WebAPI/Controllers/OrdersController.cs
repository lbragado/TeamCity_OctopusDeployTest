using CQRS.Orders.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        readonly IMediator Mediator;
        public OrdersController(IMediator mediator)
        {
            Mediator = mediator;
        }

        /// <summary>
        /// Ejecuta una Orden de Compra/Venta actualizando el Saldo de la cuenta, el Stock de la emisora y registrando la operación. 
        /// </summary>
        /// <param name="accountId">Id de la cuenta donde se tomará el saldo</param>
        /// <param name="command">Contrato de entrada</param>
        /// <returns>Response con saldo de la cuenta, datos de la emisora y mensajes de error (en caso de existir)</returns>
        [HttpPost("{accountId}/orders")]
        public async Task<IActionResult> BuySellOrder(int accountId, BuySellOrderCommand command)
        {
            command.AccountId = accountId;
            return Ok(await Mediator.Send(command));
        }
    }
}