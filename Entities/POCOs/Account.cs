using System.Collections.Generic;

namespace Entities.POCOs
{
    public class Account
    {
        public int Id { get; set; }
        public decimal Cash { get; set; }
        public List<CurrentBalance> CurrentBalances { get; set; }
        public List<Operation> Operations { get; set; }
    }
}
