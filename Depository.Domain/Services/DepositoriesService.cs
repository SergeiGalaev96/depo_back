using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IDepositoriesService
    {
        Task<EntityOperationResult<depositories>> CreateDepositary(depositories depositary, Guid user_id_with_credentials);
        Task<EntityOperationResult<depositories>> UpdateDepositary(depositories depositary, Guid user_id_with_credentials);
        Task<EntityOperationResult<depositories>> DeleteDepositary(int? id, Guid user_id_with_credentials);
    }


    public class DepositoriesService : IDepositoriesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public DepositoriesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<depositories>> CreateDepositary(depositories depository, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<depositories>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<depositories>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    depository.created_at = DateTime.Now;
                    depository.updated_at = DateTime.Now;
                    depository.partners = null;

                    var entity = await unitOfWork.depositories.InsertAsync(depository, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<depositories>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<depositories>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<depositories>> DeleteDepositary(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<depositories>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<depositories>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var depository = unitOfWork.depositories.Get(id);
                    if (depository != null)
                    {
                        depository.updated_at = DateTime.Now;
                        depository.deleted = true;
                        depository.partners = null;
                        unitOfWork.depositories.Delete(depository, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<depositories>.Success(depository);
                    }
                    else
                        return EntityOperationResult<depositories>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<depositories>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<depositories>> UpdateDepositary(depositories depository, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<depositories>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<depositories>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    depository.updated_at = DateTime.Now;
                    depository.partners = null;
                    unitOfWork.depositories.Update(depository, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<depositories>.Success(depository);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<depositories>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
