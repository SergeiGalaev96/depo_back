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
    public interface IMail_TypesService
    {
        Task<EntityOperationResult<mail_types>> Create(mail_types mail_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<mail_types>> Update(mail_types mail_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<mail_types>> Delete(int? id, Guid user_id_with_credentials);
    }

    public class Mail_TypesService : IMail_TypesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Mail_TypesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<mail_types>> Create(mail_types mail_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<mail_types>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {

                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<mail_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");


                    mail_type.created_at = DateTime.Now;
                    mail_type.updated_at = DateTime.Now;

                    var entity = await unitOfWork.mail_types.InsertAsync(mail_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<mail_types>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<mail_types>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<mail_types>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<mail_types>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<mail_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var mail_type = unitOfWork.mail_types.Get(id);
                    if (mail_type != null)
                    {
                        mail_type.updated_at = DateTime.Now;
                        mail_type.deleted = true;
                        unitOfWork.mail_types.Delete(mail_type, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<mail_types>.Success(mail_type);
                    }
                    else
                        return EntityOperationResult<mail_types>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<mail_types>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<mail_types>> Update(mail_types mail_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<mail_types>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<mail_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    mail_type.updated_at = DateTime.Now;
                    unitOfWork.mail_types.Update(mail_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<mail_types>.Success(mail_type);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<mail_types>.Failure().AddError(ex.ToString());
                }
            }
        }
    }

}
