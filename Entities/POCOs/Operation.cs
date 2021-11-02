using System;
using System.Collections.Generic;

namespace Entities.POCOs
{
    public class Operation
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime DateOperation { get; set; }
        public string OperationType { get; set; }
        public string Issuer_name { get; set; }
        public int Total_shares { get; set; }
        public decimal Share_price { get; set; }
        public List<Account> Accounts { get; set; }
    }
}
