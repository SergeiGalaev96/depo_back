using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.DAL.Repositories
{
    public class User_RolesRepository:Repository<user_roles>, IUser_RolesRepository
    {
        public User_RolesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.user_roles;
        }

        public  List<user_roles> GetList()
        {
            return  Queryable.Where(DbSet, x =>  (! x.deleted.Value.Equals(true))).ToList();
        }
    }

    
}
