using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IService_TypesService
    {
        Task<EntityOperationResult<service_types>> CreateServiceType(service_types service_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<service_types>> UpdateServiceType(service_types service_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<service_types>> DeleteServiceType(int? id, Guid user_id_with_credentials);
    }

    public class Service_TypesService: IService_TypesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Service_TypesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async  Task<EntityOperationResult<service_types>> CreateServiceType(service_types service_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<service_types>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<service_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var IsExistName = unitOfWork.service_types.IsExistName(service_type.name);
                    if (IsExistName)
                        return EntityOperationResult<service_types>
                            .Failure()
                            .AddError($"Объект с таким наименованием уже существует.");
                    service_type.created_at = DateTime.Now;
                    service_type.updated_at = DateTime.Now;
                    var entity = await unitOfWork.service_types.InsertAsync(service_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<service_types>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<service_types>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<service_types>> DeleteServiceType(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<service_types>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<service_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var service_type = unitOfWork.service_types.Get(id);
                    if (service_type != null)
                    {
                        service_type.updated_at = DateTime.Now;
                        service_type.deleted = true;
                        unitOfWork.service_types.Delete(service_type, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<service_types>.Success(service_type);
                    }
                    else
                        return EntityOperationResult<service_types>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<service_types>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<service_types>> UpdateServiceType(service_types service_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<service_types>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<service_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    service_type.updated_at = DateTime.Now;
                    unitOfWork.service_types.Update(service_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<service_types>.Success(service_type);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<service_types>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
