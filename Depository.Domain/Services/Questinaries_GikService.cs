using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IQuestinaries_GikService
    {
        Task<EntityOperationResult<questinaries_gik>> CreateQuestinary_Gik(questinaries_gik questinary_gik, Guid user_id_with_credentials);
        Task<EntityOperationResult<questinaries_gik>> UpdateQuestinary_Gik(questinaries_gik questinary_gik, Guid user_id_with_credentials);
        Task<EntityOperationResult<questinaries_gik>> DeleteQuestinary_Gik(int? id, Guid user_id_with_credentials);
    }

    public class Questinaries_GikService : IQuestinaries_GikService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Questinaries_GikService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<questinaries_gik>> CreateQuestinary_Gik(questinaries_gik questinary_gik, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<questinaries_gik>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<questinaries_gik>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");


                    questinary_gik.created_at = DateTime.Now;
                    questinary_gik.updated_at = DateTime.Now;
                    var entity = await unitOfWork.questinaries_gik.InsertAsync(questinary_gik, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<questinaries_gik>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<questinaries_gik>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<questinaries_gik>> DeleteQuestinary_Gik(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<questinaries_gik>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<questinaries_gik>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    var questinary_gik = unitOfWork.questinaries_gik.Get(id);
                    if (questinary_gik != null)
                    {
                        questinary_gik.updated_at = DateTime.Now;
                        questinary_gik.deleted = true;
                        unitOfWork.questinaries_gik.Delete(questinary_gik, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<questinaries_gik>.Success(questinary_gik);
                    }
                    else
                        return EntityOperationResult<questinaries_gik>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<questinaries_gik>.Failure().AddError(ex.ToString());
                }
            }
        }

        public async Task<EntityOperationResult<questinaries_gik>> UpdateQuestinary_Gik(questinaries_gik questinary_gik, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<questinaries_gik>
                            .Failure()
                            .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<questinaries_gik>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    questinary_gik.updated_at = DateTime.Now;
                    unitOfWork.questinaries_gik.Update(questinary_gik, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<questinaries_gik>.Success(questinary_gik);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<questinaries_gik>.Failure().AddError(ex.ToString());
                }
            }
        }
    }
}

