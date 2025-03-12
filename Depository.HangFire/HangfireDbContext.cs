using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Depository.HangFire
{
    public class HangfireDbContext:DbContext
    {
        public HangfireDbContext(DbContextOptions<HangfireDbContext> options):base(options)
        {

        }

    }
}
