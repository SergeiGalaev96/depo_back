using Depository.DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.DAL
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IApplicationDbContextFactory _applicationDbContextFactory;

        public UnitOfWorkFactory(IApplicationDbContextFactory applicationDbContextFactory)
        {
            _applicationDbContextFactory = applicationDbContextFactory;
        }

        public IUnitOfWork MakeUnitOfWork()
        {
            return new UnitOfWork(_applicationDbContextFactory.Create());
        }
    }
}
