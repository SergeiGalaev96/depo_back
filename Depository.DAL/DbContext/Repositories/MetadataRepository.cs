using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public class MetadataRepository:Repository<metadata>, IMetadataRepository
    {
        public MetadataRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.metadata;
        }

        public metadata GetByDefId(Guid defid)
        {
            return Queryable.Where(DbSet, x => x.defid.Equals(defid)).FirstOrDefault();
        }
    }
}
