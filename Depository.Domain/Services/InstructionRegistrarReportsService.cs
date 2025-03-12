using Depository.Core.Models;
using Depository.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Depository.DAL;

namespace Depository.Domain.Services
{
   


    public interface IInstructionRegistrarReportsService
    {
        Task<EntityOperationResult<instruction_registrar_reports>> Create(instruction_registrar_reports instruction_registrar_report, Guid user_id_with_credentials);
        Task<EntityOperationResult<instruction_registrar_reports>> Update(instruction_registrar_reports instruction_registrar_report, Guid user_id_with_credentials);
        Task<EntityOperationResult<instruction_registrar_reports>> Delete(int? id, Guid user_id_with_credentials);
    }


    public class InstructionRegistrarReportsService : IInstructionRegistrarReportsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public InstructionRegistrarReportsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }
       

       


        public async Task<EntityOperationResult<instruction_registrar_reports>> Create(instruction_registrar_reports instruction_registrar_report, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_registrar_reports>
                           .Failure()
                           .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_registrar_reports>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    instruction_registrar_report.created_at = DateTime.Now;
                    instruction_registrar_report.updated_at = DateTime.Now;
                    var entity = await unitOfWork.instruction_registrar_reports.InsertAsync(instruction_registrar_report, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instruction_registrar_reports>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_registrar_reports>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<instruction_registrar_reports>> Delete(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_registrar_reports>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_registrar_reports>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var instruction_registrar_report = unitOfWork.instruction_registrar_reports.Get(id);
                    if (instruction_registrar_report != null)
                    {
                        instruction_registrar_report.updated_at = DateTime.Now;
                        instruction_registrar_report.deleted = true;
                        unitOfWork.instruction_registrar_reports.Delete(instruction_registrar_report, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<instruction_registrar_reports>.Success(instruction_registrar_report);
                    }
                    else
                        return EntityOperationResult<instruction_registrar_reports>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_registrar_reports>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<instruction_registrar_reports>> Update(instruction_registrar_reports instruction_registrar_report, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<instruction_registrar_reports>
                           .Failure()
                           .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<instruction_registrar_reports>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    instruction_registrar_report.updated_at = DateTime.Now;
                    unitOfWork.instruction_registrar_reports.Update(instruction_registrar_report, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<instruction_registrar_reports>.Success(instruction_registrar_report);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<instruction_registrar_reports>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
