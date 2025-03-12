using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IStock_SecurityService
    {
        Task<EntityOperationResult<stock_security>> Update(stock_security stock_security, Guid user_id_with_credentials);
        Task<EntityOperationResult<stock_security>> Create(stock_security stock_security, Guid user_id_with_credentials);
        Task<EntityOperationResult<stock_security>> DeleteStockSecurity(int? id, Guid user_id_with_credentials);
    }

    public class Stock_SecurityService : IStock_SecurityService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Stock_SecurityService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<stock_security>> Create(stock_security stock_security, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<stock_security>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<stock_security>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    stock_security.created_at = DateTime.Now;
                    stock_security.updated_at = DateTime.Now;
                    var entity = await unitOfWork.stock_security.InsertAsync(stock_security, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<stock_security>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<stock_security>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<stock_security>> Update(stock_security stock_security, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<stock_security>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<stock_security>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    // var stock_security_from_db = unitOfWork.stock_security.Get(stock_security.id);
                    // stock_security.created_at = stock_security_from_db.created_at;
                    stock_security.updated_at = DateTime.Now;
                    unitOfWork.stock_security.Update(stock_security, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<stock_security>.Success(stock_security);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<stock_security>.Failure().AddError(ex.ToString());
                }
            }
        }
        public async Task<EntityOperationResult<stock_security>> DeleteStockSecurity(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<stock_security>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<stock_security>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var stock_security = unitOfWork.stock_security.Get(id);
                    if (stock_security != null)
                    {
                        stock_security.updated_at = DateTime.Now;
                        stock_security.deleted = true;
                        unitOfWork.stock_security.Delete(stock_security, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<stock_security>.Success(stock_security);
                    }
                    else
                        return EntityOperationResult<stock_security>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<stock_security>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
