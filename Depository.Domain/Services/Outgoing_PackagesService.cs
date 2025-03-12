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

    public interface IOutgoing_PackagesService
    {
        Task<EntityOperationResult<outgoing_packages>> Create(outgoing_packages outgoing_package, Guid user_id_with_credentials);
        Task<EntityOperationResult<outgoing_packages>> Update(outgoing_packages outgoing_package, Guid user_id_with_credentials);
        Task<EntityOperationResult<outgoing_packages>> Delete(int? id, Guid user_id_with_credentials);
    }
    public class Outgoing_PackagesService : IOutgoing_PackagesService
    {


        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Outgoing_PackagesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }
        public async Task<EntityOperationResult<outgoing_packages>> Create(outgoing_packages outgoing_package, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<outgoing_packages>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<outgoing_packages>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    else
                    {
                        outgoing_package.created_at = DateTime.Now;
                        outgoing_package.updated_at = DateTime.Now;
                        var entity = await unitOfWork.outgoing_packages.InsertAsync(outgoing_package, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<outgoing_packages>.Success(entity);
                    }
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<outgoing_packages>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<outgoing_packages>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<outgoing_packages>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<outgoing_packages>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var outgoing_package = unitOfWork.outgoing_packages.Get(id);
                    if (outgoing_package != null)
                    {
                        outgoing_package.updated_at = DateTime.Now;
                        outgoing_package.deleted = true;
                        unitOfWork.outgoing_packages.Delete(outgoing_package, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<outgoing_packages>.Success(outgoing_package);
                    }
                    else
                        return EntityOperationResult<outgoing_packages>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<outgoing_packages>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<outgoing_packages>> Update(outgoing_packages outgoing_package, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<outgoing_packages>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<outgoing_packages>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    outgoing_package.updated_at = DateTime.Now;
                    unitOfWork.outgoing_packages.Update(outgoing_package, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<outgoing_packages>.Success(outgoing_package);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<outgoing_packages>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
