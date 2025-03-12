using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IInstruction_Types_User_PermissionsRepository:IRepository<instruction_types_user_permissions>
    {
        List<instruction_types_user_permissions> GetList();
    }
}
