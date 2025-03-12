using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IIssuersService
    {
        Task<EntityOperationResult<issuers>> CreateIssuer(issuers issuer, Guid user_id_with_credentials);
        Task<EntityOperationResult<issuers>> UpdateIssuer(issuers issuer, Guid user_id_with_credentials);
        Task<EntityOperationResult<issuers>> DeleteIssuer(int? id, Guid user_id_with_credentials);
    }

    public class IssuersService : IIssuersService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public IssuersService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<issuers>> CreateIssuer(issuers issuer, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<issuers>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<issuers>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    issuer.created_at = DateTime.Now;
                    issuer.updated_at = DateTime.Now;
                    issuer.registrars = null;
                    issuer.securities = null;
                    issuer.instructions = null;
                    var entity = await unitOfWork.issuers.InsertAsync(issuer, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<issuers>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<issuers>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<issuers>> DeleteIssuer(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<issuers>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<issuers>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var issuer = unitOfWork.issuers.Get(id);
                    if (issuer!=null)
                    {
                        issuer.updated_at = DateTime.Now;
                        issuer.deleted = true;
                        unitOfWork.issuers.Delete(issuer, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<issuers>.Success(issuer);
                    }
                    else
                        return EntityOperationResult<issuers>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<issuers>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<issuers>> UpdateIssuer(issuers issuer, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<issuers>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<issuers>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    issuer.updated_at = DateTime.Now;
                    issuer.registrars = null;
                    issuer.securities = null;
                    issuer.instructions = null;

                    unitOfWork.issuers.Update(issuer, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<issuers>.Success(issuer);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<issuers>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
