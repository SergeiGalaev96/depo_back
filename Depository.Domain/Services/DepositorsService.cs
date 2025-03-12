using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IDepositorsService
    {
        Task<EntityOperationResult<depositors>> CreateDepositor(depositors depositor, Guid user_id_with_credentials);
        Task<EntityOperationResult<depositors>> UpdateDepositor(depositors depositor, Guid user_id_with_credentials);
        Task<EntityOperationResult<depositors>> DeleteDepositor(int? id, Guid user_id_with_credentials);

    }
    public class DepositorsService:IDepositorsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public DepositorsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<depositors>> CreateDepositor(depositors depositor, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<depositors>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<depositors>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    depositor.created_at = DateTime.Now;
                    depositor.updated_at = DateTime.Now;
                    depositor.partners = null;
                    var entity = await unitOfWork.depositors.InsertAsync(depositor, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<depositors>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<depositors>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<depositors>> DeleteDepositor(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<depositors>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<depositors>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var depositor = unitOfWork.depositors.Get(id);
                    if (depositor!=null)
                    {
                        depositor.updated_at = DateTime.Now;
                        depositor.deleted = true;
                        unitOfWork.depositors.Delete(depositor, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<depositors>.Success(depositor);
                    }
                    else
                        return EntityOperationResult<depositors>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<depositors>.Failure().AddError(ex.Message);
                }
            }
        }

        public async  Task<EntityOperationResult<depositors>> UpdateDepositor(depositors depositor, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<depositors>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<depositors>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    depositor.updated_at = DateTime.Now;
                    depositor.partners = null;
                    unitOfWork.depositors.Update(depositor, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<depositors>.Success(depositor);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<depositors>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
