using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ILegal_StatusesService
    {
        Task<EntityOperationResult<legal_statuses>> CreateLegal_Status(legal_statuses legal_status, Guid user_id_with_credentials);
        Task<EntityOperationResult<legal_statuses>> UpdateLegal_Status(legal_statuses legal_status, Guid user_id_with_credentials);
        Task<EntityOperationResult<legal_statuses>> DeleteLegal_Status(int? id, Guid user_id_with_credentials);
    }

    public class Legal_StatusesService : ILegal_StatusesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Legal_StatusesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<legal_statuses>> CreateLegal_Status(legal_statuses legal_status, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<legal_statuses>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<legal_statuses>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    legal_status.created_at = DateTime.Now;
                    legal_status.updated_at = DateTime.Now;
                    var entity = await unitOfWork.legal_statuses.InsertAsync(legal_status, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<legal_statuses>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<legal_statuses>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<legal_statuses>> DeleteLegal_Status(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<legal_statuses>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<legal_statuses>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var legal_status = unitOfWork.legal_statuses.Get(id);
                    if (legal_status != null)
                    {
                        legal_status.updated_at = DateTime.Now;
                        legal_status.deleted = true;
                        unitOfWork.legal_statuses.Delete(legal_status, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<legal_statuses>.Success(legal_status);
                    }
                    else
                        return EntityOperationResult<legal_statuses>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<legal_statuses>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<legal_statuses>> UpdateLegal_Status(legal_statuses legal_status, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<legal_statuses>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<legal_statuses>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    legal_status.updated_at = DateTime.Now;
                    unitOfWork.legal_statuses.Update(legal_status, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<legal_statuses>.Success(legal_status);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<legal_statuses>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
