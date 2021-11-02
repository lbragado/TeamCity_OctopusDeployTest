using Entities.Interfaces;
using Entities.POCOs;
using Repository.EFCore.DataContext;

namespace Repository.EFCore.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        readonly RepositoryContext Context;

        public AccountRepository(RepositoryContext context) => Context = context;

        public Account AddAccount(decimal cash)
        {
            Account account = new Account { Cash = cash };
            Context.Add(account);

            return account;
        }

        public Account UpdateAccount(int id, decimal cash)
        {
            Account account = GetById(id);

            if (account != null)
            {
                account.Cash = cash;
            }

            return account;
        }

        public Account GetById(int id)
        {
            Account account = Context.Accounts.Find(id);
            return account;
        }
    }
}
