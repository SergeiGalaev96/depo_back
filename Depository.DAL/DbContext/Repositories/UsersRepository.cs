using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public class UsersRepository:Repository<users>, IUsersRepository
    {
        public UsersRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.users;
        }

        public users GetByUserId(Guid user_id)
        {
            return Queryable.Where(DbSet, x => x.user_id.Equals(user_id)).FirstOrDefault();
        }

        public List<users> GetList()
        {
            return Queryable.Where(DbSet, x => (!x.deleted.Value.Equals(true))).ToList();
        }

        public bool IsExistUserId(Guid user_id)
        {
            return Queryable.Any(DbSet, x => !x.deleted.Value.Equals(true) && x.user_id.Equals(user_id));
        }
    }
}
