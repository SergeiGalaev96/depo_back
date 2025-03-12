using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ITrades_History_CurrenciesService
    {
        Task<EntityOperationResult<trades_history_currencies>> Create(trades_history_currencies trades_history_currency, Guid user_id_with_credentials);
        Task<EntityOperationResult<trades_history_currencies>> Update(trades_history_currencies trades_history_currency, Guid user_id_with_credentials);
        Task<EntityOperationResult<trades_history_currencies>> Delete(int? id, Guid user_id_with_credentials);
    }

    public class Trades_History_CurrenciesService : ITrades_History_CurrenciesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Trades_History_CurrenciesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<trades_history_currencies>> Create(trades_history_currencies trades_history_currency, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<trades_history_currencies>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<trades_history_currencies>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    trades_history_currency.created_at = DateTime.Now;
                    trades_history_currency.updated_at = DateTime.Now;
                    var entity = await unitOfWork.trades_history_currencies.InsertAsync(trades_history_currency, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<trades_history_currencies>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<trades_history_currencies>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<trades_history_currencies>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<trades_history_currencies>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<trades_history_currencies>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var trades_history_currency = unitOfWork.trades_history_currencies.Get(id);
                    if (trades_history_currency != null)
                    {
                        trades_history_currency.updated_at = DateTime.Now;
                        trades_history_currency.deleted = true;
                        unitOfWork.trades_history_currencies.Delete(trades_history_currency, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<trades_history_currencies>.Success(trades_history_currency);
                    }
                    else
                        return EntityOperationResult<trades_history_currencies>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<trades_history_currencies>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<trades_history_currencies>> Update(trades_history_currencies trades_history_currency, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<trades_history_currencies>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<trades_history_currencies>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    trades_history_currency.updated_at = DateTime.Now;
                    unitOfWork.trades_history_currencies.Update(trades_history_currency, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<trades_history_currencies>.Success(trades_history_currency);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<trades_history_currencies>.Failure().AddError(ex.ToString());
                }
            }
        }
    }

}
