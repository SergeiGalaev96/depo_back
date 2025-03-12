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
    public interface IIncoming_PackagesService
    {
        Task<EntityOperationResult<incoming_packages>> Create(incoming_packages incoming_package, Guid user_id_with_credentials);
        Task<EntityOperationResult<incoming_packages>> Update(incoming_packages incoming_package, Guid user_id_with_credentials);
        Task<EntityOperationResult<incoming_packages>> Delete(int? id, Guid user_id_with_credentials);
    }
    public class Incoming_PackagesService : IIncoming_PackagesService
    {

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Incoming_PackagesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }
        public async Task<EntityOperationResult<incoming_packages>> Create(incoming_packages incoming_package, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<incoming_packages>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<incoming_packages>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    else
                    {
                        incoming_package.created_at = DateTime.Now;
                        incoming_package.updated_at = DateTime.Now;
                        var entity = await unitOfWork.incoming_packages.InsertAsync(incoming_package, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<incoming_packages>.Success(entity);
                    }
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<incoming_packages>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<incoming_packages>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<incoming_packages>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<incoming_packages>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var incoming_package = unitOfWork.incoming_packages.Get(id);
                    if (incoming_package != null)
                    {
                        incoming_package.updated_at = DateTime.Now;
                        incoming_package.deleted = true;
                        unitOfWork.incoming_packages.Delete(incoming_package, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<incoming_packages>.Success(incoming_package);
                    }
                    else
                        return EntityOperationResult<incoming_packages>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<incoming_packages>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<incoming_packages>> Update(incoming_packages incoming_package, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<incoming_packages>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<incoming_packages>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    incoming_package.updated_at = DateTime.Now;
                    if (!incoming_package.processed.Equals(true) && !String.IsNullOrEmpty(incoming_package.ordds_transfer_result) && !String.IsNullOrEmpty(incoming_package.ord_transfer_result) && !String.IsNullOrEmpty(incoming_package.trd_transfer_result))
                        incoming_package.processed = true;
                    unitOfWork.incoming_packages.Update(incoming_package, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<incoming_packages>.Success(incoming_package);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<incoming_packages>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
