using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.DAL.DbContext.Repositories
{
    public interface IIncoming_PackagesRepository : IRepository<incoming_packages>
    {
        List<incoming_packages> GetList();
        List<incoming_packages> GetRawBySector(int sector_id);
        List<incoming_packages> GetBySector(int sector_id);
        List<incoming_packages> GetRaw();
    }
    public class Incoming_PackagesRepository : Repository<incoming_packages>, IIncoming_PackagesRepository
    {
        public Incoming_PackagesRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.incoming_packages;
        }

        public List<incoming_packages> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public List<incoming_packages> GetBySector(int sector_id)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true) && x.sector_id == sector_id).ToList();
        }

        public List<incoming_packages> GetRawBySector(int sector_id)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true) && x.processed.Equals(false) && x.sector_id==sector_id).ToList();
        }

        public List<incoming_packages> GetRaw()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true) && x.processed.Equals(false)).ToList();
        }
    }
}
