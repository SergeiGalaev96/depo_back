using Depository.Core.Models;
using Depository.Core;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{


    public interface IAccountingEntryStopTypesService
    {
        Task<EntityOperationResult<accounting_entry_stop_types>> Create(accounting_entry_stop_types accounting_entry_stop_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<accounting_entry_stop_types>> Update(accounting_entry_stop_types accounting_entry_stop_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<accounting_entry_stop_types>> Delete(int? id, Guid user_id_with_credentials);

    }

    public class AccountingEntryStopTypesService : IAccountingEntryStopTypesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public AccountingEntryStopTypesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<accounting_entry_stop_types>> Create (accounting_entry_stop_types accounting_entry_stop_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<accounting_entry_stop_types>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {

                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<accounting_entry_stop_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    accounting_entry_stop_type.created_at = DateTime.Now;
                    accounting_entry_stop_type.updated_at = DateTime.Now;

                    var entity = await unitOfWork.accounting_entry_stop_types.InsertAsync(accounting_entry_stop_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<accounting_entry_stop_types>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<accounting_entry_stop_types>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<accounting_entry_stop_types>> Update(accounting_entry_stop_types accounting_entry_stop_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<accounting_entry_stop_types>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<accounting_entry_stop_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    accounting_entry_stop_type.updated_at = DateTime.Now;
                    unitOfWork.accounting_entry_stop_types.Update(accounting_entry_stop_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<accounting_entry_stop_types>.Success(accounting_entry_stop_type);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<accounting_entry_stop_types>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<accounting_entry_stop_types>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<accounting_entry_stop_types>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<accounting_entry_stop_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var accounting_entry_stop_type = unitOfWork.accounting_entry_stop_types.Get(id);
                    if (accounting_entry_stop_type != null)
                    {
                        accounting_entry_stop_type.updated_at = DateTime.Now;
                        accounting_entry_stop_type.deleted = true;
                        unitOfWork.accounting_entry_stop_types.Delete(accounting_entry_stop_type, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<accounting_entry_stop_types>.Success(accounting_entry_stop_type);
                    }
                    else
                        return EntityOperationResult<accounting_entry_stop_types>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<accounting_entry_stop_types>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
