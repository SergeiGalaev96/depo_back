using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IInstruction_ReportsService
    {
        Task<EntityOperationResult<instruction_reports>> Create(instruction_reports instruction_report, Guid user_id_with_credentials);
        Task<EntityOperationResult<instruction_reports>> Update(instruction_reports instruction_report, Guid user_id_with_credentials);
        Task<EntityOperationResult<instruction_reports>> Delete(int? id, Guid user_id_with_credentials);
    }

    public class Instruction_ReportsService : IInstruction_ReportsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Instruction_ReportsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<instruction_reports>> Create(instruction_reports instruction_report, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_reports>
                         .Failure()
                         .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_reports>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");


                    instruction_report.created_at = DateTime.Now;
                    instruction_report.updated_at = DateTime.Now;
                    var entity = await unitOfWork.instruction_reports.InsertAsync(instruction_report, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instruction_reports>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_reports>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<instruction_reports>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_reports>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_reports>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var instruction_report = unitOfWork.instruction_reports.Get(id);
                    if (instruction_report != null)
                    {
                        instruction_report.updated_at = DateTime.Now;
                        instruction_report.deleted = true;
                        unitOfWork.instruction_reports.Delete(instruction_report, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<instruction_reports>.Success(instruction_report);
                    }
                    else
                        return EntityOperationResult<instruction_reports>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_reports>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<instruction_reports>> Update(instruction_reports instruction_report, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_reports>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_reports>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    instruction_report.updated_at = DateTime.Now;
                    unitOfWork.instruction_reports.Update(instruction_report, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instruction_reports>.Success(instruction_report);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_reports>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}
