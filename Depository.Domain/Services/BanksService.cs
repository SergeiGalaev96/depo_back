using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
   public  interface IBanksService
   {
        Task<EntityOperationResult<banks>> CreateBank(banks bank, Guid user_id_with_credentials);
        Task<EntityOperationResult<banks>> UpdateBank(banks bank, Guid user_id_with_credentials);
        Task<EntityOperationResult<banks>> DeleteBank(int? id, Guid user_id_with_credentials);

    }

    public class BanksService:IBanksService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public BanksService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<banks>> CreateBank(banks bank, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<banks>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
               
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<banks>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var isExistBIC = unitOfWork.banks.IsExistBIС(bank.bik);
                    if (isExistBIC) return EntityOperationResult<banks>
                            .Failure()
                            .AddError($"Банк с таким БИК уже существует");
                    bank.created_at = DateTime.Now;
                    bank.updated_at = DateTime.Now;
                    
                    var entity = await unitOfWork.banks.InsertAsync(bank, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<banks>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<banks>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<banks>> UpdateBank(banks bank, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<banks>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<banks>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    bank.updated_at = DateTime.Now;
                    unitOfWork.banks.Update(bank, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<banks>.Success(bank);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<banks>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<banks>> DeleteBank(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<banks>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<banks>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var bank = unitOfWork.banks.Get(id);
                    if (bank!=null)
                    {
                        bank.updated_at = DateTime.Now;
                        bank.deleted = true;
                        unitOfWork.banks.Delete(bank, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<banks>.Success(bank);
                    }
                    else
                        return EntityOperationResult<banks>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<banks>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
