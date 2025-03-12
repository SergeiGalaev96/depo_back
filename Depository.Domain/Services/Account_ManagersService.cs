using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IAccount_ManagersService
    {
        Task<EntityOperationResult<account_managers>> CreateAccount_Manager(account_managers account_manager, Guid user_id_with_credentials);
        Task<EntityOperationResult<account_managers>> UpdateAccount_Manager(account_managers account_manager, Guid user_id_with_credentials);
        Task<EntityOperationResult<account_managers>> DeleteAccount_Manager(int? id, Guid user_id_with_credentials);
    }

    public class Account_ManagersService : IAccount_ManagersService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Account_ManagersService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<account_managers>> CreateAccount_Manager(account_managers account_manager, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<account_managers>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<account_managers>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    account_manager.created_at = DateTime.Now;
                    account_manager.updated_at = DateTime.Now;
                    var entity = await unitOfWork.account_managers.InsertAsync(account_manager, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<account_managers>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<account_managers>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<account_managers>> DeleteAccount_Manager(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<account_managers>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<account_managers>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var account_manager = unitOfWork.account_managers.Get(id);
                    if (account_manager != null)
                    {
                        account_manager.updated_at = DateTime.Now;
                        account_manager.deleted = true;
                        unitOfWork.account_managers.Delete(account_manager, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<account_managers>.Success(account_manager);
                    }
                    else
                        return EntityOperationResult<account_managers>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<account_managers>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<account_managers>> UpdateAccount_Manager(account_managers account_manager, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<account_managers>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<account_managers>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    account_manager.updated_at = DateTime.Now;
                    unitOfWork.account_managers.Update(account_manager, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<account_managers>.Success(account_manager);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<account_managers>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
