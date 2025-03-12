using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IImported_Files_GikRepository:IRepository<imported_files_gik>
    {
        List<imported_files_gik> GetList();
    }
}
