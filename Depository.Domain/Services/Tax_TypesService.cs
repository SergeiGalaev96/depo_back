using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{

    public interface ITax_TypesService
    {
        Task<EntityOperationResult<tax_types>> Create(tax_types tax_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<tax_types>> Update(tax_types tax_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<tax_types>> Delete(int? id, Guid user_id_with_credentials);
    }
    public class Tax_TypesService : ITax_TypesService
    {

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Tax_TypesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }
        public async Task<EntityOperationResult<tax_types>> Create(tax_types tax_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<tax_types>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<tax_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var IsExist = unitOfWork.tax_types.IsExistName(tax_type.name);
                    if (IsExist)
                        return EntityOperationResult<tax_types>
                            .Failure()
                            .AddError($"Объект с таким наименованием уже существует");
                    tax_type.created_at = DateTime.Now;
                    tax_type.updated_at = DateTime.Now;
                    var entity = await unitOfWork.tax_types.InsertAsync(tax_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<tax_types>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<tax_types>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<tax_types>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<tax_types>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<tax_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var tax_type = unitOfWork.tax_types.Get(id);
                    if (tax_type != null)
                    {
                        tax_type.updated_at = DateTime.Now;
                        tax_type.deleted = true;
                        unitOfWork.tax_types.Delete(tax_type, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<tax_types>.Success(tax_type);
                    }
                    else
                        return EntityOperationResult<tax_types>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<tax_types>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<tax_types>> Update(tax_types tax_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<tax_types>
                            .Failure()
                            .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<tax_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    tax_type.updated_at = DateTime.Now;
                    unitOfWork.tax_types.Update(tax_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<tax_types>.Success(tax_type);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<tax_types>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
