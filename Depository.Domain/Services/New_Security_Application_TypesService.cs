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
    public interface INew_Security_Application_TypesService
    {
        Task<EntityOperationResult<new_security_application_types>> Create(new_security_application_types new_security_application_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<new_security_application_types>> Update(new_security_application_types new_security_application_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<new_security_application_types>> Delete(int? id, Guid user_id_with_credentials);
    }

    public class New_Security_Application_TypesService : INew_Security_Application_TypesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public New_Security_Application_TypesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<new_security_application_types>> Create(new_security_application_types new_security_application_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<new_security_application_types>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<new_security_application_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    new_security_application_type.created_at = DateTime.Now;
                    new_security_application_type.updated_at = DateTime.Now;
                    var entity = await unitOfWork.new_security_application_types.InsertAsync(new_security_application_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<new_security_application_types>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<new_security_application_types>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<new_security_application_types>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<new_security_application_types>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<new_security_application_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var new_security_application_type = unitOfWork.new_security_application_types.Get(id);
                    if (new_security_application_type != null)
                    {
                        new_security_application_type.updated_at = DateTime.Now;
                        new_security_application_type.deleted = true;
                        unitOfWork.new_security_application_types.Delete(new_security_application_type, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<new_security_application_types>.Success(new_security_application_type);
                    }
                    else
                        return EntityOperationResult<new_security_application_types>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<new_security_application_types>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<new_security_application_types>> Update(new_security_application_types new_security_application_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<new_security_application_types>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<new_security_application_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    new_security_application_type.updated_at = DateTime.Now;
                    unitOfWork.new_security_application_types.Update(new_security_application_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<new_security_application_types>.Success(new_security_application_type);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<new_security_application_types>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
