using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IRegionsService
    {
        Task<EntityOperationResult<regions>> CreateRegion(regions region, Guid user_id_with_credentials);
        Task<EntityOperationResult<regions>> UpdateRegion(regions region, Guid user_id_with_credentials);
        Task<EntityOperationResult<regions>> DeleteRegion(int? id, Guid user_id_with_credentials);
    }
    public class RegionsService:IRegionsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public RegionsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<regions>> CreateRegion(regions region, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<regions>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<regions>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var IsExist = unitOfWork.regions.IsExistName(region.name);
                    if (IsExist)
                        return EntityOperationResult<regions>
                            .Failure()
                            .AddError($"Объект с таким наименованием уже существует");
                    region.created_at = DateTime.Now;
                    region.updated_at = DateTime.Now;
                    var entity = await unitOfWork.regions.InsertAsync(region, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<regions>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<regions>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<regions>> DeleteRegion(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<regions>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<regions>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var region = unitOfWork.regions.Get(id);
                    if (region != null)
                    {
                        region.updated_at = DateTime.Now;
                        region.deleted = true;
                        unitOfWork.regions.Delete(region, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<regions>.Success(region);
                    }
                    else
                        return EntityOperationResult<regions>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<regions>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<regions>> UpdateRegion(regions region, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<regions>
                            .Failure()
                            .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<regions>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    region.updated_at = DateTime.Now;
                    unitOfWork.regions.Update(region, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<regions>.Success(region);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<regions>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
