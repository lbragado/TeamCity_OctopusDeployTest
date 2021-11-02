using Entities.POCOs;
using System;

namespace Entities.Interfaces
{
    public interface IOperationRepository
    {
        Operation AddOperation(int AccountId, DateTime DateOperation, string OperationType, string Issuer_name, int Total_shares, decimal Share_price);
    }
}
