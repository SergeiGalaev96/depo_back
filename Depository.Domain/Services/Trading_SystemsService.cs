using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ITrading_SystemsService
    {
        Task<EntityOperationResult<trading_systems>> CreateTrading_System(trading_systems corr_depository, Guid user_id_with_credentials);
        Task<EntityOperationResult<trading_systems>> UpdateTrading_System(trading_systems corr_depository, Guid user_id_with_credentials);
        Task<EntityOperationResult<trading_systems>> DeleteTrading_System(int? id, Guid user_id_with_credentials);
    }

    public class Trading_SystemsService : ITrading_SystemsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Trading_SystemsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<trading_systems>> CreateTrading_System(trading_systems trading_system, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<trading_systems>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<trading_systems>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    trading_system.created_at = DateTime.Now;
                    trading_system.updated_at = DateTime.Now;
                    var entity = await unitOfWork.trading_systems.InsertAsync(trading_system, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<trading_systems>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<trading_systems>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<trading_systems>> DeleteTrading_System(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<trading_systems>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<trading_systems>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var trading_system = unitOfWork.trading_systems.Get(id);
                    if (trading_system != null)
                    {
                        trading_system.updated_at = DateTime.Now;
                        trading_system.deleted = true;
                        unitOfWork.trading_systems.Delete(trading_system, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<trading_systems>.Success(trading_system);
                    }
                    else
                        return EntityOperationResult<trading_systems>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<trading_systems>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<trading_systems>> UpdateTrading_System(trading_systems trading_system, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<trading_systems>
                            .Failure()
                            .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<trading_systems>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    trading_system.updated_at = DateTime.Now;
                    unitOfWork.trading_systems.Update(trading_system, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<trading_systems>.Success(trading_system);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<trading_systems>.Failure().AddError(ex.Message);
                }
            }
        }
    }

}
