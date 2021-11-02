using System.Collections.Generic;

namespace Entities.Wrappers
{
    public class CurrentBalanceResponse
    {
        public decimal Cash { get; set; }
        public List<IssuersResponse> Issuers { get; set; }
    }
}
