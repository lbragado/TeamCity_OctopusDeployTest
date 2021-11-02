using Entities.POCOs;

namespace Entities.Interfaces
{
    public interface IAccountRepository
    {
        Account AddAccount(decimal cash);
        Account UpdateAccount(int id, decimal cash);
        Account GetById(int id);
    }
}
