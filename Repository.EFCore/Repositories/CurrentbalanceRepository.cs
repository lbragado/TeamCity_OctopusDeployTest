using Entities.Interfaces;
using Entities.POCOs;
using Repository.EFCore.DataContext;
using System.Linq;

namespace Repository.EFCore.Repositories
{
    public class CurrentbalanceRepository : ICurrentbalanceRepository
    {
        readonly RepositoryContext Context;

        public CurrentbalanceRepository(RepositoryContext context) => Context = context;

        public CurrentBalance AddCurrentbalance(int AccountId, string Issure_name, int Stock)
        {
            CurrentBalance currentBalance = new CurrentBalance { AccountId = AccountId, Issuer_name = Issure_name, Stock = Stock };
            
            Context.Add(currentBalance);

            return currentBalance;
        }
        public CurrentBalance UpdateCurrentbalance(int AccountId, string Issure_name, int Stock)
        {
            CurrentBalance currentBalance = Context.CurrentBalances.Where(a => a.AccountId == AccountId && a.Issuer_name == Issure_name).FirstOrDefault();

            if (currentBalance == null)
            {
                currentBalance = new CurrentBalance { AccountId = AccountId, Issuer_name = Issure_name, Stock = Stock };

                Context.Add(currentBalance);
            } 
            else
            {
                currentBalance.Stock = Stock;
            }

            return currentBalance;
        }        

        public CurrentBalance ChangeCurrentbalance(int AccountId, string Issure_name, int Stock)
        {
            CurrentBalance currentBalance = GetByAccountAndIssure(AccountId, Issure_name);

            if (currentBalance == null)
            {
                currentBalance = AddCurrentbalance(AccountId, Issure_name, Stock);
            }
            else
            {
                UpdateCurrentbalance(AccountId, Issure_name, currentBalance.Stock + Stock);
            }

            return currentBalance;
        }
        public CurrentBalance GetByAccountAndIssure(int AccountId, string Issure_name)
        {
            CurrentBalance currentBalance = Context.CurrentBalances.Where(a => a.AccountId == AccountId && a.Issuer_name == Issure_name).FirstOrDefault();
            return currentBalance;
        }        
    }
}
