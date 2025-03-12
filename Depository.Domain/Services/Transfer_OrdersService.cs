using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ITransfer_OrdersService
    {
        Task<EntityOperationResult<transfer_orders>> Create(transfer_orders transfer_order, Guid user_id_with_credentials);
        Task<EntityOperationResult<transfer_orders>> Update(transfer_orders transfer_order, Guid user_id_with_credentials);
        Task<EntityOperationResult<transfer_orders>> Delete(int? id, Guid user_id_with_credentials);
    }

    public class Transfer_OrdersService : ITransfer_OrdersService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Transfer_OrdersService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<transfer_orders>> Create(transfer_orders transfer_order, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<transfer_orders>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<transfer_orders>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");


                    transfer_order.created_at = DateTime.Now;
                    transfer_order.updated_at = DateTime.Now;
                    var entity = await unitOfWork.transfer_orders.InsertAsync(transfer_order, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<transfer_orders>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<transfer_orders>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<transfer_orders>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<transfer_orders>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<transfer_orders>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var transfer_order = unitOfWork.transfer_orders.Get(id);
                    if (transfer_order != null)
                    {
                        transfer_order.updated_at = DateTime.Now;
                        transfer_order.deleted = true;
                        unitOfWork.transfer_orders.Delete(transfer_order, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<transfer_orders>.Success(transfer_order);
                    }
                    else
                        return EntityOperationResult<transfer_orders>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<transfer_orders>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<transfer_orders>> Update(transfer_orders transfer_order, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<transfer_orders>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<transfer_orders>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    transfer_order.updated_at = DateTime.Now;
                    unitOfWork.transfer_orders.Update(transfer_order, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<transfer_orders>.Success(transfer_order);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<transfer_orders>.Failure().AddError(ex.ToString());
                }
            }
        }
    }

}
