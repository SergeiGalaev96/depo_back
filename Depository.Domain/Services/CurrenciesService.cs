using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface ICurrenciesService
    {
        Task<EntityOperationResult<currencies>> CreateCurrency(currencies currency, Guid user_id_with_credentials);
        Task<EntityOperationResult<currencies>> UpdateCurrency(currencies currency, Guid user_id_with_credentials);
        Task<EntityOperationResult<currencies>> DeleteCurrency(int? id, Guid user_id_with_credentials);
    }

    public class CurrencyService: ICurrenciesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public CurrencyService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<currencies>> CreateCurrency(currencies currency, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<currencies>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<currencies>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    currency.created_at = DateTime.Now;
                    currency.updated_at = DateTime.Now;
                    var entity = await unitOfWork.currencies.InsertAsync(currency, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<currencies>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<currencies>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<currencies>> DeleteCurrency(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<currencies>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<currencies>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var currency = unitOfWork.currencies.Get(id);
                    if (currency != null)
                    {
                        currency.updated_at = DateTime.Now;
                        currency.deleted = true;
                        unitOfWork.currencies.Delete(currency, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<currencies>.Success(currency);
                    }
                    else
                        return EntityOperationResult<currencies>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<currencies>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<currencies>> UpdateCurrency(currencies currency, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<currencies>
                            .Failure()
                            .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<currencies>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    currency.updated_at = DateTime.Now;
                    unitOfWork.currencies.Update(currency, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<currencies>.Success(currency);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<currencies>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}

