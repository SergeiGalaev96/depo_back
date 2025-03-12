using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IDirectoryRepository:IRepository<directory>
    {
        List<directory> GetList();
    }
}
