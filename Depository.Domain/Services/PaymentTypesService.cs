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

    public interface IPaymentTypesService
    {
        Task<EntityOperationResult<payment_types>> Create(payment_types payment_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<payment_types>> Update(payment_types payment_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<payment_types>> Delete(int? id, Guid user_id_with_credentials);
    }
    public class PaymentTypesService : IPaymentTypesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public PaymentTypesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<payment_types>> Create(payment_types payment_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<payment_types>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<payment_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    payment_type.created_at = DateTime.Now;
                    payment_type.updated_at = DateTime.Now;
                    var entity = await unitOfWork.payment_types.InsertAsync(payment_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<payment_types>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<payment_types>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<payment_types>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<payment_types>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<payment_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var payment_type = unitOfWork.payment_types.Get(id);
                    if (payment_type != null)
                    {
                        payment_type.updated_at = DateTime.Now;
                        payment_type.deleted = true;
                        unitOfWork.payment_types.Delete(payment_type, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<payment_types>.Success(payment_type);
                    }
                    else
                        return EntityOperationResult<payment_types>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<payment_types>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<payment_types>> Update(payment_types payment_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<payment_types>
                        .Failure()
                        .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<payment_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    payment_type.updated_at = DateTime.Now;

                    unitOfWork.payment_types.Update(payment_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<payment_types>.Success(payment_type);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<payment_types>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
