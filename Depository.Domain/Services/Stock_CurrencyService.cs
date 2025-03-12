using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IStock_CurrencyService
    {
        Task<EntityOperationResult<stock_currency>> Update(stock_currency stock_currency, Guid user_id_with_credentials);
        Task<EntityOperationResult<stock_currency>> Create(stock_currency stock_currency, Guid user_id_with_credentials);
    }

    public class Stock_CurrencyService : IStock_CurrencyService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Stock_CurrencyService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<stock_currency>> Create(stock_currency stock_currency, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<stock_currency>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<stock_currency>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    stock_currency.created_at = DateTime.Now;
                    stock_currency.updated_at = DateTime.Now;
                    var stock_currency_from_db = unitOfWork.stock_currency.GetByParam(stock_currency.account, stock_currency.currency);
                    if (stock_currency_from_db == null)
                    {
                        var entity = await unitOfWork.stock_currency.InsertAsync(stock_currency, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<stock_currency>.Success(entity);
                    }
                    else
                    {
                        if (stock_currency_from_db.quantity>0 || stock_currency_from_db.quantity_stop>0 || stock_currency_from_db.quantity_pledge>0)
                        {
                            return EntityOperationResult<stock_currency>.Failure().AddError("Данный счет уже существует, просто отредактируйте данный счет");
                        }
                        else
                        {
                            if (stock_currency_from_db.quantity == 0 && stock_currency.quantity > 0)
                            {
                                stock_currency_from_db.quantity = stock_currency.quantity;
                            }
                            if (stock_currency_from_db.quantity_stop == 0 && stock_currency.quantity_stop > 0)
                            {
                                stock_currency_from_db.quantity_stop = stock_currency.quantity_stop;
                            }
                            if (stock_currency_from_db.quantity_pledge == 0 && stock_currency.quantity_pledge > 0)
                            {
                                stock_currency_from_db.quantity_pledge = stock_currency.quantity_pledge;
                            }
                            unitOfWork.stock_currency.Update(stock_currency_from_db, user_with_credentials.id);
                            await unitOfWork.CompleteAsync();
                            return EntityOperationResult<stock_currency>.Success(stock_currency_from_db);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<stock_currency>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<stock_currency>> Update(stock_currency stock_currency, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<stock_currency>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<stock_currency>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    stock_currency.updated_at = DateTime.Now;
                    unitOfWork.stock_currency.Update(stock_currency, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<stock_currency>.Success(stock_currency);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<stock_currency>.Failure().AddError(ex.ToString());
                }
            }
        }

    }
}
