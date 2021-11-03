using AutoFixture;
using CQRS.Orders.Commands;
using CQRS.Orders.Handlers;
using Entities.Interfaces;
using Entities.POCOs;
using Moq;
using System;
using System.Threading;
using Xunit;

namespace UnitTestProject
{
    
    public class UnitTestBuySellOrderCommandHandler
    {
        /// <summary>
        /// Método para probar la Compra de emisora, es decir, con valores correctos.
        /// </summary>
        [Fact]
        public void Buy_Order_Is_Possible()
        {
            //Datos para Mock de la Cuenta (Account) 
            int testAccountId = 1;
            decimal testCashAccount = 500;

            //Valores de Entrada (todos son correctos)
            var fixture = new Fixture();
            var command = fixture.Create<BuySellOrderCommand>();
            command.AccountId = 1;          //Crearemos un mock con una cuenta con Id=1 con saldo suficiente
            command.Issuer_name = "APPL";
            command.Operation = "BUY";      
            command.Totoal_shares = 5;      //Cantidad de emisoras a comprar 
            command.Share_price = 5;        //Precio por emisora, el total será Totoal_shares * Share_price ($25) por lo tanto el Cash es suficiente
            //Establecemos en el campo Timestamp la fecha del día de hoy con un horario válido de operación (6:00 am a 15:00 pm)
            //en nuestro ejemplo estableceremos la fecha del día de hoy a las 7:00 am 
            DateTime dateOperation = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0);
            long unixTime = ((DateTimeOffset)dateOperation).ToUnixTimeSeconds();
            command.Timestamp = (int)unixTime;

            //Indicamos que en el Handler al llamar al método GetById devolverá el Mock de la Cuenta
            var mockAccountRepository = new Mock<IAccountRepository>();                       
            mockAccountRepository.Setup(repo => repo
                                                .GetById(testAccountId))
                                                .Returns(new Account() { Id = testAccountId, Cash = testCashAccount });

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

            //En el Response NO deben de existir elementos en la lista Business_errors, lo cual indicará
            //que no hubo errores
            Assert.True(result != null && result.Result.Business_errors.Count == 0);
        }

        /// <summary>
        /// Método para probar la Compra de emisora en un horario NO Válido de operación, el mercado sólo está abierto 
        /// de 6:00 am a 15:00 pm por lo cual en esta prueba estableceremos la fecha del día de hoy con hora de 5:59
        /// </summary>
        [Fact]
        public void Buy_Order_Is_Not_Possible_Close_Market()
        {
            //Datos para Mock de la Cuenta (Account) 
            int testAccountId = 1;
            decimal testCashAccount = 500;      

            //Valores de Entrada (todos son correctos)
            var fixture = new Fixture();
            var command = fixture.Create<BuySellOrderCommand>();
            command.AccountId = 1;          //Crearemos un mock con una cuenta con Id=1 con saldo suficiente
            command.Issuer_name = "APPL";
            command.Operation = "BUY";
            command.Totoal_shares = 5;      //Cantidad de emisoras a comprar 
            command.Share_price = 30;       //Precio por emisora, el total será Totoal_shares * Share_price ($150) 
            //Establecemos en el campo Timestamp la fecha del día de hoy con un horario NO válido de operación (6:00 am a 15:00 pm)
            //en nuestro ejemplo estableceremos la fecha del día de hoy a las 5:59 am el cual es un horario No válido. 
            DateTime dateOperation = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 5, 59, 0);
            long unixTime = ((DateTimeOffset)dateOperation).ToUnixTimeSeconds();
            command.Timestamp = (int)unixTime;

            //Indicamos que en el Handler al llamar al método GetById devolverá el Mock de la Cuenta (Account)
            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository.Setup(repo => repo
                                                .GetById(testAccountId))
                                                .Returns(new Account() { Id = testAccountId, Cash = testCashAccount });

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
            var businessError = result.Result.Business_errors.Find(e => e.Equals("CLOSE_MARKET"));

