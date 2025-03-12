﻿using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Partner_TypesRepository:Repository<partner_types>, IPartner_TypesRepository
    {
        public Partner_TypesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.partner_types;
        }

        public List<partner_types> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public bool IsExistName(string name)
        {
            return Queryable.Any(DbSet, x => x.name.Equals(name));
        }
    }
}
