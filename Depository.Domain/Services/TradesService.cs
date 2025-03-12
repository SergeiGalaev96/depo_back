using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ITradesService
    {
        Task<EntityOperationResult<trades>> CreateTrade(trades trade, Guid user_id_with_credentials);
        Task<EntityOperationResult<trades>> UpdateTrade(trades trade, Guid user_id_with_credentials);
        Task<EntityOperationResult<trades>> DeleteTrade(int? id, Guid user_id_with_credentials);
    }

    public class TradesService : ITradesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public TradesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<trades>> CreateTrade(trades trade, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<trades>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<trades>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    trade.created_at = DateTime.Now;
                    trade.updated_at = DateTime.Now;
                    var entity = await unitOfWork.trades.InsertAsync(trade, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<trades>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<trades>.Failure().AddError(ex.ToString());
                }
            }
        }

        
        public  async Task<EntityOperationResult<trades>> DeleteTrade(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<trades>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<trades>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var trade = unitOfWork.trades.Get(id);
                    if (trade != null)
                    {
                        trade.updated_at = DateTime.Now;
                        trade.deleted = true;
                        unitOfWork.trades.Delete(trade, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<trades>.Success(trade);
                    }
                    else
                        return EntityOperationResult<trades>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<trades>.Failure().AddError(ex.ToString());
                }
            }
        }

      
        public  async Task<EntityOperationResult<trades>> UpdateTrade(trades trade, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<trades>
                         .Failure()
                         .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<trades>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    trade.updated_at = DateTime.Now;
                    unitOfWork.trades.Update(trade, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<trades>.Success(trade);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<trades>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
