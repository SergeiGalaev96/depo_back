﻿using Depository.DAL.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.DAL
{
    public class ApplicationDbContextFactory : IApplicationDbContextFactory
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public ApplicationDbContextFactory(
            DbContextOptions<ApplicationDbContext> options)
        {
            _options = options;
        }

        public ApplicationDbContext Create()
        {
            return new ApplicationDbContext(_options);
        }
    }
}
