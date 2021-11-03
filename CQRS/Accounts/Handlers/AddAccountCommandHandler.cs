using CQRS.Accounts.Commands;
using Entities.Interfaces;
using Entities.POCOs;
using Entities.Wrappers;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CQRS.Accounts.Handlers
{
    public class AddAccountCommandHandler
        : IRequestHandler<AddAccountCommand, AccountResponse>
    {
        readonly IAccountRepository AccountRepository;        
        readonly IUnitOfWork UnitOfWork;

        public AddAccountCommandHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork) => 
            (AccountRepository, UnitOfWork) = (accountRepository, unitOfWork);        
        public async Task<AccountResponse> Handle(AddAccountCommand request, CancellationToken cancellationToken)
        {            
            if (request.Cash <= 0)
                throw new System.ArgumentNullException("Cash is wrong");

            Account account = AccountRepository.AddAccount(request.Cash);

            await UnitOfWork.SaveChangesAsync();

            var accountResponse = new AccountResponse() { 
                Id = account.Id, 
                Cash = account.Cash, 
                Issuers= new List<IssuersResponse>() 
            };

            return accountResponse;
        }        
    }
}
