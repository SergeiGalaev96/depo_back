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
    public interface IOutgoing_PackagesRepository:IRepository<outgoing_packages>
    {
        List<outgoing_packages> GetList();
        bool IsExist(DateTime created_at);
        outgoing_packages GetByDateAndSector(DateTime created_at, int sector_id);
    }
    public class Outgoing_PackagesRepository:Repository<outgoing_packages>, IOutgoing_PackagesRepository
    {
        public Outgoing_PackagesRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.outgoing_packages;
        }

        public outgoing_packages GetByDateAndSector(DateTime created_at, int sector_id)
        {
            return Queryable.Where(DbSet, x => x.created_at.Date.Equals(created_at.Date) && !x.deleted.Value.Equals(true) && x.sector_id==sector_id).FirstOrDefault();
        }

        public List<outgoing_packages> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public bool IsExist(DateTime created_at)
        {
            return Queryable.Any(DbSet, x => x.created_at.Date.Equals(created_at.Date) && !x.deleted.Value.Equals(true));
        }
    }
}
