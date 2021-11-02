using CQRS.Orders.Commands;
using Entities.Interfaces;
using Entities.POCOs;
using Entities.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CQRS.Orders.Handlers
{
    public class BuySellOrderCommandHandler
        : IRequestHandler<BuySellOrderCommand, OrderResponse>
    {
        private readonly IAccountRepository AccountRepository;
        readonly ICurrentbalanceRepository CurrentbalanceRepository;
        readonly IOperationRepository OperationRepository;
        readonly IUnitOfWork UnitOfWork;


        public BuySellOrderCommandHandler(
            IAccountRepository accountRepository,
            IOperationRepository operationRepository,
            ICurrentbalanceRepository currentbalanceRepository,
            IUnitOfWork unitOfWork) =>
           (
            AccountRepository,
            OperationRepository,
            CurrentbalanceRepository,
            UnitOfWork) = (
            accountRepository,
            operationRepository,
            currentbalanceRepository,
            unitOfWork);

        public object Handle(BuySellOrderCommand command)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderResponse> Handle(BuySellOrderCommand request, CancellationToken cancellationToken)
        {
            var orderResponse = new OrderResponse() { Business_errors = new List<string>() };
            var issuers = new List<IssuersResponse>();

            //Obtenemos la cuenta
            Account account = AccountRepository.GetById(request.AccountId);
            if (account == null)
            {
                orderResponse.Business_errors.Add("INVALID_ACCOUNT");
                return orderResponse;
            }            

            //Inicializamos el Response de la Orden para regresar el Saldo (Cash) y las emisoras (issuers) incluso si tenemos mensajes de error
            orderResponse.Current_balance = new CurrentBalanceResponse{Cash = account.Cash,Issuers = issuers};

            if (request.Timestamp == 0)            
                orderResponse.Business_errors.Add("Timestamp is null");             

            var TimeStampToConvert = request.Timestamp;            
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(TimeStampToConvert).ToLocalTime();

            DateTime dateNow = DateTime.Now;
            DateTime openTime = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 6, 0, 0);
            DateTime closeTime = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 15, 0, 0);

            //Closed Market: El horario de operaciones debe realizarse entre 06:00 am y 15:00 pm
            if (DateTime.Compare(dateTime, openTime) < 0 || DateTime.Compare(dateTime, closeTime) > 0)            
                orderResponse.Business_errors.Add("CLOSE_MARKET");             

            if (request.Issuer_name == "")            
                orderResponse.Business_errors.Add("IssuerName is empty");                

            if (request.Totoal_shares <= 0)            
                orderResponse.Business_errors.Add("Total shares is wrong");            

            if (request.Share_price <= 0)            
                orderResponse.Business_errors.Add("Share price is wrong");

            //Si existe al menos un mensaje de error detenemos el flujo y regresamos el Response
            if (orderResponse.Business_errors.Count > 0)
                return orderResponse;

            int total_shares = request.Totoal_shares;
            decimal total_price = request.Totoal_shares * request.Share_price;

            if (request.Operation.Trim().ToUpper().Equals("BUY"))
            {
                //Insufficient Balance: sin suficiente efectivo
                if (account.Cash <= 0 || account.Cash < total_price)
                {
                    orderResponse.Business_errors.Add("NOT_ENOUGH_CASH");
                    return orderResponse;
                }

                //Si es una compra el monto total de la operación lo establecemos en negativo 
                total_price = -total_price;
            }
            else if (request.Operation.Trim().ToUpper().Equals("SELL"))
            {
                //Obtenemos el CurrentBalance (balance con el stock de la emisora) para la cuenta indicada
                CurrentBalance currentBalance = CurrentbalanceRepository.GetByAccountAndIssure(request.AccountId, request.Issuer_name);

                //Insufficient Stocks: validamos si cuenta con Stock suficiente
                if (currentBalance == null || currentBalance.Stock < request.Totoal_shares)
                {
                    orderResponse.Business_errors.Add("NOT_ENOUGH_STOCK");
                    return orderResponse;
                }

                total_shares = -total_shares;
            }
            else
            {
                orderResponse.Business_errors.Add("WRONG_OPERATION");
            }            

            //Insertamos (si no existe) o actualizamos el balance (tabla en donde registramos la Cuenta, Issuer y su Stock)
            CurrentbalanceRepository.ChangeCurrentbalance(request.AccountId, request.Issuer_name, total_shares);

            //Registramos la operación, en esta tabla registraremos todas las operaciónes para tener un histórico
            OperationRepository.AddOperation(
                request.AccountId,
                dateTime,
                request.Operation,
                request.Issuer_name,
                request.Totoal_shares,
                request.Share_price
             );

            //Actualizamos el Saldo de la cuenta
            AccountRepository.UpdateAccount(request.AccountId, (account.Cash + total_price));

            orderResponse.Current_balance.Cash = account.Cash;
            orderResponse.Current_balance.Issuers.Add(new IssuersResponse()
            {
                Issuer_name = request.Issuer_name,
                Total_shares = request.Totoal_shares,
                Share_price = request.Share_price
            });

            await UnitOfWork.SaveChangesAsync();

            return orderResponse;
        }        
    }
}
