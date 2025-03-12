using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IInstruction_StatusesService
    {
        Task<EntityOperationResult<instruction_statuses>> Create(instruction_statuses instruction_status, Guid user_id_with_credentials);
        Task<EntityOperationResult<instruction_statuses>> Update(instruction_statuses instruction_status, Guid user_id_with_credentials);
        Task<EntityOperationResult<instruction_statuses>> Delete(int? id, Guid user_id_with_credentials);
    }
    public class Instruction_StatusesService : IInstruction_StatusesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Instruction_StatusesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<instruction_statuses>> Create(instruction_statuses instruction_status, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_statuses>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_statuses>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var isExistAccNumber = unitOfWork.instruction_statuses.isExistName(instruction_status.name); 
                    if (isExistAccNumber) return EntityOperationResult<instruction_statuses>
                           .Failure()
                           .AddError($"Объект с таким именем уже существует");
                    instruction_status.created_at = DateTime.Now;
                    instruction_status.updated_at = DateTime.Now;
                    var entity = await unitOfWork.instruction_statuses.InsertAsync(instruction_status, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instruction_statuses>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_statuses>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<instruction_statuses>> Delete(int? id, Guid user_id_with_credentials)
        {

            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_statuses>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_statuses>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var instruction_status = unitOfWork.instruction_statuses.Get(id);
                    if (instruction_status != null)
                    {
                        instruction_status.updated_at = DateTime.Now;
                        instruction_status.deleted = true;
                        unitOfWork.instruction_statuses.Delete(instruction_status, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<instruction_statuses>.Success(instruction_status);
                    }
                    else
                        return EntityOperationResult<instruction_statuses>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_statuses>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<instruction_statuses>> Update(instruction_statuses instruction_status, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_statuses>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_statuses>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    instruction_status.updated_at = DateTime.Now;
                    unitOfWork.instruction_statuses.Update(instruction_status, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instruction_statuses>.Success(instruction_status);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_statuses>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
