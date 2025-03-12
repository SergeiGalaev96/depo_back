using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IInstruction_TypesService
    {
        Task<EntityOperationResult<instruction_types>> CreateInstruction_Type(instruction_types instruction_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<instruction_types>> UpdateInstruction_Type(instruction_types instruction_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<instruction_types>> DeleteInstruction_Type(int? id, Guid user_id_with_credentials);
    }

    public class Instruction_TypesService : IInstruction_TypesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Instruction_TypesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<instruction_types>> CreateInstruction_Type(instruction_types instruction_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_types>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    instruction_type.created_at = DateTime.Now;
                    instruction_type.updated_at = DateTime.Now;
                    var entity = await unitOfWork.instruction_types.InsertAsync(instruction_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instruction_types>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_types>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<instruction_types>> DeleteInstruction_Type(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_types>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var instruction_type = unitOfWork.instruction_types.Get(id);
                    if (instruction_type != null)
                    {
                        instruction_type.updated_at = DateTime.Now;
                        instruction_type.deleted = true;
                        unitOfWork.instruction_types.Delete(instruction_type, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<instruction_types>.Success(instruction_type);
                    }
                    else
                        return EntityOperationResult<instruction_types>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_types>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<instruction_types>> UpdateInstruction_Type(instruction_types instruction_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_types>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    instruction_type.updated_at = DateTime.Now;
                    unitOfWork.instruction_types.Update(instruction_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instruction_types>.Success(instruction_type);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_types>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
