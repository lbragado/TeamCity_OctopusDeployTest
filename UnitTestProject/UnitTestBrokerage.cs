using AutoFixture;
using CQRS.Orders.Commands;
using CQRS.Orders.Handlers;
using Entities.Interfaces;
using Entities.POCOs;
using MediatR;
using Moq;
using System.Threading;
using Xunit;

namespace UnitTestProject
{
    
    public class UnitTestBrokerage
    {
        
        [Fact]
        public void BuySellOrderCommandHandlerUnitTest()
        {
            var mediator = new Mock<IMediator>();

            var fixture = new Fixture();
            var command = fixture.Create<BuySellOrderCommand>();

            command.AccountId = 1;
            command.Issuer_name = "APPL";
            command.Operation = "BUY";
            command.Totoal_shares = 5;
            command.Share_price = 5;
            command.Timestamp = 1635886768;

            
            var mockAccountRepository = new Mock<IAccountRepository>();

            int testAccountId = 1;
            mockAccountRepository.Setup(repo => repo.GetById(testAccountId)).Returns(GetAccount());

            var mockOperationRepository = new Mock<IOperationRepository>();
            var mockCurrentbalanceRepository = new Mock<ICurrentbalanceRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();


            var handler = new BuySellOrderCommandHandler(
                mockAccountRepository.Object,
                mockOperationRepository.Object,
                mockCurrentbalanceRepository.Object,
                mockUnitOfWork.Object);

            CancellationToken cancellationToken = new CancellationToken(true);
            var result = handler.Handle(command, cancellationToken);
            Assert.NotNull(result);
            
        }

        private Account GetAccount()
        {            
            var account = new Account()
            {
                Id = 1,
                Cash = 500
            };

            return account;
        }
    }
}
