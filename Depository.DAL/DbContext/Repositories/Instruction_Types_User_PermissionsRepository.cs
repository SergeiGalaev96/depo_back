using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Instruction_Types_User_PermissionsRepository:Repository<instruction_types_user_permissions>, IInstruction_Types_User_PermissionsRepository
    {
        public Instruction_Types_User_PermissionsRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.instruction_types_user_permissions;
        }

        public List<instruction_types_user_permissions> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
