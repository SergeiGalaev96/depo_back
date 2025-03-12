using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IPartnersService
    {
        Task<EntityOperationResult<partners>> CreatePartner(partners partner, Guid user_id_with_credentials);
        Task<EntityOperationResult<partners>> UpdatePartner(partners partner, Guid user_id_with_credentials);
        Task<EntityOperationResult<partners>> DeletePartner(int? id, Guid user_id_with_credentials);
    }

    public class PartnersService : IPartnersService
    {

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public PartnersService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<partners>> CreatePartner(partners partner, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<partners>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<partners>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    partner.created_at = DateTime.Now;
                    partner.updated_at = DateTime.Now;
                    var entity = await unitOfWork.partners.InsertAsync(partner, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<partners>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<partners>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<partners>> DeletePartner(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<partners>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<partners>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var partner = unitOfWork.partners.Get(id);
                    if (partner!=null)
                    {
                        partner.updated_at = DateTime.Now;
                        partner.deleted = true;
                        unitOfWork.partners.Delete(partner, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<partners>.Success(partner);
                    }
                    else
                        return EntityOperationResult<partners>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<partners>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<partners>> UpdatePartner(partners partner, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<partners>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<partners>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    partner.updated_at = DateTime.Now;
                    unitOfWork.partners.Update(partner, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<partners>.Success(partner);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<partners>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
