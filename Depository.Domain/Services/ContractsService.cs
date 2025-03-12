using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IContractsService
    {
        Task<EntityOperationResult<contracts>> CreateContract(contracts contract, Guid user_id_with_credentials);
        Task<EntityOperationResult<contracts>> UpdateContract(contracts contract, Guid user_id_with_credentials);
        Task<EntityOperationResult<contracts>> DeleteContract(int? id, Guid user_id_with_credentials);
    }

    public class ContractService:IContractsService
    {

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public ContractService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<contracts>> CreateContract(contracts contract, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<contracts>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<contracts>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    contract.created_at = DateTime.Now;
                    contract.updated_at = DateTime.Now;
                    var entity = await unitOfWork.contracts.InsertAsync(contract, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<contracts>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<contracts>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<contracts>> DeleteContract(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<contracts>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<contracts>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var contract = unitOfWork.contracts.Get(id);
                    if (contract != null)
                    {
                        contract.updated_at = DateTime.Now;
                        contract.deleted = true;
                        unitOfWork.contracts.Delete(contract, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<contracts>.Success(contract);
                    }
                    else
                        return EntityOperationResult<contracts>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<contracts>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<contracts>> UpdateContract(contracts contract, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<contracts>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<contracts>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    contract.updated_at = DateTime.Now;
                    unitOfWork.contracts.Update(contract, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<contracts>.Success(contract);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<contracts>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
