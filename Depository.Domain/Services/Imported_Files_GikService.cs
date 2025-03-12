using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IImported_Files_GikService
    {
        Task<EntityOperationResult<imported_files_gik>> CreateImported_File_Gik(imported_files_gik imported_file_gik, Guid user_id_with_credentials);
        Task<EntityOperationResult<imported_files_gik>> UpdateImported_File_Gik(imported_files_gik imported_file_gik, Guid user_id_with_credentials);
        Task<EntityOperationResult<imported_files_gik>> DeleteImported_File_Gik(int? id, Guid user_id_with_credentials);
    }
    public class Imported_Files_GikService : IImported_Files_GikService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Imported_Files_GikService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<imported_files_gik>> CreateImported_File_Gik(imported_files_gik imported_file_gik, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<imported_files_gik>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<imported_files_gik>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    imported_file_gik.created_at = DateTime.Now;
                    imported_file_gik.updated_at = DateTime.Now;
                    var entity = await unitOfWork.imported_files_gik.InsertAsync(imported_file_gik, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<imported_files_gik>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<imported_files_gik>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<imported_files_gik>> DeleteImported_File_Gik(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<imported_files_gik>
                            .Failure()
                            .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<imported_files_gik>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var imported_file_gik = unitOfWork.imported_files_gik.Get(id);
                    if (imported_file_gik != null)
                    {
                        imported_file_gik.updated_at = DateTime.Now;
                        imported_file_gik.deleted = true;
                        unitOfWork.imported_files_gik.Delete(imported_file_gik, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<imported_files_gik>.Success(imported_file_gik);
                    }
                    else
                        return EntityOperationResult<imported_files_gik>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<imported_files_gik>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<imported_files_gik>> UpdateImported_File_Gik(imported_files_gik imported_file_gik, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<imported_files_gik>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<imported_files_gik>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    imported_file_gik.updated_at = DateTime.Now;
                    unitOfWork.imported_files_gik.Update(imported_file_gik, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<imported_files_gik>.Success(imported_file_gik);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<imported_files_gik>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}

