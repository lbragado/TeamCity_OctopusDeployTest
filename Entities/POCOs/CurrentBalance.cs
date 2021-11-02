using System.Collections.Generic;

namespace Entities.POCOs
{
    public class CurrentBalance
    {
        public int Id { get; set; }

        public int AccountId { get; set; }
        public string Issuer_name { get; set; }
        public int Stock { get; set; }
        public List<Account> Accounts { get; set; }
    }
}
