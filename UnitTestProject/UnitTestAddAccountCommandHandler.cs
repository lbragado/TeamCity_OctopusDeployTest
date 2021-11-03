using AutoFixture;
using CQRS.Accounts.Commands;
using CQRS.Accounts.Handlers;
using Entities.Interfaces;
using Entities.POCOs;
using Entities.Wrappers;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace UnitTestProject
{
    public class UnitTestAddAccountCommandHandler
    {
        /// <summary>
        /// Método para probar la creación de unacuenta con su saldo.
        /// </summary>
        [Fact]
        public void Add_Account_Is_Possible()
        {
            //Datos para Mock de la Cuenta (Account) 
            int testAccountId = 1;
            decimal testCashAccount = 500;            

            //Valores de Entrada (todos son correctos)
            var fixture = new Fixture();
            var command = fixture.Create<AddAccountCommand>();
            command.Cash = testCashAccount;

            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository.Setup(repo => repo
                                                .AddAccount(testCashAccount))
                                                .Returns(new Account() {Id= testAccountId, Cash=testCashAccount  });

            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var handler = new AddAccountCommandHandler(
                mockAccountRepository.Object,
                mockUnitOfWork.Object);

            CancellationToken cancellationToken = new CancellationToken(true);

            var result = handler.Handle(command, cancellationToken);

            Assert.Equal(testAccountId, result.Result.Id);
        }

        /// <summary>
        /// Método para probar la creación de una cuenta sin introducir saldo (cash) por lo tanto se debe generar 
        /// una excepción ArgumentNullException en el AddAccountCommandHandler
        /// </summary>
        [Fact]        
        public async System.Threading.Tasks.Task Add_Account_Is_Not_Possible_Without_Cash()
        {
            //Datos para Mock de la Cuenta (Account) 
            int testAccountId = 1;
            decimal testCashAccount = 0;

            //Valores de Entrada (todos son correctos)
            var fixture = new Fixture();
            var command = fixture.Create<AddAccountCommand>();
            command.Cash = testCashAccount;

            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository.Setup(repo => repo
                                                .AddAccount(testCashAccount))
                                                .Returns(new Account() { Id = testAccountId, Cash = testCashAccount });

            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var handler = new AddAccountCommandHandler(
                mockAccountRepository.Object,
                mockUnitOfWork.Object);

            CancellationToken cancellationToken = new CancellationToken(true);

            var result = handler.Handle(command, cancellationToken);

            await Assert.ThrowsAsync<ArgumentNullException>(()=> handler.Handle(command, cancellationToken));
            
        }
    }
}
