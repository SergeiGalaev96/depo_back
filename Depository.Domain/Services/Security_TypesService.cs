using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ISecurity_TypesService
    {
        Task<EntityOperationResult<security_types>> CreateSecurity_Type(security_types security_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<security_types>> UpdateSecurity_Type(security_types security_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<security_types>> DeleteSecurity_Type(int? id, Guid user_id_with_credentials);
    }


    public class Security_TypesService:ISecurity_TypesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Security_TypesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<security_types>> CreateSecurity_Type(security_types security_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<security_types>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<security_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    security_type.created_at = DateTime.Now;
                    security_type.updated_at = DateTime.Now;
                    var entity = await unitOfWork.security_types.InsertAsync(security_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<security_types>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<security_types>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<security_types>> DeleteSecurity_Type(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<security_types>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<security_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var security_type = unitOfWork.security_types.Get(id);
                    if (security_type != null)
                    {
                        security_type.updated_at = DateTime.Now;
                        security_type.deleted = true;
                        unitOfWork.security_types.Delete(security_type, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<security_types>.Success(security_type);
                    }
                    else
                        return EntityOperationResult<security_types>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<security_types>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<security_types>> UpdateSecurity_Type(security_types security_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<security_types>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<security_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    security_type.updated_at = DateTime.Now;
                    unitOfWork.security_types.Update(security_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<security_types>.Success(security_type);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<security_types>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
