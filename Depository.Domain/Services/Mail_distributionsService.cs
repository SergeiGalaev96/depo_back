using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface  IMail_distributionsService
    {
        Task<EntityOperationResult<mail_distributions>> Create(mail_distributions mail_distribution, Guid user_id_with_credentials);
        Task<EntityOperationResult<mail_distributions>> Update(mail_distributions mail_distribution, Guid user_id_with_credentials);
        Task<EntityOperationResult<mail_distributions>> Delete(int? id, Guid user_id_with_credentials);
    }
    public class Mail_distributionsService : IMail_distributionsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Mail_distributionsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<mail_distributions>> Create(mail_distributions mail_distribution, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<mail_distributions>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<mail_distributions>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    mail_distribution.mails = null;
                    mail_distribution.created_at = DateTime.Now;
                    mail_distribution.updated_at = DateTime.Now;
                    var entity = await unitOfWork.mail_distributions.InsertAsync(mail_distribution, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<mail_distributions>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<mail_distributions>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<mail_distributions>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<mail_distributions>
                         .Failure()
                         .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<mail_distributions>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var mail_distribution = unitOfWork.mail_distributions.Get(id);
                    if (mail_distribution != null)
                    {
                        mail_distribution.updated_at = DateTime.Now;
                        mail_distribution.deleted = true;
                        unitOfWork.mail_distributions.Delete(mail_distribution, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<mail_distributions>.Success(mail_distribution);
                    }
                    else
                        return EntityOperationResult<mail_distributions>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<mail_distributions>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<mail_distributions>> Update(mail_distributions mail_distribution, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<mail_distributions>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<mail_distributions>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    mail_distribution.mails = null;
                    mail_distribution.updated_at = DateTime.Now;
                    unitOfWork.mail_distributions.Update(mail_distribution, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<mail_distributions>.Success(mail_distribution);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<mail_distributions>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
