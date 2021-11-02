using Entities.Interfaces;
using Entities.POCOs;
using Repository.EFCore.DataContext;
using System;

namespace Repository.EFCore.Repositories
{
    public class OperationRepository : IOperationRepository
    {
        readonly RepositoryContext Context;

        public OperationRepository(RepositoryContext context) => Context = context;

        public Operation AddOperation(int AccountId, DateTime DateOperation, string OperationType, string Issuer_name, int Total_shares, decimal Share_price)
        {
            Operation Operation = new Operation { AccountId = AccountId, OperationType = OperationType, DateOperation = DateOperation, Issuer_name = Issuer_name, Total_shares = Total_shares, Share_price = Share_price };
            Context.Add(Operation);

            return Operation;
        }
    }
}
