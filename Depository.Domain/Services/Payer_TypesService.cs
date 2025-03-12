using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IPayer_TypesService
    {
        Task<EntityOperationResult<payer_types>> CreatePayer_Type(payer_types payer_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<payer_types>> UpdatePayer_Type(payer_types payer_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<payer_types>> DeletePayer_Type(int? id, Guid user_id_with_credentials);
    }

    public class Payer_TypesService : IPayer_TypesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Payer_TypesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<payer_types>> CreatePayer_Type(payer_types payer_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<payer_types>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<payer_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    payer_type.created_at = DateTime.Now;
                    payer_type.updated_at = DateTime.Now;
                    var entity = await unitOfWork.payer_types.InsertAsync(payer_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<payer_types>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<payer_types>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<payer_types>> DeletePayer_Type(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<payer_types>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<payer_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var payer_type = unitOfWork.payer_types.Get(id);
                    if (payer_type != null)
                    {
                        payer_type.updated_at = DateTime.Now;
                        payer_type.deleted = true;
                        unitOfWork.payer_types.Delete(payer_type, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<payer_types>.Success(payer_type);
                    }
                    else
                        return EntityOperationResult<payer_types>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<payer_types>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<payer_types>> UpdatePayer_Type(payer_types payer_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<payer_types>
                            .Failure()
                            .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<payer_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    payer_type.updated_at = DateTime.Now;
                    unitOfWork.payer_types.Update(payer_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<payer_types>.Success(payer_type);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<payer_types>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
