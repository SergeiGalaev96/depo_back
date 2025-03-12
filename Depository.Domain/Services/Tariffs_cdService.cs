using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ITariffs_cdService
    {
        Task<EntityOperationResult<tariffs_cd>> CreateTariff_cd(tariffs_cd tariff_cd, Guid user_id_with_credentials);
        Task<EntityOperationResult<tariffs_cd>> UpdateTariff_cd(tariffs_cd tariff_cd, Guid user_id_with_credentials);
        Task<EntityOperationResult<tariffs_cd>> DeleteTariff_cd(int? id, Guid user_id_with_credentials);
    }

    public class Tariffs_cdService : ITariffs_cdService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Tariffs_cdService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<tariffs_cd>> CreateTariff_cd(tariffs_cd tariff_cd, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<tariffs_cd>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<tariffs_cd>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    tariff_cd.created_at = DateTime.Now;
                    tariff_cd.updated_at = DateTime.Now;
                    var entity = await unitOfWork.tariffs_cd.InsertAsync(tariff_cd, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<tariffs_cd>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<tariffs_cd>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<tariffs_cd>> DeleteTariff_cd(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<tariffs_cd>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<tariffs_cd>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var tariff_cd = unitOfWork.tariffs_cd.Get(id);
                    if (tariff_cd != null)
                    {
                        tariff_cd.updated_at = DateTime.Now;
                        tariff_cd.deleted = true;
                        unitOfWork.tariffs_cd.Delete(tariff_cd, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<tariffs_cd>.Success(tariff_cd);
                    }
                    else
                        return EntityOperationResult<tariffs_cd>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<tariffs_cd>.Failure().AddError(ex.ToString());
                }
            }
        }

            public async Task<EntityOperationResult<tariffs_cd>> UpdateTariff_cd(tariffs_cd tariff_cd, Guid user_id_with_credentials)
            {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<tariffs_cd>
                       .Failure()
                       .AddError($"User ID  is null");

                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    try
                    {
                        var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                        if (user_with_credentials == null)
                            return EntityOperationResult<tariffs_cd>
                                .Failure()
                                .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                        tariff_cd.updated_at = DateTime.Now;
                        unitOfWork.tariffs_cd.Update(tariff_cd, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<tariffs_cd>.Success(tariff_cd);
                    }
                    catch (Exception ex)
                    {
                        return EntityOperationResult<tariffs_cd>.Failure().AddError(ex.ToString());
                    }
                }
            }
    }
}
