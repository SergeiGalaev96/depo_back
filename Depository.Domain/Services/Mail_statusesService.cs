using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IMail_statusesService
    {
        Task<EntityOperationResult<mail_statuses>> Create(mail_statuses mail_status, Guid user_id_with_credentials);
        Task<EntityOperationResult<mail_statuses>> Update(mail_statuses mail_status, Guid user_id_with_credentials);
        Task<EntityOperationResult<mail_statuses>> Delete(int? id, Guid user_id_with_credentials);
    }

    public class Mail_statusesService : IMail_statusesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Mail_statusesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<mail_statuses>> Create(mail_statuses mail_status, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<mail_statuses>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<mail_statuses>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    mail_status.created_at = DateTime.Now;
                    mail_status.updated_at = DateTime.Now;
                    var entity = await unitOfWork.mail_statuses.InsertAsync(mail_status, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<mail_statuses>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<mail_statuses>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<mail_statuses>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<mail_statuses>
                         .Failure()
                         .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<mail_statuses>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var mail_status = unitOfWork.mail_statuses.Get(id);
                    if (mail_status != null)
                    {
                        mail_status.updated_at = DateTime.Now;
                        mail_status.deleted = true;
                        unitOfWork.mail_statuses.Delete(mail_status, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<mail_statuses>.Success(mail_status);
                    }
                    else
                        return EntityOperationResult<mail_statuses>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<mail_statuses>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<mail_statuses>> Update(mail_statuses mail_status, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<mail_statuses>
                        .Failure()
                        .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<mail_statuses>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    mail_status.updated_at = DateTime.Now;
                    unitOfWork.mail_statuses.Update(mail_status, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<mail_statuses>.Success(mail_status);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<mail_statuses>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
