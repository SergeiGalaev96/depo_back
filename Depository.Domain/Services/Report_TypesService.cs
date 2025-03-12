using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IReport_TypesService
    {
        Task<EntityOperationResult<report_types>> CreateReport_Type(report_types report_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<report_types>> UpdateReport_Type(report_types report_type, Guid user_id_with_credentials);
        Task<EntityOperationResult<report_types>> DeleteReport_Type(int? id, Guid user_id_with_credentials);
    }

    public class Report_TypesService : IReport_TypesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Report_TypesService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<report_types>> CreateReport_Type(report_types report_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<report_types>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<report_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    report_type.created_at = DateTime.Now;
                    report_type.updated_at = DateTime.Now;
                    var entity = await unitOfWork.report_types.InsertAsync(report_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<report_types>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<report_types>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<report_types>> DeleteReport_Type(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<report_types>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<report_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var report_type = unitOfWork.report_types.Get(id);
                    if (report_type != null)
                    {
                        report_type.updated_at = DateTime.Now;
                        report_type.deleted = true;
                        unitOfWork.report_types.Delete(report_type, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<report_types>.Success(report_type);
                    }
                    else
                        return EntityOperationResult<report_types>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<report_types>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<report_types>> UpdateReport_Type(report_types report_type, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<report_types>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<report_types>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    report_type.updated_at = DateTime.Now;
                    unitOfWork.report_types.Update(report_type, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<report_types>.Success(report_type);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<report_types>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
