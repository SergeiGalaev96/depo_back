using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IUsersService
    {
        Task<EntityOperationResult<users>> CreateUser(users user, Guid user_id);
        Task<EntityOperationResult<users>> UpdateUser(users user, Guid user_id_with_credentials);
        Task<EntityOperationResult<users>> DeleteUser(Guid user_id, Guid user_id_with_credentials);
    }
    public class UsersService : IUsersService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public UsersService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<users>> CreateUser(users user, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<users>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                    try
                    {
                        var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                        if (user_with_credentials == null)
                            return EntityOperationResult<users>
                                .Failure()
                                .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                        var isExistUserId = unitOfWork.users.IsExistUserId(user.user_id);

                        if (isExistUserId)
                            return EntityOperationResult<users>
                                .Failure()
                                .AddError($"Пользователя которого пытаетесь создать, с таким идентификатором уже существует");
                        user.created_at = DateTime.Now;
                        user.updated_at = DateTime.Now;
                        var entity = await unitOfWork.users.InsertAsync(user, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<users>.Success(entity);
                    }
                    catch (Exception ex)
                    {
                        return EntityOperationResult<users>.Failure().AddError(ex.Message);
                    }
            }
        }

        public async Task<EntityOperationResult<users>> DeleteUser(Guid user_id, Guid user_id_with_credentials)// Пользователь не может удалить самого себя, удалить можно с помощью другого аккаунта(повыше полномочиями).
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<users>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user = unitOfWork.users.GetByUserId(user_id);
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user == null)
                        return EntityOperationResult<users>
                            .Failure()
                            .AddError($"Пользователя которого пытаетесь удалить, с таким идентификатором не существует");
                    else if (user_with_credentials == null)
                        return EntityOperationResult<users>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    else
                    {
                        user.updated_at = DateTime.Now;
                        user.deleted = true;
                        unitOfWork.users.Delete(user, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<users>.Success(user);
                    }
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<users>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<users>> UpdateUser(users user, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<users>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var userFromDb = unitOfWork.users.GetByUserId(user.user_id);
                    if (userFromDb==null) return EntityOperationResult<users>
                         .Failure()
                         .AddError($"Пользователь с таким идентификатором не существует");
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<users>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                       // user.id = userFromDb.id;
                        user.updated_at = DateTime.Now;
                        userFromDb.firstname = user.firstname;
                        userFromDb.lastname = user.lastname;
                        userFromDb.name = user.name;
                        userFromDb.user_id = user.user_id;
                        userFromDb.email = user.email;
                        userFromDb.attributes = user.attributes;
                        userFromDb.deleted = user.deleted;
                        unitOfWork.users.Update(userFromDb, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<users>.Success(user);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<users>.Failure().AddError(ex.Message);
                }
            }
        }
    }

}
