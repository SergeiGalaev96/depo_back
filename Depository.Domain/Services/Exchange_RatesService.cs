using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IExchange_RatesService
    {
        Task<EntityOperationResult<exchange_rates>> CreateExchangeRates(exchange_rates exchange_Rates);
    }
    public class Exchange_RatesService : IExchange_RatesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Exchange_RatesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<exchange_rates>> CreateExchangeRates(exchange_rates exchange_Rates)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var isExistRate = unitOfWork.exchange_rates.isExistRate(exchange_Rates.date, exchange_Rates.currency);
                if (isExistRate) return EntityOperationResult<exchange_rates>
                        .Failure()
                        .AddError($"Курс валют на текущую дату уже существует");
                try
                {
                    var entity = await unitOfWork.exchange_rates.InsertAsyncWithoutHistory(exchange_Rates);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<exchange_rates>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<exchange_rates>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
