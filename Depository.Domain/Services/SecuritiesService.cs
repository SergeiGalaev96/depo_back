using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ISecuritiesService
    {
        Task<EntityOperationResult<securities>> CreateSecurity(securities security, Guid user_id_with_credentials);
        Task<EntityOperationResult<securities>> UpdateSecurity(securities security, Guid user_id_with_credentials);
        Task<EntityOperationResult<securities>> DeleteSecurity(int? id, Guid user_id_with_credentials);
    }

    public class SecuritiesService : ISecuritiesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public SecuritiesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<securities>> CreateSecurity(securities security, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<securities>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<securities>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    security.created_at = DateTime.Now;
                    security.updated_at = DateTime.Now;
                    security.security_types = null;
                    security.issuers = null;
                    
                    var entity = await unitOfWork.securities.InsertAsync(security, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<securities>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<securities>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<securities>> DeleteSecurity(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<securities>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<securities>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var security = unitOfWork.securities.Get(id);
                    if (security != null)
                    {
                        security.updated_at = DateTime.Now;
                        security.deleted = true;
                        unitOfWork.securities.Delete(security, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<securities>.Success(security);
                    }
                    else
                        return EntityOperationResult<securities>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<securities>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<securities>> UpdateSecurity(securities security, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<securities>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<securities>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    security.updated_at = DateTime.Now;
                    security.security_types = null;
                    security.issuers = null;
                    unitOfWork.securities.Update(security, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<securities>.Success(security);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<securities>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
