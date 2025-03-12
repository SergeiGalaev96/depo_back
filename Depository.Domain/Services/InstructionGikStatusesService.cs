using Depository.Core.Models;
using Depository.Core;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{


    public interface IInstructionGikStatusesService
    {
        Task<EntityOperationResult<instructions_gik_statuses>> Create(instructions_gik_statuses instructions_gik_status, Guid user_id_with_credentials);
        Task<EntityOperationResult<instructions_gik_statuses>> Update(instructions_gik_statuses instructions_gik_status, Guid user_id_with_credentials);
        Task<EntityOperationResult<instructions_gik_statuses>> Delete(int? id, Guid user_id_with_credentials);
    }

    public class InstructionGikStatusesService : IInstructionGikStatusesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public InstructionGikStatusesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<instructions_gik_statuses>> Create(instructions_gik_statuses instructions_gik_status, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instructions_gik_statuses>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instructions_gik_statuses>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var IsExist = unitOfWork.instruction_gik_statuses.IsExistName(instructions_gik_status.name);
                    if (IsExist)
                        return EntityOperationResult<instructions_gik_statuses>
                            .Failure()
                            .AddError($"Объект с таким наименованием уже существует");
                    instructions_gik_status.created_at = DateTime.Now;
                    instructions_gik_status.updated_at = DateTime.Now;
                    var entity = await unitOfWork.instruction_gik_statuses.InsertAsync(instructions_gik_status, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instructions_gik_statuses>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instructions_gik_statuses>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<instructions_gik_statuses>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instructions_gik_statuses>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instructions_gik_statuses>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var instructions_gik_status = unitOfWork.instruction_gik_statuses.Get(id);
                    if (instructions_gik_status != null)
                    {
                        instructions_gik_status.updated_at = DateTime.Now;
                        instructions_gik_status.deleted = true;
                        unitOfWork.instruction_gik_statuses.Delete(instructions_gik_status, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<instructions_gik_statuses>.Success(instructions_gik_status);
                    }
                    else
                        return EntityOperationResult<instructions_gik_statuses>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instructions_gik_statuses>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<instructions_gik_statuses>> Update(instructions_gik_statuses instructions_gik_status, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instructions_gik_statuses>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instructions_gik_statuses>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    instructions_gik_status.updated_at = DateTime.Now;
                    unitOfWork.instruction_gik_statuses.Update(instructions_gik_status, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instructions_gik_statuses>.Success(instructions_gik_status);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instructions_gik_statuses>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
