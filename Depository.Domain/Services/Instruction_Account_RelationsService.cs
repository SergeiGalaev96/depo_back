using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IInstruction_Account_RelationsService
    {
        Task<EntityOperationResult<instruction_account_relations>> CreateInstruction_Account_Relation(instruction_account_relations instruction_account_relation, Guid user_id_with_credentials);
        Task<EntityOperationResult<instruction_account_relations>> UpdateInstruction_Account_Relation(instruction_account_relations instruction_account_relation, Guid user_id_with_credentials);
        Task<EntityOperationResult<instruction_account_relations>> DeleteInstruction_Account_Relation(int? id, Guid user_id_with_credentials);
    }

    public class Instruction_Account_RelationsService : IInstruction_Account_RelationsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public Instruction_Account_RelationsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }
        public async Task<EntityOperationResult<instruction_account_relations>> CreateInstruction_Account_Relation(instruction_account_relations instruction_account_relation, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_account_relations>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_account_relations>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var _instruction_account_relation = unitOfWork.instruction_account_relations.GetByTypeIdAndShortNumber(instruction_account_relation.instruction_type_id, instruction_account_relation.account_short_number);
                    if (_instruction_account_relation != null)
                    {
                        if (_instruction_account_relation.deleted.Value.Equals(true))
                            return EntityOperationResult<instruction_account_relations>
                                .Failure()
                                .AddError($"Такая запись существует, но ранее была удалена, чтобы появилась запись, восстановите его");
                        else
                            return EntityOperationResult<instruction_account_relations>
                                 .Failure()
                                 .AddError($"Такая запись существует");
                    }
                    instruction_account_relation.created_at = DateTime.Now;
                    instruction_account_relation.updated_at = DateTime.Now;
                    var entity = await unitOfWork.instruction_account_relations.InsertAsync(instruction_account_relation, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instruction_account_relations>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_account_relations>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<instruction_account_relations>> DeleteInstruction_Account_Relation(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_account_relations>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_account_relations>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var instruction_account_relation = unitOfWork.instruction_account_relations.Get(id);
                    if (instruction_account_relation != null)
                    {
                        instruction_account_relation.updated_at = DateTime.Now;
                        instruction_account_relation.deleted = true;
                        unitOfWork.instruction_account_relations.Delete(instruction_account_relation, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<instruction_account_relations>.Success(instruction_account_relation);
                    }
                    else
                        return EntityOperationResult<instruction_account_relations>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_account_relations>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<instruction_account_relations>> UpdateInstruction_Account_Relation(instruction_account_relations instruction_account_relation, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_account_relations>
                            .Failure()
                            .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_account_relations>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var _instruction_account_relation = unitOfWork.instruction_account_relations.GetByTypeIdAndShortNumber(instruction_account_relation.instruction_type_id, instruction_account_relation.account_short_number);
                    if (_instruction_account_relation != null)
                    {
                        if (_instruction_account_relation.deleted.Value.Equals(true) && _instruction_account_relation.id!= instruction_account_relation.id)
                            return EntityOperationResult<instruction_account_relations>
                                .Failure()
                                .AddError($"Такая запись существует, но ранее была удалена, чтобы появилась запись, восстановите его");
                        else if (_instruction_account_relation.id != instruction_account_relation.id)
                            return EntityOperationResult<instruction_account_relations>
                                 .Failure()
                                 .AddError($"Такая запись существует");
                    }
                    instruction_account_relation.updated_at = DateTime.Now;
                    unitOfWork.instruction_account_relations.Update(instruction_account_relation, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instruction_account_relations>.Success(instruction_account_relation);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_account_relations>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
