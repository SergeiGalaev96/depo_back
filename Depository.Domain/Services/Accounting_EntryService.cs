using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IAccounting_EntryService
    {
        Task<EntityOperationResult<accounting_entry>> CreateAccounting_Entry(accounting_entry accounting_entry, Guid user_id_with_credentials);
        Task<EntityOperationResult<accounting_entry>> UpdateAccounting_Entry(accounting_entry accounting_entry, Guid user_id_with_credentials);
        Task<EntityOperationResult<accounting_entry>> DeleteAccounting_Entry(int? id, Guid user_id_with_credentials);
    }

    public class Accounting_EntryService:IAccounting_EntryService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Accounting_EntryService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<accounting_entry>> CreateAccounting_Entry(accounting_entry accounting_entry, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<accounting_entry>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<accounting_entry>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    accounting_entry.created_at = DateTime.Now;
                    accounting_entry.updated_at = DateTime.Now;
                    var entity = await unitOfWork.accounting_entry.InsertAsync(accounting_entry, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<accounting_entry>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<accounting_entry>.Failure().AddError(ex.Message);
                }
            }
        }

        public async  Task<EntityOperationResult<accounting_entry>> DeleteAccounting_Entry(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<accounting_entry>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<accounting_entry>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var accounting_entry = unitOfWork.accounting_entry.Get(id);
                    if (accounting_entry != null)
                    {
                        accounting_entry.updated_at = DateTime.Now;
                        accounting_entry.deleted = true;
                        unitOfWork.accounting_entry.Delete(accounting_entry, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<accounting_entry>.Success(accounting_entry);
                    }
                    else
                        return EntityOperationResult<accounting_entry>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<accounting_entry>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<accounting_entry>> UpdateAccounting_Entry(accounting_entry accounting_entry, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<accounting_entry>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<accounting_entry>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    accounting_entry.updated_at = DateTime.Now;
                    unitOfWork.accounting_entry.Update(accounting_entry, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<accounting_entry>.Success(accounting_entry);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<accounting_entry>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
