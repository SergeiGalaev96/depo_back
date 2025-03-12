using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ITransfer_order_statusesService
    {
        Task<EntityOperationResult<transfer_order_statuses>> Create(transfer_order_statuses transfer_order_status, Guid user_id_with_credentials);
        Task<EntityOperationResult<transfer_order_statuses>> Update(transfer_order_statuses transfer_order_status, Guid user_id_with_credentials);
        Task<EntityOperationResult<transfer_order_statuses>> Delete(int? id, Guid user_id_with_credentials);
    }

    public class Transfer_order_statusesService : ITransfer_order_statusesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Transfer_order_statusesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<transfer_order_statuses>> Create(transfer_order_statuses transfer_order_status, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<transfer_order_statuses>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<transfer_order_statuses>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    transfer_order_status.created_at = DateTime.Now;
                    transfer_order_status.updated_at = DateTime.Now;
                    var entity = await unitOfWork.transfer_order_statuses.InsertAsync(transfer_order_status, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<transfer_order_statuses>.Success(transfer_order_status);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<transfer_order_statuses>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<transfer_order_statuses>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<transfer_order_statuses>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<transfer_order_statuses>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var trade = unitOfWork.transfer_order_statuses.Get(id);
                    if (trade != null)
                    {
                        trade.updated_at = DateTime.Now;
                        trade.deleted = true;
                        unitOfWork.transfer_order_statuses.Delete(trade, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<transfer_order_statuses>.Success(trade);
                    }
                    else
                        return EntityOperationResult<transfer_order_statuses>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<transfer_order_statuses>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<transfer_order_statuses>> Update(transfer_order_statuses transfer_order_status, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<transfer_order_statuses>
                         .Failure()
                         .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<transfer_order_statuses>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    transfer_order_status.updated_at = DateTime.Now;
                    unitOfWork.transfer_order_statuses.Update(transfer_order_status, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<transfer_order_statuses>.Success(transfer_order_status);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<transfer_order_statuses>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
