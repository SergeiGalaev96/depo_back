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

    public interface ISecurity_Issue_Form_TypesService
    {
        Task<EntityOperationResult<security_issue_form_types>> Create(security_issue_form_types security_issue_form_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<security_issue_form_types>> Update(security_issue_form_types security_issue_form_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<security_issue_form_types>> Delete(int? id, Guid user_id_with_credentials);

    }
    public class Security_Issue_Form_TypesService: ISecurity_Issue_Form_TypesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Security_Issue_Form_TypesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<security_issue_form_types>> Create(security_issue_form_types security_issue_form_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<security_issue_form_types>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<security_issue_form_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                     
                        security_issue_form_type.created_at = DateTime.Now;
                        security_issue_form_type.updated_at = DateTime.Now;
                        var entity = await unitOfWork.security_issue_form_types.InsertAsync(security_issue_form_type, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<security_issue_form_types>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<security_issue_form_types>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<security_issue_form_types>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<security_issue_form_types>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<security_issue_form_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var security_issue_form_type = unitOfWork.security_issue_form_types.Get(id);
                    if (security_issue_form_type != null)
                    {
                        security_issue_form_type.updated_at = DateTime.Now;
                        security_issue_form_type.deleted = true;
                        unitOfWork.security_issue_form_types.Delete(security_issue_form_type, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<security_issue_form_types>.Success(security_issue_form_type);
                    }
                    else
                        return EntityOperationResult<security_issue_form_types>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<security_issue_form_types>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<security_issue_form_types>> Update(security_issue_form_types security_issue_form_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<security_issue_form_types>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<security_issue_form_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    security_issue_form_type.updated_at = DateTime.Now;
                    unitOfWork.security_issue_form_types.Update(security_issue_form_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<security_issue_form_types>.Success(security_issue_form_type);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<security_issue_form_types>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
