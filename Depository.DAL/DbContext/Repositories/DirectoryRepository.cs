using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public class DirectoryRepository:Repository<directory>, IDirectoryRepository
    {
        public DirectoryRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.directory;
        }

        public List<directory> GetList()
        {
            return Queryable.Where(DbSet, x => !String.IsNullOrEmpty(x.description)).ToList();
        }
    }
}
