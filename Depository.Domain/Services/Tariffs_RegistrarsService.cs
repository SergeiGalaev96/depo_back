using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ITariffs_RegistrarsService
    {
        Task<EntityOperationResult<tariffs_registrars>> CreateTariff_Registrars(tariffs_registrars tariff_registrars, Guid user_id_with_credentials);
        Task<EntityOperationResult<tariffs_registrars>> UpdateTariff_Registrars(tariffs_registrars tariff_registrars, Guid user_id_with_credentials);
        Task<EntityOperationResult<tariffs_registrars>> DeleteTariff_Registrars(int? id, Guid user_id_with_credentials);
    }

    public class Tariffs_RegistrarsService: ITariffs_RegistrarsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Tariffs_RegistrarsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<tariffs_registrars>> CreateTariff_Registrars(tariffs_registrars tariff_registrars, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<tariffs_registrars>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<tariffs_registrars>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    tariff_registrars.created_at = DateTime.Now;
                    tariff_registrars.updated_at = DateTime.Now;
                    var entity = await unitOfWork.tariffs_registrars.InsertAsync(tariff_registrars, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<tariffs_registrars>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<tariffs_registrars>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<tariffs_registrars>> DeleteTariff_Registrars(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<tariffs_registrars>
                             .Failure()
                             .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<tariffs_registrars>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var tariff_registrars = unitOfWork.tariffs_registrars.Get(id);
                    if (tariff_registrars != null)
                    {
                        tariff_registrars.updated_at = DateTime.Now;
                        tariff_registrars.deleted = true;
                        unitOfWork.tariffs_registrars.Delete(tariff_registrars, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<tariffs_registrars>.Success(tariff_registrars);
                    }
                    else
                        return EntityOperationResult<tariffs_registrars>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<tariffs_registrars>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async  Task<EntityOperationResult<tariffs_registrars>> UpdateTariff_Registrars(tariffs_registrars tariff_registrars, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<tariffs_registrars>
                       .Failure()
                       .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<tariffs_registrars>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    tariff_registrars.updated_at = DateTime.Now;
                    unitOfWork.tariffs_registrars.Update(tariff_registrars, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<tariffs_registrars>.Success(tariff_registrars);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<tariffs_registrars>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
