using Entities.Wrappers;
using MediatR;

namespace CQRS.Accounts.Commands
{
    public class AddAccountCommand : IRequest<AccountResponse>
    {
        public decimal Cash { get; set; }
    }
}
