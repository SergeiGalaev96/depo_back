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
    public interface IVatsService
    {
        Task<EntityOperationResult<vats>> Create(vats vat, Guid user_id_with_credentials);
        Task<EntityOperationResult<vats>> Update(vats vat, Guid user_id_with_credentials);
        Task<EntityOperationResult<vats>> Delete(int? id, Guid user_id_with_credentials);
    }
    public class VatsService : IVatsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public VatsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<vats>> Create(vats vat, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<vats>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<vats>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    vat.created_at = DateTime.Now;
                    vat.updated_at = DateTime.Now;
                    var entity = await unitOfWork.vats.InsertAsync(vat, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<vats>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<vats>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<vats>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<vats>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<vats>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var vat = unitOfWork.vats.Get(id);
                    if (vat != null)
                    {
                        vat.updated_at = DateTime.Now;
                        vat.deleted = true;
                        unitOfWork.vats.Delete(vat, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<vats>.Success(vat);
                    }
                    else
                        return EntityOperationResult<vats>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<vats>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<vats>> Update(vats vat, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<vats>
                        .Failure()
                        .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<vats>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    vat.updated_at = DateTime.Now;

                    unitOfWork.vats.Update(vat, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<vats>.Success(vat);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<vats>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
