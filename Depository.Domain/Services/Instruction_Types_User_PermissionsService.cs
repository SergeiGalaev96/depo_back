using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IInstruction_Types_User_PermissionsService
    {
        Task<EntityOperationResult<instruction_types_user_permissions>> Create(instruction_types_user_permissions instruction_types_user_permission, Guid user_id_with_credentials);
        Task<EntityOperationResult<instruction_types_user_permissions>> Update(instruction_types_user_permissions instruction_types_user_permission, Guid user_id_with_credentials);
        Task<EntityOperationResult<instruction_types_user_permissions>> Delete(int? id, Guid user_id_with_credentials);
    }

    public class Instruction_Types_User_PermissionsService : IInstruction_Types_User_PermissionsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Instruction_Types_User_PermissionsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<instruction_types_user_permissions>> Create(instruction_types_user_permissions instruction_types_user_permission, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_types_user_permissions>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_types_user_permissions>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");


                    instruction_types_user_permission.created_at = DateTime.Now;
                    instruction_types_user_permission.updated_at = DateTime.Now;
                    var entity = await unitOfWork.instruction_types_user_permissions.InsertAsync(instruction_types_user_permission, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instruction_types_user_permissions>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_types_user_permissions>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async  Task<EntityOperationResult<instruction_types_user_permissions>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_types_user_permissions>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_types_user_permissions>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var instruction_types_user_permission = unitOfWork.instruction_types_user_permissions.Get(id);
                    if (instruction_types_user_permission != null)
                    {
                        instruction_types_user_permission.updated_at = DateTime.Now;
                        instruction_types_user_permission.deleted = true;
                        unitOfWork.instruction_types_user_permissions.Delete(instruction_types_user_permission, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<instruction_types_user_permissions>.Success(instruction_types_user_permission);
                    }
                    else
                        return EntityOperationResult<instruction_types_user_permissions>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_types_user_permissions>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<instruction_types_user_permissions>> Update(instruction_types_user_permissions instruction_types_user_permission, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_types_user_permissions>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_types_user_permissions>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    instruction_types_user_permission.updated_at = DateTime.Now;
                    unitOfWork.instruction_types_user_permissions.Update(instruction_types_user_permission, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instruction_types_user_permissions>.Success(instruction_types_user_permission);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_types_user_permissions>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
