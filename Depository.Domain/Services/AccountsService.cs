using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IAccountsService
    {
        Task<EntityOperationResult<accounts>> CreateAccount(accounts account, Guid user_id_with_credentials);
        Task<EntityOperationResult<accounts>> UpdateAccount(accounts account, Guid user_id_with_credentials);
        Task<EntityOperationResult<accounts>> DeleteAccount(int? id, Guid user_id_with_credentials);

    }

    public class AccountService : IAccountsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public AccountService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<accounts>> CreateAccount(accounts account, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<accounts>
                           .Failure()
                           .AddError($"User ID  is null");
            if (account == null)
            {
                return EntityOperationResult<accounts>
                               .Failure()
                               .AddError($"Формат входящих данных некорректен, объект равен нулю!");
            }
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<accounts>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var founded_account = unitOfWork.accounts.FindByAccNumber(account.accnumber);
                    if (founded_account != null)
                    {
                        founded_account.updated_at = DateTime.Now;
                        founded_account.deleted = false;
                        founded_account.dateclosed = null;
                        founded_account.isclosed = false;
                        unitOfWork.accounts.Update(founded_account, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<accounts>.Success(founded_account);
                    }
                    else
                    {
                        account.created_at = DateTime.Now;
                        account.updated_at = DateTime.Now;
                        var entity = await unitOfWork.accounts.InsertAsync(account, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<accounts>.Success(entity);
                    }
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<accounts>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<accounts>> DeleteAccount(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<accounts>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<accounts>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var account = unitOfWork.accounts.Get(id);
                    if (account != null)
                    {
                        account.updated_at = DateTime.Now;
                        account.deleted = true;
                        unitOfWork.accounts.Delete(account, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<accounts>.Success(account);
                    }
                    else
                        return EntityOperationResult<accounts>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<accounts>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<accounts>> UpdateAccount(accounts account, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<accounts>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<accounts>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    account.updated_at = DateTime.Now;
                    unitOfWork.accounts.Update(account, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<accounts>.Success(account);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<accounts>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
