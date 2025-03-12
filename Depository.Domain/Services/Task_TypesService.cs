using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{

    public interface ITask_TypesService
    {
        Task<EntityOperationResult<task_types>> Create(task_types task_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<task_types>> Update(task_types task_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<task_types>> Delete(int? id, Guid user_id_with_credentials);
    }
    public class Task_TypesService: ITask_TypesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Task_TypesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<task_types>> Create(task_types task_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<task_types>
                        .Failure()
                        .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<task_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    task_type.created_at = DateTime.Now;
                    task_type.updated_at = DateTime.Now;
                    var entity = await unitOfWork.task_types.InsertAsync(task_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<task_types>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<task_types>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<task_types>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<task_types>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<task_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var task_type = unitOfWork.task_types.Get(id);
                    if (task_type != null)
                    {
                        task_type.updated_at = DateTime.Now;
                        task_type.deleted = true;
                        unitOfWork.task_types.Delete(task_type, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<task_types>.Success(task_type);
                    }
                    else
                        return EntityOperationResult<task_types>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<task_types>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<task_types>> Update(task_types task_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<task_types>
                         .Failure()
                         .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<task_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    task_type.updated_at = DateTime.Now;
                    unitOfWork.task_types.Update(task_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<task_types>.Success(task_type);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<task_types>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
