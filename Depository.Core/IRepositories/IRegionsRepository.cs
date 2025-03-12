﻿using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IRegionsRepository:IRepository<regions>
    {
        List<regions> GetList();
        bool IsExistName(string name);
    }
}
