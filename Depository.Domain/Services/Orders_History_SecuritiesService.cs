using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IOrders_History_SecuritiesService
    {
        Task<EntityOperationResult<orders_history_securities>> CreateOrders_History_Security(orders_history_securities orders_history_security, Guid user_id_with_credentials);
        Task<EntityOperationResult<orders_history_securities>> UpdateOrders_History_Security(orders_history_securities orders_history_security, Guid user_id_with_credentials);
        Task<EntityOperationResult<orders_history_securities>> DeleteOrders_History_Security(int? id, Guid user_id_with_credentials);
    }

    public class Orders_History_SecuritiesService : IOrders_History_SecuritiesService
    {

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Orders_History_SecuritiesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<orders_history_securities>> CreateOrders_History_Security(orders_history_securities orders_history_security, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<orders_history_securities>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<orders_history_securities>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    orders_history_security.created_at = DateTime.Now;
                    orders_history_security.updated_at = DateTime.Now;
                    var entity = await unitOfWork.orders_history_securities.InsertAsync(orders_history_security, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<orders_history_securities>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<orders_history_securities>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<orders_history_securities>> DeleteOrders_History_Security(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<orders_history_securities>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<orders_history_securities>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var orders_history_security = unitOfWork.orders_history_securities.Get(id);
                    if (orders_history_security != null)
                    {
                        orders_history_security.updated_at = DateTime.Now;
                        orders_history_security.deleted = true;
                        unitOfWork.orders_history_securities.Delete(orders_history_security, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<orders_history_securities>.Success(orders_history_security);
                    }
                    else
                        return EntityOperationResult<orders_history_securities>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<orders_history_securities>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<orders_history_securities>> UpdateOrders_History_Security(orders_history_securities orders_history_security, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<orders_history_securities>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<orders_history_securities>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    orders_history_security.updated_at = DateTime.Now;
                    unitOfWork.orders_history_securities.Update(orders_history_security, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<orders_history_securities>.Success(orders_history_security);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<orders_history_securities>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
