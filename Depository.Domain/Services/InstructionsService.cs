using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IInstructionsService
    {
        Task<EntityOperationResult<instructions>> CreateInstruction(instructions instruction, Guid user_id_with_credentials);
        Task<EntityOperationResult<instructions>> UpdateInstruction(instructions instruction, Guid user_id_with_credentials);
        Task<EntityOperationResult<instructions>> DeleteInstruction(int? id, Guid user_id_with_credentials);
    }

    public class InstructionsService : IInstructionsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public InstructionsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<instructions>> CreateInstruction(instructions instruction, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instructions>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instructions>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    instruction.created_at = DateTime.Now;
                    instruction.updated_at = DateTime.Now;
                    //instruction.created_user_partner= user_id_with_credentials;
                    var entity = await unitOfWork.instructions.InsertAsync(instruction, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    
                    return EntityOperationResult<instructions>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instructions>.Failure().AddError(ex.Message);
                }
            }
        }

        private bool isClearing(int instruction_type)
        {
            const int CLEARING_CURRENCY = 9;
            const int CLEARING_SECURITY = 10;
            if ((instruction_type== CLEARING_CURRENCY) || (instruction_type== CLEARING_SECURITY))
                    return true;
            else return false;
        }

        public async Task<EntityOperationResult<instructions>> DeleteInstruction(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instructions>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instructions>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var instruction = unitOfWork.instructions.Get(id);
                    if (instruction != null)
                    {
                        instruction.updated_at = DateTime.Now;
                        instruction.deleted = true;
                        unitOfWork.instructions.Delete(instruction, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<instructions>.Success(instruction);
                    }
                    else
                        return EntityOperationResult<instructions>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instructions>.Failure().AddError(ex.Message);
                }
            }
        }



        public async Task<EntityOperationResult<instructions>> UpdateInstruction(instructions instruction, Guid user_id_with_credentials)
        {
            
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instructions>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instructions>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    if (instruction.id==null)
                        return EntityOperationResult<instructions>
                           .Failure()
                           .AddError($"Идентификатор объекта равен нулю");

                   

                    
                    instruction.updated_at = DateTime.Now;
                    unitOfWork.instructions.Update(instruction, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instructions>.Success(instruction);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instructions>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
