using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ILocalitiesService
    {
        Task<EntityOperationResult<localities>> CreateLocality(localities locality, Guid user_id_with_credentials);
        Task<EntityOperationResult<localities>> UpdateLocality(localities locality, Guid user_id_with_credentials);
        Task<EntityOperationResult<localities>> DeleteLocality(int? id, Guid user_id_with_credentials);
    }

    public class LocalitiesService: ILocalitiesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public LocalitiesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<localities>> CreateLocality(localities locality, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<localities>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<localities>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    locality.created_at = DateTime.Now;
                    locality.updated_at = DateTime.Now;
                    var entity = await unitOfWork.localities.InsertAsync(locality, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<localities>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<localities>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<localities>> DeleteLocality(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<localities>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<localities>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var locality = unitOfWork.localities.Get(id);
                    if (locality!=null)
                    {
                        locality.updated_at = DateTime.Now;
                        locality.deleted = true;
                        unitOfWork.localities.Delete(locality, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<localities>.Success(locality);
                    }
                    else
                        return EntityOperationResult<localities>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<localities>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<localities>> UpdateLocality(localities locality, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<localities>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<localities>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    locality.updated_at = DateTime.Now;
                    unitOfWork.localities.Update(locality, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<localities>.Success(locality);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<localities>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
