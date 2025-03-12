using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ICitiesService
    {
        Task<EntityOperationResult<cities>> CreateCity(cities city, Guid user_id_with_credentials);
        Task<EntityOperationResult<cities>> UpdateCity(cities city, Guid user_id_with_credentials);
        Task<EntityOperationResult<cities>> DeleteCity(int? id, Guid user_id_with_credentials);
    }

    public class CitiesService : ICitiesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public CitiesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<cities>> CreateCity(cities city, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<cities>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<cities>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var IsExist = unitOfWork.cities.IsExistName(city.name);
                    if (IsExist)
                        return EntityOperationResult<cities>
                            .Failure()
                            .AddError($"Объект с таким наименованием уже существует");
                    city.created_at = DateTime.Now;
                    city.updated_at = DateTime.Now;
                    var entity = await unitOfWork.cities.InsertAsync(city, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<cities>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<cities>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<cities>> DeleteCity(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<cities>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<cities>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var city = unitOfWork.cities.Get(id);
                    if (city != null)
                    {
                        city.updated_at = DateTime.Now;
                        city.deleted = true;
                        unitOfWork.cities.Delete(city, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<cities>.Success(city);
                    }
                    else
                        return EntityOperationResult<cities>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<cities>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<cities>> UpdateCity(cities city, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<cities>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<cities>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    city.updated_at = DateTime.Now;
                    unitOfWork.cities.Update(city, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<cities>.Success(city);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<cities>.Failure().AddError(ex.Message);
                }
            }
        }
    }

}
