using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ICorr_DepositoriesService
    {
        Task<EntityOperationResult<corr_depositories>> CreateCorr_Depository(corr_depositories corr_depository, Guid user_id_with_credentials);
        Task<EntityOperationResult<corr_depositories>> UpdateCorr_Depository(corr_depositories corr_depository, Guid user_id_with_credentials);
        Task<EntityOperationResult<corr_depositories>> DeleteCorr_Depository(int? id, Guid user_id_with_credentials);
    }

    public class Corr_DepositoriesService : ICorr_DepositoriesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Corr_DepositoriesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<corr_depositories>> CreateCorr_Depository(corr_depositories corr_depository, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<corr_depositories>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<corr_depositories>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    corr_depository.created_at = DateTime.Now;
                    corr_depository.updated_at = DateTime.Now;
                    var entity = await unitOfWork.corr_depositories.InsertAsync(corr_depository, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<corr_depositories>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<corr_depositories>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<corr_depositories>> DeleteCorr_Depository(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<corr_depositories>
                             .Failure()
                             .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<corr_depositories>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var corr_depository = unitOfWork.corr_depositories.Get(id);
                    if (corr_depository != null)
                    {
                        corr_depository.updated_at = DateTime.Now;
                        corr_depository.deleted = true;
                        unitOfWork.corr_depositories.Delete(corr_depository, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<corr_depositories>.Success(corr_depository);
                    }
                    else
                        return EntityOperationResult<corr_depositories>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<corr_depositories>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<corr_depositories>> UpdateCorr_Depository(corr_depositories corr_depository, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<corr_depositories>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<corr_depositories>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    corr_depository.updated_at = DateTime.Now;
                    unitOfWork.corr_depositories.Update(corr_depository, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<corr_depositories>.Success(corr_depository);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<corr_depositories>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
