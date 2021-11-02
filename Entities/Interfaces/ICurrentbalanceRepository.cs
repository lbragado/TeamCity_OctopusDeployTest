using Entities.POCOs;

namespace Entities.Interfaces
{
    public interface ICurrentbalanceRepository
    {
        CurrentBalance AddCurrentbalance(int AccountId, string Issure_name, int Stock);
        CurrentBalance UpdateCurrentbalance(int AccountId, string Issure_name, int Stock);
        CurrentBalance GetByAccountAndIssure(int AccountId, string Issure_name);
        CurrentBalance ChangeCurrentbalance(int AccountId, string Issure_name, int Stock);
    }
}
