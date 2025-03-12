using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.DAL.DbContext.Repositories
{

    public interface ITax_TypesRepository:IRepository<tax_types>
    {
        List<tax_types> GetList();
        bool IsExistName(string name);
    }
    public class Tax_TypesRepository:Repository<tax_types>,  ITax_TypesRepository
    {

        public Tax_TypesRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.tax_types;
        }

        public List<tax_types> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public bool IsExistName(string name)
        {
            return Queryable.Any(DbSet, x => x.name.Equals(name));
        }
    }
}
