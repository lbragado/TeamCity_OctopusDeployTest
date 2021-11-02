using Entities.Wrappers;
using MediatR;

namespace CQRS.Orders.Commands
{
    public class BuySellOrderCommand : IRequest<OrderResponse>
    {
        public int AccountId { get; set; }
        public int Timestamp { get; set; }
        public string Operation { get; set; }
        public string Issuer_name { get; set; }
        public int Totoal_shares { get; set; }
        public int Share_price { get; set; }

    }
}
