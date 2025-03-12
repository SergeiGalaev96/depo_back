using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IUser_RolesRepository:IRepository<user_roles>
    {
        List<user_roles> GetList();
    }
}
