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

    public interface IExcertsService
    {
        Task<EntityOperationResult<excerts>> Create(excerts excert, Guid user_id_with_credentials);
        Task<EntityOperationResult<excerts>> Update(excerts excert, Guid user_id_with_credentials);
        Task<EntityOperationResult<excerts>> Delete(int? id, Guid user_id_with_credentials);

    }
    public class ExcertsService : IExcertsService
    {

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public ExcertsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }
        public async Task<EntityOperationResult<excerts>> Create(excerts excert, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<excerts>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<excerts>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                        excert.updated_at = DateTime.Now;
                        excert.deleted = false;
                        unitOfWork.excerts.Update(excert, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<excerts>.Success(excert);
                   
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<excerts>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<excerts>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<excerts>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<excerts>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var excert = unitOfWork.excerts.Get(id);
                    if (excert != null)
                    {
                        excert.updated_at = DateTime.Now;
                        excert.deleted = true;
                        unitOfWork.excerts.Delete(excert, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<excerts>.Success(excert);
                    }
                    else
                        return EntityOperationResult<excerts>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<excerts>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<excerts>> Update(excerts excert, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<excerts>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<excerts>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    excert.updated_at = DateTime.Now;
                    unitOfWork.excerts.Update(excert, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<excerts>.Success(excert);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<excerts>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
