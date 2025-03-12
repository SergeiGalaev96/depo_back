using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IPartner_TypesService
    {
        Task<EntityOperationResult<partner_types>> CreatePartnerType(partner_types partner_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<partner_types>> UpdatePartnerType(partner_types partner_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<partner_types>> DeletePartnerType(int? id, Guid user_id_with_credentials);
    }
    public class Partner_TypesService: IPartner_TypesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Partner_TypesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<partner_types>> CreatePartnerType(partner_types partner_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<partner_types>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<partner_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var isExistName = unitOfWork.partner_types.IsExistName(partner_type.name);
                    if (isExistName) return EntityOperationResult<partner_types>
                           .Failure()
                           .AddError($"Объект с таким наименованием уже существует");
                    partner_type.created_at = DateTime.Now;
                    partner_type.updated_at = DateTime.Now;
                    var entity = await unitOfWork.partner_types.InsertAsync(partner_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<partner_types>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<partner_types>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<partner_types>> DeletePartnerType(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<partner_types>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<partner_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var partner_type = unitOfWork.partner_types.Get(id);
                    if (partner_type != null)
                    {
                        partner_type.updated_at = DateTime.Now;
                        partner_type.deleted = true;
                        unitOfWork.partner_types.Delete(partner_type, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<partner_types>.Success(partner_type);
                    }
                    else
                        return EntityOperationResult<partner_types>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<partner_types>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<partner_types>> UpdatePartnerType(partner_types partner_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<partner_types>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<partner_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    partner_type.updated_at = DateTime.Now;
                    unitOfWork.partner_types.Update(partner_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<partner_types>.Success(partner_type);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<partner_types>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
