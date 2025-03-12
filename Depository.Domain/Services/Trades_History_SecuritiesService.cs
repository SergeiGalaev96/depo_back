using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ITrades_History_SecuritiesService
    {
        Task<EntityOperationResult<trades_history_securities>> Create(trades_history_securities trades_history_security, Guid user_id_with_credentials);
        Task<EntityOperationResult<trades_history_securities>> Update(trades_history_securities trades_history_security, Guid user_id_with_credentials);
        Task<EntityOperationResult<trades_history_securities>> Delete(int? id, Guid user_id_with_credentials);
    }

    public class Trades_History_SecuritiesService : ITrades_History_SecuritiesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Trades_History_SecuritiesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<trades_history_securities>> Create(trades_history_securities trades_history_security, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<trades_history_securities>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<trades_history_securities>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    trades_history_security.created_at = DateTime.Now;
                    trades_history_security.updated_at = DateTime.Now;
                    var entity = await unitOfWork.trades_history_securities.InsertAsync(trades_history_security, user_with_credentials.id);
                    
                    await unitOfWork.CompleteAsync();
                    unitOfWork.trades_history_securities.Refresh(trades_history_security);
                    return EntityOperationResult<trades_history_securities>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<trades_history_securities>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<trades_history_securities>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<trades_history_securities>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<trades_history_securities>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var trades_history_security = unitOfWork.trades_history_securities.Get(id);
                    if (trades_history_security != null)
                    {
                        trades_history_security.updated_at = DateTime.Now;
                        trades_history_security.deleted = true;
                        unitOfWork.trades_history_securities.Delete(trades_history_security, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<trades_history_securities>.Success(trades_history_security);
                    }
                    else
                        return EntityOperationResult<trades_history_securities>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<trades_history_securities>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<trades_history_securities>> Update(trades_history_securities trades_history_security, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<trades_history_securities>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<trades_history_securities>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    trades_history_security.updated_at = DateTime.Now;
                    unitOfWork.trades_history_securities.Update(trades_history_security, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<trades_history_securities>.Success(trades_history_security);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<trades_history_securities>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
