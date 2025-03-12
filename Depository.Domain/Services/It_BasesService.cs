using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IIt_BasesService
    {
        Task<EntityOperationResult<it_bases>> Create(it_bases it_base, Guid user_id_with_credentials);
        Task<EntityOperationResult<it_bases>> Update(it_bases it_base, Guid user_id_with_credentials);
        Task<EntityOperationResult<it_bases>> Delete(int? id, Guid user_id_with_credentials);

    }

    public class It_BasesService : IIt_BasesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public It_BasesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<it_bases>> Create(it_bases it_base, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<it_bases>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<it_bases>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var isExistName = unitOfWork.it_bases.IsExistName(it_base.name); //уточнить позже, нужно ли проверять по номеру
                    if (isExistName) return EntityOperationResult<it_bases>
                           .Failure()
                           .AddError($"Объект с таким наименованием уже существует");
                    it_base.created_at = DateTime.Now;
                    it_base.updated_at = DateTime.Now;
                    var entity = await unitOfWork.it_bases.InsertAsync(it_base, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<it_bases>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<it_bases>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async  Task<EntityOperationResult<it_bases>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<it_bases>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<it_bases>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var it_base = unitOfWork.it_bases.Get(id);
                    if (it_base != null)
                    {
                        it_base.updated_at = DateTime.Now;
                        it_base.deleted = true;
                        unitOfWork.it_bases.Delete(it_base, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<it_bases>.Success(it_base);
                    }
                    else
                        return EntityOperationResult<it_bases>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<it_bases>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<it_bases>> Update(it_bases it_base, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<it_bases>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<it_bases>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    it_base.updated_at = DateTime.Now;
                    unitOfWork.it_bases.Update(it_base, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<it_bases>.Success(it_base);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<it_bases>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}

