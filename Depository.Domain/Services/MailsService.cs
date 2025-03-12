using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IMailsService
    {
        Task<EntityOperationResult<mails>> Create(mails mail, Guid user_id_with_credentials);
        Task<EntityOperationResult<mails>> Update(mails mail, Guid user_id_with_credentials);
        Task<EntityOperationResult<mails>> Delete(int? id, Guid user_id_with_credentials);
    }

    public class MailsService : IMailsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public MailsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<mails>> Create(mails mail, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<mails>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<mails>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    mail.mail_distributions = null;
                    mail.created_at = DateTime.Now;
                    mail.updated_at = DateTime.Now;
                    var entity = await unitOfWork.mails.InsertAsync(mail, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<mails>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<mails>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<mails>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<mails>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<mails>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var mail = unitOfWork.mails.Get(id);
                    if (mail != null)
                    {

                        mail.updated_at = DateTime.Now;
                        mail.deleted = true;
                        unitOfWork.mails.Delete(mail, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<mails>.Success(mail);
                    }
                    else
                        return EntityOperationResult<mails>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<mails>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<mails>> Update(mails mail, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<mails>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<mails>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    mail.mail_distributions = null;
                    mail.updated_at = DateTime.Now;
                    unitOfWork.mails.Update(mail, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<mails>.Success(mail);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<mails>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}

