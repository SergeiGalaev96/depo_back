using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IAccounts_GikService
    {
        Task<EntityOperationResult<accounts_gik>> CreateAccount_Gik(accounts_gik account_gik, Guid user_id_with_credentials);
        Task<EntityOperationResult<accounts_gik>> UpdateAccount_Gik(accounts_gik account_gik, Guid user_id_with_credentials);
        Task<EntityOperationResult<accounts_gik>> DeleteAccount_Gik(int? id, Guid user_id_with_credentials);
    }

    public class Accounts_GikService : IAccounts_GikService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Accounts_GikService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<accounts_gik>> CreateAccount_Gik(accounts_gik account_gik, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<accounts_gik>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<accounts_gik>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    account_gik.created_at = DateTime.Now;
                    account_gik.updated_at = DateTime.Now;
                    var entity = await unitOfWork.accounts_gik.InsertAsync(account_gik, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<accounts_gik>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<accounts_gik>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<accounts_gik>> DeleteAccount_Gik(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<accounts_gik>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<accounts_gik>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var account_gik = unitOfWork.accounts_gik.Get(id);
                    if (account_gik != null)
                    {
                        account_gik.updated_at = DateTime.Now;
                        account_gik.deleted = true;
                        unitOfWork.accounts_gik.Delete(account_gik, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<accounts_gik>.Success(account_gik);
                    }
                    else
                        return EntityOperationResult<accounts_gik>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<accounts_gik>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<accounts_gik>> UpdateAccount_Gik(accounts_gik account_gik, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<accounts_gik>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<accounts_gik>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    account_gik.updated_at = DateTime.Now;
                    unitOfWork.accounts_gik.Update(account_gik, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<accounts_gik>.Success(account_gik);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<accounts_gik>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
    }
