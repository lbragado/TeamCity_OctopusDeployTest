using System.Collections.Generic;

namespace Entities.Wrappers
{
    public class AccountResponse
    {
        public AccountResponse()
        {

        }
        public AccountResponse(int id, decimal cash, List<IssuersResponse> issuers)
        {
            Id = id;
            Cash = cash;
            Issuers = issuers;
        }        

        public int Id { get; set; }
        public decimal Cash { get; set; }
        public List<IssuersResponse> Issuers { get; set; }
    }
}
