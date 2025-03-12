using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ICoefficient_DepositorsService
    {
        Task<EntityOperationResult<coefficient_depositors>> CreateCoefficient_Depositor(coefficient_depositors coefficient_depositor, Guid user_id_with_credentials);
        Task<EntityOperationResult<coefficient_depositors>> UpdateCoefficient_Depositor(coefficient_depositors coefficient_depositor, Guid user_id_with_credentials);
        Task<EntityOperationResult<coefficient_depositors>> DeleteCoefficient_Depositor(int? id, Guid user_id_with_credentials);
    }
    public class Coefficient_DepositorsService: ICoefficient_DepositorsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Coefficient_DepositorsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<coefficient_depositors>> CreateCoefficient_Depositor(coefficient_depositors coefficient_depositor, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<coefficient_depositors>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<coefficient_depositors>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    coefficient_depositor.created_at = DateTime.Now;
                    coefficient_depositor.updated_at = DateTime.Now;
                    var entity = await unitOfWork.coefficient_depositors.InsertAsync(coefficient_depositor, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<coefficient_depositors>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<coefficient_depositors>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<coefficient_depositors>> DeleteCoefficient_Depositor(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<coefficient_depositors>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<coefficient_depositors>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var coefficient_depositor = unitOfWork.coefficient_depositors.Get(id);
                    if (coefficient_depositor != null)
                    {
                        coefficient_depositor.updated_at = DateTime.Now;
                        coefficient_depositor.deleted = true;
                        unitOfWork.coefficient_depositors.Delete(coefficient_depositor, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<coefficient_depositors>.Success(coefficient_depositor);
                    }
                    else
                        return EntityOperationResult<coefficient_depositors>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<coefficient_depositors>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<coefficient_depositors>> UpdateCoefficient_Depositor(coefficient_depositors coefficient_depositor, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<coefficient_depositors>
                            .Failure()
                            .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<coefficient_depositors>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    coefficient_depositor.updated_at = DateTime.Now;
                    unitOfWork.coefficient_depositors.Update(coefficient_depositor, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<coefficient_depositors>.Success(coefficient_depositor);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<coefficient_depositors>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
