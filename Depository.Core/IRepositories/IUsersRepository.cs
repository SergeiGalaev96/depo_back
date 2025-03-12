using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IUsersRepository:IRepository<users>
    {
        bool IsExistUserId(Guid user_id);
        users GetByUserId(Guid user_id);
        List<users> GetList();
    }
}
