using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IUser_RolesService
    {
        Task<EntityOperationResult<user_roles>> CreateUserRoles(user_roles user_role, Guid user_id_with_credentials);
        Task<EntityOperationResult<user_roles>> UpdateUserRoles(user_roles user_role, Guid user_id_with_credentials);
        Task<EntityOperationResult<user_roles>> DeleteUserRoles(int? id, Guid user_id_with_credentials);
    }

    public class User_RolesService:IUser_RolesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public User_RolesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async  Task<EntityOperationResult<user_roles>> CreateUserRoles(user_roles user_role, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<user_roles>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<user_roles>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    user_role.created_at = DateTime.Now;
                    user_role.updated_at = DateTime.Now;
                    var entity = await unitOfWork.user_roles.InsertAsync(user_role, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<user_roles>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<user_roles>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<user_roles>> DeleteUserRoles(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<user_roles>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<user_roles>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var user_role = unitOfWork.user_roles.Get(id);
                    if (user_role != null)
                    {
                        user_role.updated_at = DateTime.Now;
                        user_role.deleted = true;
                        unitOfWork.user_roles.Delete(user_role, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<user_roles>.Success(user_role);
                    }
                    else
                        return EntityOperationResult<user_roles>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<user_roles>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<user_roles>> UpdateUserRoles(user_roles user_role, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<user_roles>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<user_roles>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    user_role.updated_at = DateTime.Now;
                    unitOfWork.user_roles.Update(user_role, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<user_roles>.Success(user_role);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<user_roles>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
