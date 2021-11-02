using System.Collections.Generic;

namespace Entities.Wrappers
{
    public class OrderResponse
    {
        public OrderResponse()
        {

        }
        public OrderResponse(CurrentBalanceResponse current_balance, List<string> business_errors)
        {
            Current_balance = current_balance;
            Business_errors = business_errors;
        }

        public CurrentBalanceResponse Current_balance { get; set; }
        public List<string> Business_errors { get; set; }
    }        
}
