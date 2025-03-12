using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Imported_Files_GikRepository:Repository<imported_files_gik>, IImported_Files_GikRepository
    {
        public Imported_Files_GikRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.imported_files_gik;
        }

        public List<imported_files_gik> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
