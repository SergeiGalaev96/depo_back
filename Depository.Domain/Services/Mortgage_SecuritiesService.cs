using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{


    public interface IMortgage_SecuritiesService
    {
        Task<EntityOperationResult<mortgage_securities>> Create(mortgage_securities mortgage_security, Guid user_id_with_credentials);
        Task<EntityOperationResult<mortgage_securities>> Update(mortgage_securities mortgage_security, Guid user_id_with_credentials);
        Task<EntityOperationResult<mortgage_securities>> Delete(int? id, Guid user_id_with_credentials);

    }

    public class Mortgage_SecuritiesService : IMortgage_SecuritiesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Mortgage_SecuritiesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<mortgage_securities>> Create(mortgage_securities mortgage_security, Guid user_id_with_credentials)
        {

            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<mortgage_securities>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<mortgage_securities>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    mortgage_security.created_at = DateTime.Now;
                    mortgage_security.updated_at = DateTime.Now;
                    var entity = await unitOfWork.mortgage_securities.InsertAsync(mortgage_security, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<mortgage_securities>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<mortgage_securities>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<mortgage_securities>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<mortgage_securities>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<mortgage_securities>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var mortgage_security = unitOfWork.mortgage_securities.Get(id);
                    if (mortgage_security != null)
                    {
                        mortgage_security.updated_at = DateTime.Now;
                        mortgage_security.deleted = true;
                        unitOfWork.mortgage_securities.Delete(mortgage_security, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<mortgage_securities>.Success(mortgage_security);
                    }
                    else
                        return EntityOperationResult<mortgage_securities>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<mortgage_securities>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<mortgage_securities>> Update(mortgage_securities mortgage_security, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<mortgage_securities>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<mortgage_securities>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    mortgage_security.updated_at = DateTime.Now;
                    unitOfWork.mortgage_securities.Update(mortgage_security, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<mortgage_securities>.Success(mortgage_security);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<mortgage_securities>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
