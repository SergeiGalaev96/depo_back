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

    public interface IThsTasksService
    {
        Task<EntityOperationResult<ths_tasks>> Create(ths_tasks ths_task, Guid user_id_with_credentials);
        Task<EntityOperationResult<ths_tasks>> Update(ths_tasks ths_task, Guid user_id_with_credentials);
        Task<EntityOperationResult<ths_tasks>> Delete(int? id, Guid user_id_with_credentials);
    }
    public class ThsTasksService: IThsTasksService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public ThsTasksService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<ths_tasks>> Create(ths_tasks ths_task, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<ths_tasks>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<ths_tasks>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    ths_task.created_at = DateTime.Now;
                    ths_task.updated_at = DateTime.Now;
                    var entity = await unitOfWork.ths_tasks.InsertAsync(ths_task, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<ths_tasks>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<ths_tasks>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<ths_tasks>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<ths_tasks>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<ths_tasks>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var ths_task = unitOfWork.ths_tasks.Get(id);
                    if (ths_task != null)
                    {
                        ths_task.updated_at = DateTime.Now;
                        ths_task.deleted = true;
                        unitOfWork.ths_tasks.Delete(ths_task, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<ths_tasks>.Success(ths_task);
                    }
                    else
                        return EntityOperationResult<ths_tasks>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<ths_tasks>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<ths_tasks>> Update(ths_tasks ths_task, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<ths_tasks>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<ths_tasks>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    ths_task.updated_at = DateTime.Now;
                    unitOfWork.ths_tasks.Update(ths_task, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<ths_tasks>.Success(ths_task);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<ths_tasks>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
