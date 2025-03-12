using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ITransit_Charge_For_Cd_ServicesService
    {
        Task<EntityOperationResult<transit_charge_for_cd_services>> CreateTransit_Charge_For_Cd_Service(transit_charge_for_cd_services transit_charge_for_cd_service, Guid user_id_with_credentials);
        Task<EntityOperationResult<transit_charge_for_cd_services>> UpdateTransit_Charge_For_Cd_Service(transit_charge_for_cd_services transit_charge_for_cd_service, Guid user_id_with_credentials);
        Task<EntityOperationResult<transit_charge_for_cd_services>> DeleteTransit_Charge_For_Cd_Service(int? id, Guid user_id_with_credentials);
    }

    public class Transit_Charge_For_Cd_ServicesService : ITransit_Charge_For_Cd_ServicesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Transit_Charge_For_Cd_ServicesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<transit_charge_for_cd_services>> CreateTransit_Charge_For_Cd_Service(transit_charge_for_cd_services transit_charge_for_cd_service, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<transit_charge_for_cd_services>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<transit_charge_for_cd_services>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    transit_charge_for_cd_service.created_at = DateTime.Now;
                    transit_charge_for_cd_service.updated_at = DateTime.Now;
                    var entity = await unitOfWork.transit_charge_for_cd_services.InsertAsync(transit_charge_for_cd_service, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<transit_charge_for_cd_services>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<transit_charge_for_cd_services>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<transit_charge_for_cd_services>> DeleteTransit_Charge_For_Cd_Service(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<transit_charge_for_cd_services>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<transit_charge_for_cd_services>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var transit_charge_for_cd_service = unitOfWork.transit_charge_for_cd_services.Get(id);
                    if (transit_charge_for_cd_service != null)
                    {
                        transit_charge_for_cd_service.updated_at = DateTime.Now;
                        transit_charge_for_cd_service.deleted = true;
                        unitOfWork.transit_charge_for_cd_services.Delete(transit_charge_for_cd_service, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<transit_charge_for_cd_services>.Success(transit_charge_for_cd_service);
                    }
                    else
                        return EntityOperationResult<transit_charge_for_cd_services>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<transit_charge_for_cd_services>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<transit_charge_for_cd_services>> UpdateTransit_Charge_For_Cd_Service(transit_charge_for_cd_services transit_charge_for_cd_service, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<transit_charge_for_cd_services>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<transit_charge_for_cd_services>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    transit_charge_for_cd_service.updated_at = DateTime.Now;
                    unitOfWork.transit_charge_for_cd_services.Update(transit_charge_for_cd_service, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<transit_charge_for_cd_services>.Success(transit_charge_for_cd_service);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<transit_charge_for_cd_services>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
