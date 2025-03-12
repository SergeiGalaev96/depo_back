using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IOper_DaysService
    {
        Task<EntityOperationResult<oper_days>> Create(oper_days oper_day, Guid user_id_with_credentials);
        Task<EntityOperationResult<oper_days>> Open(oper_days oper_day, Guid user_id_with_credentials);
        Task<EntityOperationResult<oper_days>> Close(oper_days oper_day, Guid user_id_with_credentials);

        Task<EntityOperationResult<oper_days>> Update(oper_days oper_day, Guid user_id_with_credentials);
        Task<EntityOperationResult<oper_days>> Delete(int? id, Guid user_id_with_credentials);
    }

    public class Oper_DaysService : IOper_DaysService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Oper_DaysService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<oper_days>> Close(oper_days oper_day, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<oper_days>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<oper_days>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var isExist = unitOfWork.oper_days.IsDayOpened(oper_day.date);
                    if (!isExist)
                        return EntityOperationResult<oper_days>
                            .Failure()
                            .AddError($"Операционный день еще не открыт!");
                    var isClosed = unitOfWork.oper_days.IsDayClosed(oper_day.date);
                    if (isClosed)
                        return EntityOperationResult<oper_days>
                            .Failure()
                            .AddError($"Операционный день уже закрыт!");
                    var oper_days = unitOfWork.oper_days.GetByDate(oper_day.date.Date);

                    oper_days.created_at = DateTime.Now;
                    oper_days.updated_at = DateTime.Now;
                    oper_days.close = oper_day.close;
                    unitOfWork.oper_days.Update(oper_days, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<oper_days>.Success(oper_days);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<oper_days>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<oper_days>> Open(oper_days oper_day, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<oper_days>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<oper_days>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var isExist = unitOfWork.oper_days.IsDayOpened(oper_day.date);
                    if (isExist)
                        return EntityOperationResult<oper_days>
                            .Failure()
                            .AddError($"Операционный уже открыт!");

                    oper_day.created_at = DateTime.Now;
                    oper_day.updated_at = DateTime.Now;
                    var entity = await unitOfWork.oper_days.InsertAsync(oper_day, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<oper_days>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<oper_days>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<oper_days>> Create(oper_days oper_day, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<oper_days>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<oper_days>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var isExist = unitOfWork.oper_days.IsExist(oper_day.date);
                    if (isExist)
                        return EntityOperationResult<oper_days>
                            .Failure()
                            .AddError($"Операционный день на эту дату уже создан!");
                    oper_day.created_at = DateTime.Now;
                    oper_day.updated_at = DateTime.Now;
                    var entity = await unitOfWork.oper_days.InsertAsync(oper_day, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<oper_days>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<oper_days>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<oper_days>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<oper_days>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<oper_days>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var oper_day = unitOfWork.oper_days.Get(id);
                    if (oper_day != null)
                    {
                        oper_day.updated_at = DateTime.Now;
                        oper_day.deleted = true;
                        unitOfWork.oper_days.Delete(oper_day, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<oper_days>.Success(oper_day);
                    }
                    else
                        return EntityOperationResult<oper_days>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<oper_days>.Failure().AddError(ex.ToString());
                }
            }
        }

       

        public async Task<EntityOperationResult<oper_days>> Update(oper_days oper_day, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<oper_days>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<oper_days>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    
                    oper_day.updated_at = DateTime.Now;
                    unitOfWork.oper_days.Update(oper_day, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<oper_days>.Success(oper_day);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<oper_days>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
