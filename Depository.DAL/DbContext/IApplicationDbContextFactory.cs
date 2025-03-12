using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.DAL.DbContext
{
    public interface IApplicationDbContextFactory
    {
        ApplicationDbContext Create();
    }
}
