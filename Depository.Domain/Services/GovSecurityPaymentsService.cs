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


    public interface IGovSecurityPaymentsService
    {
        Task<EntityOperationResult<gov_securities_payments>> Create(gov_securities_payments gov_securities_payment, Guid user_id_with_credentials);
        Task<EntityOperationResult<gov_securities_payments>> Update(gov_securities_payments gov_securities_payment, Guid user_id_with_credentials);
        Task<EntityOperationResult<gov_securities_payments>> Delete(int? id, Guid user_id_with_credentials);
    }

    public class GovSecurityPaymentsService : IGovSecurityPaymentsService
    {

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public GovSecurityPaymentsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<gov_securities_payments>> Create(gov_securities_payments gov_securities_payment, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<gov_securities_payments>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<gov_securities_payments>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    gov_securities_payment.created_at = DateTime.Now;
                    gov_securities_payment.updated_at = DateTime.Now;
                    var entity = await unitOfWork.gov_securities_payments.InsertAsync(gov_securities_payment, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<gov_securities_payments>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<gov_securities_payments>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<gov_securities_payments>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<gov_securities_payments>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<gov_securities_payments>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var gov_securities_payment = unitOfWork.gov_securities_payments.Get(id);
                    if (gov_securities_payment != null)
                    {
                        gov_securities_payment.updated_at = DateTime.Now;
                        gov_securities_payment.deleted = true;
                        unitOfWork.gov_securities_payments.Delete(gov_securities_payment, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<gov_securities_payments>.Success(gov_securities_payment);
                    }
                    else
                        return EntityOperationResult<gov_securities_payments>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<gov_securities_payments>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<gov_securities_payments>> Update(gov_securities_payments gov_securities_payment, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<gov_securities_payments>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<gov_securities_payments>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    gov_securities_payment.updated_at = DateTime.Now;
                    unitOfWork.gov_securities_payments.Update(gov_securities_payment, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<gov_securities_payments>.Success(gov_securities_payment);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<gov_securities_payments>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
