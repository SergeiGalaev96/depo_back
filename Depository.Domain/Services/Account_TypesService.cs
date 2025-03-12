using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{

    public interface IAccount_TypesService
    {
        Task<EntityOperationResult<account_types>> CreateAccount_Type(account_types account_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<account_types>> UpdateAccount_Type(account_types account_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<account_types>> DeleteAccount_Type(int? id, Guid user_id_with_credentials);

    }

    public class Account_TypesService : IAccount_TypesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Account_TypesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<account_types>> CreateAccount_Type(account_types account_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<account_types>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<account_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    account_type.created_at = DateTime.Now;
                    account_type.updated_at = DateTime.Now;
                    var entity = await unitOfWork.account_types.InsertAsync(account_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<account_types>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<account_types>.Failure().AddError(JsonConvert.SerializeObject(ex));
                }
            }
        }

        public async Task<EntityOperationResult<account_types>> DeleteAccount_Type(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<account_types>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<account_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var account_type = unitOfWork.account_types.Get(id);
                    if (account_type != null)
                    {
                        account_type.updated_at = DateTime.Now;
                        account_type.deleted = true;
                        unitOfWork.account_types.Delete(account_type, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<account_types>.Success(account_type);
                    }
                    else
                        return EntityOperationResult<account_types>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<account_types>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<account_types>> UpdateAccount_Type(account_types account_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<account_types>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<account_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    account_type.updated_at = DateTime.Now;
                    unitOfWork.account_types.Update(account_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<account_types>.Success(account_type);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<account_types>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}

