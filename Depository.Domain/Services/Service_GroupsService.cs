using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IService_GroupsService
    {
        Task<EntityOperationResult<service_groups>> CreateServiceGroup(service_groups service_group, Guid user_id_with_credentials);
        Task<EntityOperationResult<service_groups>> UpdateServiceGroup(service_groups service_group, Guid user_id_with_credentials);
        Task<EntityOperationResult<service_groups>> DeleteServiceGroup(int? id, Guid user_id_with_credentials);
    }

    public class Service_GroupsService: IService_GroupsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Service_GroupsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<service_groups>> CreateServiceGroup(service_groups service_group, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<service_groups>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<service_groups>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    service_group.created_at = DateTime.Now;
                    service_group.updated_at = DateTime.Now;
                    var entity = await unitOfWork.service_groups.InsertAsync(service_group, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<service_groups>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<service_groups>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<service_groups>> DeleteServiceGroup(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<service_groups>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<service_groups>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var service_group = unitOfWork.service_groups.Get(id);
                    if (service_group != null)
                    {
                        service_group.updated_at = DateTime.Now;
                        service_group.deleted = true;
                        unitOfWork.service_groups.Delete(service_group, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<service_groups>.Success(service_group);
                    }
                    else
                        return EntityOperationResult<service_groups>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<service_groups>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<service_groups>> UpdateServiceGroup(service_groups service_group, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<service_groups>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<service_groups>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    if (service_group.id == null)
                        return EntityOperationResult<service_groups>
                           .Failure()
                           .AddError($"Идентификатор объекта равен нулю");
                    service_group.updated_at = DateTime.Now;
                    unitOfWork.service_groups.Update(service_group, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<service_groups>.Success(service_group);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<service_groups>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
