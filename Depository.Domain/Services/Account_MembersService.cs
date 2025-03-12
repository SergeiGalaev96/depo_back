using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IAccount_MembersService
    {
        Task<EntityOperationResult<account_members>> CreateAccount_Member(account_members account_member, Guid user_id_with_credentials);
        Task<EntityOperationResult<account_members>> UpdateAccount_Member(account_members account_member, Guid user_id_with_credentials);
        Task<EntityOperationResult<account_members>> DeleteAccount_Member(int? id, Guid user_id_with_credentials);
    }

    public class Account_MembersService: IAccount_MembersService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Account_MembersService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }


        public async Task<EntityOperationResult<account_members>> CreateAccount_Member(account_members account_member, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<account_members>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<account_members>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var is_exist_name = unitOfWork.account_members.IsExistName(account_member.name);
                    if (is_exist_name)
                        return EntityOperationResult<account_members>
                            .Failure()
                            .AddError($"Объект 'account_members' с полем 'name' уже существует");
                    account_member.created_at = DateTime.Now;
                    account_member.updated_at = DateTime.Now;
                    var entity = await unitOfWork.account_members.InsertAsync(account_member, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<account_members>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<account_members>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<account_members>> DeleteAccount_Member(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<account_members>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<account_members>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var account_member = unitOfWork.account_members.Get(id);
                    if (account_member != null)
                    {
                        account_member.updated_at = DateTime.Now;
                        account_member.deleted = true;
                        unitOfWork.account_members.Delete(account_member, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<account_members>.Success(account_member);
                    }
                    else
                        return EntityOperationResult<account_members>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<account_members>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<account_members>> UpdateAccount_Member(account_members account_member, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<account_members>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<account_members>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    account_member.updated_at = DateTime.Now;
                    unitOfWork.account_members.Update(account_member, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<account_members>.Success(account_member);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<account_members>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}