            //En el Response debe de existir elementos en la lista Business_errors, lo cual indicará hubo algún error
            Assert.True(!string.IsNullOrEmpty(businessError));
        }

        /// <summary>
        /// Método para probar la Compra de emisora sin saldo (cash) suficiente.
        /// </summary>
        [Fact]
        public void Buy_Order_Is_Not_Possible_Insufficient_Balance()
        {
            //Datos para Mock de la Cuenta (Account) 
            int testAccountId = 1;
            decimal testCashAccount = 100;      //El saldo será insuficiente

            //Valores de Entrada (todos son correctos)
            var fixture = new Fixture();
            var command = fixture.Create<BuySellOrderCommand>();
            command.AccountId = 1;          //Crearemos un mock con una cuenta con Id=1 con saldo suficiente
            command.Issuer_name = "APPL";
            command.Operation = "BUY";
            command.Totoal_shares = 5;      //Cantidad de emisoras a comprar 
            command.Share_price = 30;       //Precio por emisora, el total será Totoal_shares * Share_price ($150) por lo tanto el Cash es Insuficiente
            //Establecemos en el campo Timestamp la fecha del día de hoy con un horario válido de operación (6:00 am a 15:00 pm)
            //en nuestro ejemplo estableceremos la fecha del día de hoy a las 7:00 am 
            DateTime dateOperation = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0);
            long unixTime = ((DateTimeOffset)dateOperation).ToUnixTimeSeconds();
            command.Timestamp = (int)unixTime;

            //Indicamos que en el Handler al llamar al método GetById devolverá el Mock de la Cuenta (Account)
            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository.Setup(repo => repo
                                                .GetById(testAccountId))
                                                .Returns(new Account() { Id = testAccountId, Cash = testCashAccount });

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

            var businessError = result.Result.Business_errors.Find(e => e.Equals("NOT_ENOUGH_CASH"));

            //En el Response debe de existir elementos en la lista Business_errors, lo cual indicará hubo algún error
            Assert.True(!string.IsNullOrEmpty(businessError));
        }

        /// <summary>
        /// Método para probar la Venta de emisora, es decir, con valores correctos.
        /// </summary>
        [Fact]
        public void Sell_Order_Is_Possible()
        {
            //Datos para Mock de la Cuenta (Account) y CurrentBalance
            int testAccountId = 1;
            decimal testCashAccount = 500;
            string testIssuer_name = "APPL";
            int testStock = 50;     //Para esta prueba el Stock debe ser suficiente para poder hacer la venta

            //Valores de Entrada (todos son correctos)
            var fixture = new Fixture();
            var command = fixture.Create<BuySellOrderCommand>();
            command.AccountId = 1;          //Crearemos un mock con una cuenta con Id=1 con saldo suficiente
            command.Issuer_name = "APPL";
            command.Operation = "SELL";
            command.Totoal_shares = 5;      //Cantidad de emisoras a comprar 
            command.Share_price = 5;        //Precio por emisora, el total será Totoal_shares * Share_price ($25) por lo tanto el Cash es suficiente
            //Establecemos en el campo Timestamp la fecha del día de hoy con un horario válido de operación (6:00 am a 15:00 pm)
            //en nuestro ejemplo estableceremos la fecha del día de hoy a las 7:00 am 
            DateTime dateOperation = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0);
            long unixTime = ((DateTimeOffset)dateOperation).ToUnixTimeSeconds();
            command.Timestamp = (int)unixTime;

            //Indicamos que en el Handler al llamar al método GetById devolverá el Mock de la Cuenta
            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository.Setup(repo => repo
                                                .GetById(testAccountId))
                                                .Returns(new Account() { Id = testAccountId, Cash = testCashAccount });

            var mockOperationRepository = new Mock<IOperationRepository>();

            //Indicamos que en el Handler al llamar al método GetByAccountAndIssure devolverá el Mock de Currentbalance
            var mockCurrentbalanceRepository = new Mock<ICurrentbalanceRepository>();
            mockCurrentbalanceRepository.Setup(repo => repo
                                                .GetByAccountAndIssure(testAccountId, testIssuer_name))
                                                .Returns(new CurrentBalance() { 
                                                    Id = testAccountId,
                                                    AccountId=testAccountId,
                                                    Issuer_name=testIssuer_name,
                                                    Stock = testStock
                                                });
            
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var handler = new BuySellOrderCommandHandler(
                mockAccountRepository.Object,
                mockOperationRepository.Object,
                mockCurrentbalanceRepository.Object,
                mockUnitOfWork.Object);

            CancellationToken cancellationToken = new CancellationToken(true);
            
            var result = handler.Handle(command, cancellationToken);

            //En el Response NO deben de existir elementos en la lista Business_errors, lo cual indicará
            //que no hubo errores
            Assert.True(result != null && result.Result.Business_errors.Count == 0);
        }
        
        /// <summary>
        /// Método para probar la Venta de emisoraen un horario NO Válido de operación, el mercado sólo está abierto 
        /// de 6:00 am a 15:00 pm por lo cual en esta prueba estableceremos la fecha del día de hoy con hora de 5:59
        /// </summary>
        [Fact]
        public void Sell_Order_Is_Not_Possible_Close_Market()
        {
            //Datos para Mock de la Cuenta (Account) y CurrentBalance
            int testAccountId = 1;
            decimal testCashAccount = 500;
            string testIssuer_name = "APPL";
            int testStock = 50;

            //Valores de Entrada (todos son correctos)
            var fixture = new Fixture();
            var command = fixture.Create<BuySellOrderCommand>();
            command.AccountId = 1;          //Crearemos un mock con una cuenta con Id=1 con saldo suficiente
            command.Issuer_name = "APPL";
            command.Operation = "SELL";
            command.Totoal_shares = 5;      //Cantidad de emisoras a comprar 
            command.Share_price = 5;        //Precio por emisora, el total será Totoal_shares * Share_price ($25) por lo tanto el Cash es suficiente
            //Establecemos en el campo Timestamp la fecha del día de hoy con un horario NO válido de operación (6:00 am a 15:00 pm)
            //en nuestro ejemplo estableceremos la fecha del día de hoy a las 5:59 am el cual es un horario No válido. 
            DateTime dateOperation = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 5, 59, 0);
            long unixTime = ((DateTimeOffset)dateOperation).ToUnixTimeSeconds();
            command.Timestamp = (int)unixTime;

            //Indicamos que en el Handler al llamar al método GetById devolverá el Mock de la Cuenta
            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository.Setup(repo => repo
                                                .GetById(testAccountId))
                                                .Returns(new Account() { Id = testAccountId, Cash = testCashAccount });

            var mockOperationRepository = new Mock<IOperationRepository>();

            //Indicamos que en el Handler al llamar al método GetByAccountAndIssure devolverá el Mock de Currentbalance
            var mockCurrentbalanceRepository = new Mock<ICurrentbalanceRepository>();
            mockCurrentbalanceRepository.Setup(repo => repo
                                                .GetByAccountAndIssure(testAccountId, testIssuer_name))
                                                .Returns(new CurrentBalance()
                                                {
                                                    Id = testAccountId,
                                                    AccountId = testAccountId,
                                                    Issuer_name = testIssuer_name,
                                                    Stock = testStock
                                                });

            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var handler = new BuySellOrderCommandHandler(
                mockAccountRepository.Object,
                mockOperationRepository.Object,
                mockCurrentbalanceRepository.Object,
                mockUnitOfWork.Object);

            CancellationToken cancellationToken = new CancellationToken(true);
            
            var result = handler.Handle(command, cancellationToken);
            var businessError = result.Result.Business_errors.Find(e => e.Equals("CLOSE_MARKET"));

            //En el Response debe de existir elementos en la lista Business_errors, lo cual indicará hubo algún error
            Assert.True(!string.IsNullOrEmpty(businessError));
        }

        /// <summary>
        /// Método para probar la Venta de emisora sin Stock suficiente
        /// </summary>
        [Fact]
        public void Sell_Order_Is_Not_Possible_Insufficient_Stock()
        {
            //Datos para Mock de la Cuenta (Account) y CurrentBalance
            int testAccountId = 1;
            decimal testCashAccount = 500;
            string testIssuer_name = "APPL";
            int testStock = 20;     //Para esta prueba el Stock NO debe ser suficiente

            //Valores de Entrada (todos son correctos)
            var fixture = new Fixture();
            var command = fixture.Create<BuySellOrderCommand>();
            command.AccountId = 1;          //Crearemos un mock con una cuenta con Id=1 con saldo suficiente
            command.Issuer_name = "APPL";
            command.Operation = "SELL";
            command.Totoal_shares = 25;      //Cantidad de emisoras a comprar, para esta prueba DEBE ser Insuficiente el Stock
            command.Share_price = 5;        //Precio por emisora, el total será Totoal_shares * Share_price ($25) por lo tanto el Cash es suficiente
            //Establecemos en el campo Timestamp la fecha del día de hoy con un horario válido de operación (6:00 am a 15:00 pm)
            //en nuestro ejemplo estableceremos la fecha del día de hoy a las 7:00 am 
            DateTime dateOperation = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0);
            long unixTime = ((DateTimeOffset)dateOperation).ToUnixTimeSeconds();
            command.Timestamp = (int)unixTime;

            //Indicamos que en el Handler al llamar al método GetById devolverá el Mock de la Cuenta
            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository.Setup(repo => repo
                                                .GetById(testAccountId))
                                                .Returns(new Account() { Id = testAccountId, Cash = testCashAccount });

            var mockOperationRepository = new Mock<IOperationRepository>();

            //Indicamos que en el Handler al llamar al método GetByAccountAndIssure devolverá el Mock de Currentbalance
            var mockCurrentbalanceRepository = new Mock<ICurrentbalanceRepository>();
            mockCurrentbalanceRepository.Setup(repo => repo
                                                .GetByAccountAndIssure(testAccountId, testIssuer_name))
                                                .Returns(new CurrentBalance()
                                                {
                                                    Id = testAccountId,
                                                    AccountId = testAccountId,
                                                    Issuer_name = testIssuer_name,
                                                    Stock = testStock
                                                });

            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var handler = new BuySellOrderCommandHandler(
                mockAccountRepository.Object,
                mockOperationRepository.Object,
                mockCurrentbalanceRepository.Object,
                mockUnitOfWork.Object);

            CancellationToken cancellationToken = new CancellationToken(true);
            
            var result = handler.Handle(command, cancellationToken);

            var businessError = result.Result.Business_errors.Find(e => e.Equals("NOT_ENOUGH_STOCK"));

            //En el Response debe de existir elementos en la lista Business_errors, lo cual indicará hubo algún error
            Assert.True(!string.IsNullOrEmpty(businessError));
        }
    }
}
