using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ITariffs_Corr_DepositoryService
    {
        Task<EntityOperationResult<tariffs_corr_depository>> CreateTariff_Corr_Depository(tariffs_corr_depository tariff_corr_depository, Guid user_id_with_credentials);
        Task<EntityOperationResult<tariffs_corr_depository>> UpdateTariff_Corr_Depository(tariffs_corr_depository tariff_corr_depository, Guid user_id_with_credentials);
        Task<EntityOperationResult<tariffs_corr_depository>> DeleteTariff_Corr_Depository(int? id, Guid user_id_with_credentials);
    }

    public class Tariffs_Corr_DepositoryService : ITariffs_Corr_DepositoryService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Tariffs_Corr_DepositoryService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<tariffs_corr_depository>> CreateTariff_Corr_Depository(tariffs_corr_depository tariff_corr_depository, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<tariffs_corr_depository>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<tariffs_corr_depository>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    tariff_corr_depository.created_at = DateTime.Now;
                    tariff_corr_depository.updated_at = DateTime.Now;
                    var entity = await unitOfWork.tariffs_corr_depository.InsertAsync(tariff_corr_depository, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<tariffs_corr_depository>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<tariffs_corr_depository>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<tariffs_corr_depository>> DeleteTariff_Corr_Depository(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<tariffs_corr_depository>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<tariffs_corr_depository>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var tariff_corr_depository = unitOfWork.tariffs_corr_depository.Get(id);
                    if (tariff_corr_depository != null)
                    {
                        tariff_corr_depository.updated_at = DateTime.Now;
                        tariff_corr_depository.deleted = true;
                        unitOfWork.tariffs_corr_depository.Delete(tariff_corr_depository, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<tariffs_corr_depository>.Success(tariff_corr_depository);
                    }
                    else
                        return EntityOperationResult<tariffs_corr_depository>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<tariffs_corr_depository>.Failure().AddError(ex.ToString());
                }
            }
        }

            public async Task<EntityOperationResult<tariffs_corr_depository>> UpdateTariff_Corr_Depository(tariffs_corr_depository tariff_corr_depository, Guid user_id_with_credentials)
            {
                if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<tariffs_corr_depository>
                               .Failure()
                               .AddError($"User ID  is null");

                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    try
                    {
                        var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                        if (user_with_credentials == null)
                            return EntityOperationResult<tariffs_corr_depository>
                                .Failure()
                                .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                        tariff_corr_depository.updated_at = DateTime.Now;
                        unitOfWork.tariffs_corr_depository.Update(tariff_corr_depository, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<tariffs_corr_depository>.Success(tariff_corr_depository);
                    }
                    catch (Exception ex)
                    {
                        return EntityOperationResult<tariffs_corr_depository>.Failure().AddError(ex.ToString());
                    }
                }
            }
        }

}
