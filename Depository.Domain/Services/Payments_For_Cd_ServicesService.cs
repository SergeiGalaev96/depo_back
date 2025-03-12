using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IPayments_For_Cd_ServicesService
    {
        Task<EntityOperationResult<payments_for_cd_services>> CreatePayment_For_Cd_Services(payments_for_cd_services payment_for_cd_services, Guid user_id_with_credentials);
        Task<EntityOperationResult<payments_for_cd_services>> UpdatePayment_For_Cd_Services(payments_for_cd_services payment_for_cd_services, Guid user_id_with_credentials);
        Task<EntityOperationResult<payments_for_cd_services>> DeletePayment_For_Cd_Services(int? id, Guid user_id_with_credentials);
    }
    public class Payments_For_Cd_ServicesService: IPayments_For_Cd_ServicesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Payments_For_Cd_ServicesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<payments_for_cd_services>> CreatePayment_For_Cd_Services(payments_for_cd_services payment_for_cd_services, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<payments_for_cd_services>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<payments_for_cd_services>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    payment_for_cd_services.created_at = DateTime.Now;
                    payment_for_cd_services.updated_at = DateTime.Now;
                    var entity = await unitOfWork.payments_for_cd_services.InsertAsync(payment_for_cd_services, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<payments_for_cd_services>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<payments_for_cd_services>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<payments_for_cd_services>> DeletePayment_For_Cd_Services(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<payments_for_cd_services>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<payments_for_cd_services>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var payment_for_cd_services = unitOfWork.payments_for_cd_services.Get(id);
                    if (payment_for_cd_services != null)
                    {
                        payment_for_cd_services.updated_at = DateTime.Now;
                        payment_for_cd_services.deleted = true;
                        unitOfWork.payments_for_cd_services.Delete(payment_for_cd_services, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<payments_for_cd_services>.Success(payment_for_cd_services);
                    }
                    else
                        return EntityOperationResult<payments_for_cd_services>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<payments_for_cd_services>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<payments_for_cd_services>> UpdatePayment_For_Cd_Services(payments_for_cd_services payment_for_cd_services, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<payments_for_cd_services>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<payments_for_cd_services>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    payment_for_cd_services.updated_at = DateTime.Now;
                    unitOfWork.payments_for_cd_services.Update(payment_for_cd_services, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<payments_for_cd_services>.Success(payment_for_cd_services);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<payments_for_cd_services>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
