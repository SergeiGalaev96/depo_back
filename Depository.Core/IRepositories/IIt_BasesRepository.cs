using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IIt_BasesRepository:IRepository<it_bases>
    {
        bool IsExistName(string name);
        List<it_bases> GetList();
    }
}
