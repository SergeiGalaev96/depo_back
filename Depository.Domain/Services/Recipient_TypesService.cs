using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IRecipient_TypesService
    {
        Task<EntityOperationResult<recipient_types>> CreateRecipient_Type(recipient_types recipient_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<recipient_types>> UpdateRecipient_Type(recipient_types recipient_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<recipient_types>> DeleteRecipient_Type(int? id, Guid user_id_with_credentials);

    }

    public class Recipient_TypesService : IRecipient_TypesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Recipient_TypesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<recipient_types>> CreateRecipient_Type(recipient_types recipient_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<recipient_types>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<recipient_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    recipient_type.created_at = DateTime.Now;
                    recipient_type.updated_at = DateTime.Now;
                    var entity = await unitOfWork.recipient_types.InsertAsync(recipient_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<recipient_types>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<recipient_types>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<recipient_types>> DeleteRecipient_Type(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<recipient_types>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<recipient_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var recipient_type = unitOfWork.recipient_types.Get(id);
                    if (recipient_type != null)
                    {
                        recipient_type.updated_at = DateTime.Now;
                        recipient_type.deleted = true;
                        unitOfWork.recipient_types.Delete(recipient_type, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<recipient_types>.Success(recipient_type);
                    }
                    else
                        return EntityOperationResult<recipient_types>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<recipient_types>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<recipient_types>> UpdateRecipient_Type(recipient_types recipient_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<recipient_types>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<recipient_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    recipient_type.updated_at = DateTime.Now;
                    unitOfWork.recipient_types.Update(recipient_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<recipient_types>.Success(recipient_type);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<recipient_types>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
