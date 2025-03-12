using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IPartner_ContactsService
    {
        Task<EntityOperationResult<partner_contacts>> CreatePartner_Contact(partner_contacts partner_contact, Guid user_id_with_credentials);
        Task<EntityOperationResult<partner_contacts>> UpdatePartner_Contact(partner_contacts partner_contact, Guid user_id_with_credentials);
        Task<EntityOperationResult<partner_contacts>> DeletePartner_Contact(int? id, Guid user_id_with_credentials);

    }

    public class Partner_ContactsService: IPartner_ContactsService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public Partner_ContactsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<EntityOperationResult<partner_contacts>> CreatePartner_Contact(partner_contacts partner_contact, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<partner_contacts>
                          .Failure()
                          .AddError($"User ID  is null");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<partner_contacts>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");
                    partner_contact.created_at = DateTime.Now;
                    partner_contact.updated_at = DateTime.Now;
                    var entity = await unitOfWork.partner_contacts.InsertAsync(partner_contact, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<partner_contacts>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<partner_contacts>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<partner_contacts>> DeletePartner_Contact(int? id, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<partner_contacts>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<partner_contacts>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    var partner_contact = unitOfWork.partner_contacts.Get(id);
                    if (partner_contact!=null)
                    {
                        partner_contact.updated_at = DateTime.Now;
                        partner_contact.deleted = true;
                        unitOfWork.partner_contacts.Delete(partner_contact, user_with_credentials.id);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<partner_contacts>.Success(partner_contact);
                    }
                    else
                        return EntityOperationResult<partner_contacts>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<partner_contacts>.Failure().AddError(ex.Message);
                }
            }
        }

        public async  Task<EntityOperationResult<partner_contacts>> UpdatePartner_Contact(partner_contacts partner_contact, Guid user_id_with_credentials)
        {
            if (user_id_with_credentials.Equals(Guid.Empty)) return EntityOperationResult<partner_contacts>
                          .Failure()
                          .AddError($"User ID  is null");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_id_with_credentials);
                    if (user_with_credentials == null)
                        return EntityOperationResult<partner_contacts>
                            .Failure()
                            .AddError($"Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции");

                    partner_contact.updated_at = DateTime.Now;
                    unitOfWork.partner_contacts.Update(partner_contact, user_with_credentials.id);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<partner_contacts>.Success(partner_contact);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<partner_contacts>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
