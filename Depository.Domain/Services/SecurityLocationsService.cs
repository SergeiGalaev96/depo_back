using Depository.Core.Models;
using Depository.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Depository.DAL;

namespace Depository.Domain.Services
{

    public interface ISecurityLocationsService
    {
        Task<EntityOperationResult<security_location>> Create(security_location security_location, Guid user_id_with_credentials);
        Task<EntityOperationResult<security_location>> Update(security_location security_location, Guid user_id_with_credentials);
        Task<EntityOperationResult<security_location>> Delete(int? id, Guid user_id_with_credentials);

    }

    public class SecurityLocationsService : ISecurityLocationsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public SecurityLocationsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<security_location>> Create(security_location security_location, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<security_location>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {

                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<security_location>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    security_location.created_at = DateTime.Now;
                    security_location.updated_at = DateTime.Now;

                    var entity = await unitOfWork.security_location.InsertAsync(security_location, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<security_location>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<security_location>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<security_location>> Update(security_location security_location, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<security_location>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<security_location>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    security_location.updated_at = DateTime.Now;
                    unitOfWork.security_location.Update(security_location, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<security_location>.Success(security_location);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<security_location>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<security_location>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<security_location>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<security_location>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var security_location = unitOfWork.security_location.Get(id);
                    if (security_location != null)
                    {
                        security_location.updated_at = DateTime.Now;
                        security_location.deleted = true;
                        unitOfWork.security_location.Delete(security_location, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<security_location>.Success(security_location);
                    }
                    else
                        return EntityOperationResult<security_location>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<security_location>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
