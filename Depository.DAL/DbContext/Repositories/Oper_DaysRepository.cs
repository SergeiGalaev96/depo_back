using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Oper_DaysRepository:Repository<oper_days>, IOper_DaysRepository
    {
        public Oper_DaysRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.oper_days;
        }

        public oper_days GetByDate(DateTime oper_date)
        {
            return Queryable.Where(DbSet, x => x.date.Date.Equals(oper_date.Date)).FirstOrDefault();
        }

        public List<oper_days> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public bool IsDayClosed(DateTime oper_date)
        {
            return Queryable.Any(DbSet, x => x.date.Date.Equals(oper_date.Date) && x.close.Date.Equals(oper_date.Date) && (!oper_date.Date.Equals(DateTime.MinValue)));
        }

        public bool IsDayOpened(DateTime operDate)
        {
            return Queryable.Any(DbSet, x => x.date.Date.Equals(operDate.Date) && x.open.Date.Equals(operDate.Date) && x.close.Date<=DateTime.MinValue.Date);
        }

        public oper_days GetLastClosedDate()
        {
            return Queryable.Where(DbSet, x => x.close.Date > DateTime.MinValue).OrderByDescending(x => x.date).FirstOrDefault();
        }

        public bool IsExist(DateTime oper_date)
        {
            return Queryable.Any(DbSet, x => x.date.Date.Equals(oper_date.Date));
        }
    }
}
