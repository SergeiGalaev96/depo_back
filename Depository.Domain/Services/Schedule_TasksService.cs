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
    public interface ISchedule_TasksService
    {
        Task<EntityOperationResult<schedule_tasks>> Create(schedule_tasks schedule_task, Guid user_id_with_credentials);
        Task<EntityOperationResult<schedule_tasks>> Update(schedule_tasks schedule_task, Guid user_id_with_credentials);
        Task<EntityOperationResult<schedule_tasks>> Delete(int? id, Guid user_id_with_credentials);
    }
    public class Schedule_TasksService: ISchedule_TasksService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Schedule_TasksService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async  Task<EntityOperationResult<schedule_tasks>> Create(schedule_tasks schedule_task, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<schedule_tasks>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<schedule_tasks>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    schedule_task.created_at = DateTime.Now;
                    schedule_task.updated_at = DateTime.Now;
                    var entity = await unitOfWork.schedule_tasks.InsertAsync(schedule_task, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<schedule_tasks>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<schedule_tasks>.Failure().AddError(ex.Message);
                }
            }
        }

        public async  Task<EntityOperationResult<schedule_tasks>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<schedule_tasks>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<schedule_tasks>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var schedule_task = unitOfWork.schedule_tasks.Get(id);
                    if (schedule_task != null)
                    {
                        schedule_task.updated_at = DateTime.Now;
                        schedule_task.deleted = true;
                        unitOfWork.schedule_tasks.Delete(schedule_task, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<schedule_tasks>.Success(schedule_task);
                    }
                    else
                        return EntityOperationResult<schedule_tasks>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<schedule_tasks>.Failure().AddError(ex.Message);
                }
            }
        }

        public async  Task<EntityOperationResult<schedule_tasks>> Update(schedule_tasks schedule_task, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<schedule_tasks>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<schedule_tasks>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    schedule_task.updated_at = DateTime.Now;
                    unitOfWork.schedule_tasks.Update(schedule_task, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<schedule_tasks>.Success(schedule_task);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<schedule_tasks>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
