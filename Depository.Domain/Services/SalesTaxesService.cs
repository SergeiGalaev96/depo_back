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
    public interface ISalesTaxesService
    {
        Task<EntityOperationResult<sales_taxes>> Create(sales_taxes sales_tax, Guid user_id_with_credentials);
        Task<EntityOperationResult<sales_taxes>> Update(sales_taxes sales_tax, Guid user_id_with_credentials);
        Task<EntityOperationResult<sales_taxes>> Delete(int? id, Guid user_id_with_credentials);
    }
    public class SalesTaxesService : ISalesTaxesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public SalesTaxesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<sales_taxes>> Create(sales_taxes sales_tax, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<sales_taxes>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<sales_taxes>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    sales_tax.created_at = DateTime.Now;
                    sales_tax.updated_at = DateTime.Now;
                    var entity = await unitOfWork.sales_taxes.InsertAsync(sales_tax, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<sales_taxes>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<sales_taxes>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<sales_taxes>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<sales_taxes>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<sales_taxes>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var sales_tax = unitOfWork.sales_taxes.Get(id);
                    if (sales_tax != null)
                    {
                        sales_tax.updated_at = DateTime.Now;
                        sales_tax.deleted = true;
                        unitOfWork.sales_taxes.Delete(sales_tax, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<sales_taxes>.Success(sales_tax);
                    }
                    else
                        return EntityOperationResult<sales_taxes>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<sales_taxes>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<sales_taxes>> Update(sales_taxes sales_tax, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<sales_taxes>
                        .Failure()
                        .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<sales_taxes>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    sales_tax.updated_at = DateTime.Now;

                    unitOfWork.sales_taxes.Update(sales_tax, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<sales_taxes>.Success(sales_tax);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<sales_taxes>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
