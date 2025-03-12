using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IAccount_StatusesService
    {
        Task<EntityOperationResult<account_statuses>> CreateAccount_Status(account_statuses account_status, Guid user_id_with_credentials);
        Task<EntityOperationResult<account_statuses>> UpdateAccount_Status(account_statuses account_status, Guid user_id_with_credentials);
        Task<EntityOperationResult<account_statuses>> DeleteAccount_Status(int? id, Guid user_id_with_credentials);
    }

    public class Account_StatusesService: IAccount_StatusesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Account_StatusesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<account_statuses>> CreateAccount_Status(account_statuses account_status, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<account_statuses>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<account_statuses>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var is_exist_name = unitOfWork.account_statuses.IsExistName(account_status.name);
                    if (is_exist_name)
                        return EntityOperationResult<account_statuses>
                            .Failure()
                            .AddError($"Объект 'account_members' с полем 'name' уже существует");
                    account_status.created_at = DateTime.Now;
                    account_status.updated_at = DateTime.Now;
                    var entity = await unitOfWork.account_statuses.InsertAsync(account_status, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<account_statuses>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<account_statuses>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<account_statuses>> DeleteAccount_Status(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<account_statuses>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<account_statuses>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var account_status = unitOfWork.account_statuses.Get(id);
                    if (account_status != null)
                    {
                        account_status.updated_at = DateTime.Now;
                        account_status.deleted = true;
                        unitOfWork.account_statuses.Delete(account_status, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<account_statuses>.Success(account_status);
                    }
                    else
                        return EntityOperationResult<account_statuses>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<account_statuses>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<account_statuses>> UpdateAccount_Status(account_statuses account_status, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<account_statuses>
                            .Failure()
                            .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<account_statuses>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    account_status.updated_at = DateTime.Now;
                    unitOfWork.account_statuses.Update(account_status, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<account_statuses>.Success(account_status);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<account_statuses>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
