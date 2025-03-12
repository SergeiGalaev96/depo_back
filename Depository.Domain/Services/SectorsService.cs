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
    public interface ISectorsService
    {
        Task<EntityOperationResult<sectors>> Create(sectors sector, Guid user_id_with_credentials);
        Task<EntityOperationResult<sectors>> Update(sectors sector, Guid user_id_with_credentials);
        Task<EntityOperationResult<sectors>> Delete(int? id, Guid user_id_with_credentials);
    }
    public class SectorsService: ISectorsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public SectorsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<sectors>> Create(sectors sector, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<sectors>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<sectors>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    sector.created_at = DateTime.Now;
                    sector.updated_at = DateTime.Now;
                    var entity = await unitOfWork.sectors.InsertAsync(sector, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<sectors>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<sectors>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<sectors>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<sectors>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<sectors>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var sector = unitOfWork.sectors.Get(id);
                    if (sector != null)
                    {
                        sector.updated_at = DateTime.Now;
                        sector.deleted = true;
                        unitOfWork.sectors.Delete(sector, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<sectors>.Success(sector);
                    }
                    else
                        return EntityOperationResult<sectors>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<sectors>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<sectors>> Update(sectors sector, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<sectors>
                        .Failure()
                        .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<sectors>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    sector.updated_at = DateTime.Now;

                    unitOfWork.sectors.Update(sector, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<sectors>.Success(sector);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<sectors>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
