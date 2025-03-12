using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ICountriesService
    {
        Task<EntityOperationResult<countries>> CreateCountry(countries country, Guid user_id_with_credentials);
        Task<EntityOperationResult<countries>> UpdateCountry(countries country, Guid user_id_with_credentials);
        Task<EntityOperationResult<countries>> DeleteCountry(int? id, Guid user_id_with_credentials);

    }

    
   
    public class CountriesService : ICountriesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public CountriesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }
        public async Task<EntityOperationResult<countries>> CreateCountry(countries country, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<countries>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<countries>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var isExistName = unitOfWork.countries.IsExistName(country.name);

                    if (isExistName) return EntityOperationResult<countries>
                            .Failure()
                            .AddError($"Страна с таким именем уже существует");
                    country.created_at = DateTime.Now;
                    country.updated_at = DateTime.Now;
                    var entity = await unitOfWork.countries.InsertAsync(country, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<countries>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<countries>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<countries>> DeleteCountry(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<countries>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<countries>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var country = unitOfWork.countries.Get(id);
                    if (country!=null)
                    {
                        country.updated_at = DateTime.Now;
                        country.deleted = true;
                        unitOfWork.countries.Delete(country, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<countries>.Success(country);
                    }
                    else
                        return EntityOperationResult<countries>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<countries>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<countries>> UpdateCountry(countries country, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<countries>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<countries>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    country.updated_at = DateTime.Now;
                    unitOfWork.countries.Update(country, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<countries>.Success(country);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<countries>.Failure().AddError(ex.Message);
                }
            }
        }
    }

}
