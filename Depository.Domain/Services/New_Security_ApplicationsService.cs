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

    public interface INew_Security_ApplicationsService
    {
        Task<EntityOperationResult<new_security_applications>> Create(new_security_applications new_security_application, Guid user_id_with_credentials);
        Task<EntityOperationResult<new_security_applications>> Update(new_security_applications new_security_application, Guid user_id_with_credentials);
        Task<EntityOperationResult<new_security_applications>> Delete(int? id, Guid user_id_with_credentials);
    }

    public class New_Security_ApplicationsService : INew_Security_ApplicationsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public New_Security_ApplicationsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<new_security_applications>> Create(new_security_applications new_security_application, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<new_security_applications>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<new_security_applications>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    new_security_application.created_at = DateTime.Now;
                    new_security_application.updated_at = DateTime.Now;
                        var entity = await unitOfWork.new_security_applications.InsertAsync(new_security_application, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<new_security_applications>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<new_security_applications>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<new_security_applications>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<new_security_applications>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<new_security_applications>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var new_security_application = unitOfWork.new_security_applications.Get(id);
                    if (new_security_application != null)
                    {
                        new_security_application.updated_at = DateTime.Now;
                        new_security_application.deleted = true;
                        unitOfWork.new_security_applications.Delete(new_security_application, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<new_security_applications>.Success(new_security_application);
                    }
                    else
                        return EntityOperationResult<new_security_applications>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<new_security_applications>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<new_security_applications>> Update(new_security_applications new_security_application, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<new_security_applications>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<new_security_applications>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    new_security_application.updated_at = DateTime.Now;
                    unitOfWork.new_security_applications.Update(new_security_application, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<new_security_applications>.Success(new_security_application);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<new_security_applications>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
