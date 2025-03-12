using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
   public  interface IExchangesService
    {
        Task<EntityOperationResult<exchanges>> CreateExchange(exchanges exchange, Guid user_id_with_credentials);
        Task<EntityOperationResult<exchanges>> UpdateExchange(exchanges exchange, Guid user_id_with_credentials);
        Task<EntityOperationResult<exchanges>> DeleteExchange(int? id, Guid user_id_with_credentials);
    }

    public class ExchangeService : IExchangesService
    {

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public ExchangeService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<exchanges>> CreateExchange(exchanges exchange, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<exchanges>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<exchanges>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    exchange.created_at = DateTime.Now;
                    exchange.updated_at = DateTime.Now;
                    var entity = await unitOfWork.exchanges.InsertAsync(exchange, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<exchanges>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<exchanges>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<exchanges>> DeleteExchange(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<exchanges>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<exchanges>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var exchange = unitOfWork.exchanges.Get(id);
                    if (exchange!=null)
                    {
                        exchange.updated_at = DateTime.Now;
                        exchange.deleted = true;
                        unitOfWork.exchanges.Delete(exchange, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<exchanges>.Success(exchange);
                    }
                    else
                        return EntityOperationResult<exchanges>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<exchanges>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<exchanges>> UpdateExchange(exchanges exchange, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<exchanges>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<exchanges>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    exchange.updated_at = DateTime.Now;
                    unitOfWork.exchanges.Update(exchange, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<exchanges>.Success(exchange);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<exchanges>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
