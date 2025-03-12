using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public  interface IRegistrarsService
    {
        Task<EntityOperationResult<registrars>> CreateRegistrar(registrars registrar, Guid user_id_with_credentials);
        Task<EntityOperationResult<registrars>> UpdateRegistrar(registrars registrar, Guid user_id_with_credentials);
        Task<EntityOperationResult<registrars>> DeleteRegistrar(int? id, Guid user_id_with_credentials);
    }

    public class RegistrarsService : IRegistrarsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public RegistrarsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<registrars>> CreateRegistrar(registrars registrar, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<registrars>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<registrars>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    registrar.created_at = DateTime.Now;
                    registrar.updated_at = DateTime.Now;
                    registrar.partners = null;
                    registrar.issuers = null;
                    var entity = await unitOfWork.registrars.InsertAsync(registrar, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<registrars>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<registrars>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<registrars>> DeleteRegistrar(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<registrars>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<registrars>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var registrar = unitOfWork.registrars.Get(id);
                    if (registrar != null)
                    {
                        registrar.updated_at = DateTime.Now;
                        registrar.deleted = true;
                        unitOfWork.registrars.Delete(registrar, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<registrars>.Success(registrar);
                    }
                    else
                        return EntityOperationResult<registrars>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<registrars>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<registrars>> UpdateRegistrar(registrars registrar, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<registrars>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<registrars>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    registrar.updated_at = DateTime.Now;
                    unitOfWork.registrars.Update(registrar, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<registrars>.Success(registrar);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<registrars>.Failure().AddError(ex.ToString());
                }
            }
        }
    }



    }
