using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ILimitsService
    {
        Task<EntityOperationResult<limits>> Create(limits limit, Guid user_id_with_credentials);
        Task<EntityOperationResult<limits>> Update(limits limit, Guid user_id_with_credentials);
        Task<EntityOperationResult<limits>> Delete(int? id, Guid user_id_with_credentials);
    }
    public class LimitsService : ILimitsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public LimitsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<limits>> Create(limits limit, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<limits>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<limits>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    limit.created_at = DateTime.Now;
                    limit.updated_at = DateTime.Now;
                    var entity = await unitOfWork.limits.InsertAsync(limit, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<limits>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<limits>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<limits>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<limits>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<limits>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var limit = unitOfWork.limits.Get(id);
                    if (limit != null)
                    {
                        limit.updated_at = DateTime.Now;
                        limit.deleted = true;
                        unitOfWork.limits.Delete(limit, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<limits>.Success(limit);
                    }
                    else
                        return EntityOperationResult<limits>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<limits>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<limits>> Update(limits limit, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<limits>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<limits>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    limit.updated_at = DateTime.Now;
                    unitOfWork.limits.Update(limit, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<limits>.Success(limit);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<limits>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
