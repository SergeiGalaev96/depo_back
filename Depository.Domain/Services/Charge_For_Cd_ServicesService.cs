using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ICharge_For_Cd_ServicesService
    {
        Task<EntityOperationResult<charge_for_cd_services>> CreateCharge_For_Cd_Service(charge_for_cd_services charge_for_cd_service, Guid user_id_with_credentials);
        Task<EntityOperationResult<charge_for_cd_services>> UpdateCharge_For_Cd_Service(charge_for_cd_services charge_for_cd_service, Guid user_id_with_credentials);
        Task<EntityOperationResult<charge_for_cd_services>> DeleteCharge_For_Cd_Service(int? id, Guid user_id_with_credentials);
    }

    public class Charge_For_Cd_ServicesService: ICharge_For_Cd_ServicesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Charge_For_Cd_ServicesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<charge_for_cd_services>> CreateCharge_For_Cd_Service(charge_for_cd_services charge_for_cd_service, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<charge_for_cd_services>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<charge_for_cd_services>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    charge_for_cd_service.created_at = DateTime.Now;
                    charge_for_cd_service.updated_at = DateTime.Now;
                    var entity = await unitOfWork.charge_for_cd_services.InsertAsync(charge_for_cd_service, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<charge_for_cd_services>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<charge_for_cd_services>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<charge_for_cd_services>> DeleteCharge_For_Cd_Service(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<charge_for_cd_services>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<charge_for_cd_services>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var charge_for_cd_service = unitOfWork.charge_for_cd_services.Get(id);
                    if (charge_for_cd_service != null)
                    {
                        charge_for_cd_service.updated_at = DateTime.Now;
                        charge_for_cd_service.deleted = true;
                        unitOfWork.charge_for_cd_services.Delete(charge_for_cd_service, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<charge_for_cd_services>.Success(charge_for_cd_service);
                    }
                    else
                        return EntityOperationResult<charge_for_cd_services>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<charge_for_cd_services>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<charge_for_cd_services>> UpdateCharge_For_Cd_Service(charge_for_cd_services charge_for_cd_service, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<charge_for_cd_services>
                            .Failure()
                            .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<charge_for_cd_services>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    charge_for_cd_service.updated_at = DateTime.Now;
                    unitOfWork.charge_for_cd_services.Update(charge_for_cd_service, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<charge_for_cd_services>.Success(charge_for_cd_service);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<charge_for_cd_services>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
    }

