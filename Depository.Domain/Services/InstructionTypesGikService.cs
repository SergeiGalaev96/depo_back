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


    public interface IInstructionTypesGikService
    {
        Task<EntityOperationResult<instruction_types_gik>> Create(instruction_types_gik instruction_types_gik, Guid user_id_with_credentials);
        Task<EntityOperationResult<instruction_types_gik>> Update(instruction_types_gik instruction_types_gik, Guid user_id_with_credentials);
        Task<EntityOperationResult<instruction_types_gik>> Delete(int? id, Guid user_id_with_credentials);
    }
    public class InstructionTypesGikService : IInstructionTypesGikService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public InstructionTypesGikService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<instruction_types_gik>> Create(instruction_types_gik instruction_type_gik, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_types_gik>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_types_gik>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    instruction_type_gik.created_at = DateTime.Now;
                    instruction_type_gik.updated_at = DateTime.Now;
                    var entity = await unitOfWork.instruction_types_gik.InsertAsync(instruction_type_gik, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instruction_types_gik>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_types_gik>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<instruction_types_gik>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_types_gik>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_types_gik>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var instruction_type_gik = unitOfWork.instruction_types_gik.Get(id);
                    if (instruction_type_gik != null)
                    {
                        instruction_type_gik.updated_at = DateTime.Now;
                        instruction_type_gik.deleted = true;
                        unitOfWork.instruction_types_gik.Delete(instruction_type_gik, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<instruction_types_gik>.Success(instruction_type_gik);
                    }
                    else
                        return EntityOperationResult<instruction_types_gik>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_types_gik>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<instruction_types_gik>> Update(instruction_types_gik instruction_type_gik, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_types_gik>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_types_gik>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    instruction_type_gik.updated_at = DateTime.Now;
                    unitOfWork.instruction_types_gik.Update(instruction_type_gik, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instruction_types_gik>.Success(instruction_type_gik);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_types_gik>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
