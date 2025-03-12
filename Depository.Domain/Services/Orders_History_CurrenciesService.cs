using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IOrders_History_CurrenciesService
    {
        Task<EntityOperationResult<orders_history_currencies>> CreateOrders_History_Currency(orders_history_currencies orders_history_currency, Guid user_id_with_credentials);
        Task<EntityOperationResult<orders_history_currencies>> UpdateOrders_History_Currency(orders_history_currencies orders_history_currency, Guid user_id_with_credentials);
        Task<EntityOperationResult<orders_history_currencies>> DeleteOrders_History_Currency(int? id, Guid user_id_with_credentials);
    }

    public class Orders_History_CurrenciesService : IOrders_History_CurrenciesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Orders_History_CurrenciesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<orders_history_currencies>> CreateOrders_History_Currency(orders_history_currencies orders_history_currency, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<orders_history_currencies>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<orders_history_currencies>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    orders_history_currency.created_at = DateTime.Now;
                    orders_history_currency.updated_at = DateTime.Now;
                    var entity = await unitOfWork.orders_history_currencies.InsertAsync(orders_history_currency, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<orders_history_currencies>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<orders_history_currencies>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<orders_history_currencies>> DeleteOrders_History_Currency(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<orders_history_currencies>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<orders_history_currencies>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var orders_history_currency = unitOfWork.orders_history_currencies.Get(id);
                    if (orders_history_currency != null)
                    {
                        orders_history_currency.updated_at = DateTime.Now;
                        orders_history_currency.deleted = true;
                        unitOfWork.orders_history_currencies.Delete(orders_history_currency, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<orders_history_currencies>.Success(orders_history_currency);
                    }
                    else
                        return EntityOperationResult<orders_history_currencies>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<orders_history_currencies>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<orders_history_currencies>> UpdateOrders_History_Currency(orders_history_currencies orders_history_currency, Guid user_id_with_credentials)
        {

            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<orders_history_currencies>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<orders_history_currencies>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    orders_history_currency.updated_at = DateTime.Now;
                    unitOfWork.orders_history_currencies.Update(orders_history_currency, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<orders_history_currencies>.Success(orders_history_currency);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<orders_history_currencies>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
