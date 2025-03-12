using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.DAL
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork MakeUnitOfWork();
    }
}
