using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IInstructions_gikService
    {
        Task<EntityOperationResult<instructions_gik>> Create(instructions_gik instruction_gik, Guid user_id_with_credentials);
        Task<EntityOperationResult<instructions_gik>> Update(instructions_gik instruction_gik, Guid user_id_with_credentials);
        Task<EntityOperationResult<instructions_gik>> Delete(int? id, Guid user_id_with_credentials);
    }
    public class Instructions_gikService : IInstructions_gikService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Instructions_gikService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<instructions_gik>> Create(instructions_gik instruction_gik, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instructions_gik>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instructions_gik>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    instruction_gik.created_at = DateTime.Now;
                    instruction_gik.updated_at = DateTime.Now;
                    var entity = await unitOfWork.instructions_gik.InsertAsync(instruction_gik, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instructions_gik>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instructions_gik>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<instructions_gik>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instructions_gik>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instructions_gik>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var instruction_gik = unitOfWork.instructions_gik.Get(id);
                    if (instruction_gik != null)
                    {
                        instruction_gik.updated_at = DateTime.Now;
                        instruction_gik.deleted = true;
                        unitOfWork.instructions_gik.Delete(instruction_gik, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<instructions_gik>.Success(instruction_gik);
                    }
                    else
                        return EntityOperationResult<instructions_gik>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instructions_gik>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<instructions_gik>> Update(instructions_gik instruction_gik, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instructions_gik>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instructions_gik>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    instruction_gik.updated_at = DateTime.Now;
                    unitOfWork.instructions_gik.Update(instruction_gik, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instructions_gik>.Success(instruction_gik);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instructions_gik>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}

