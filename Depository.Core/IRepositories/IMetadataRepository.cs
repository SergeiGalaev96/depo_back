using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Depository.Core.IRepositories
{
    public interface IMetadataRepository:IRepository<metadata>
    {
        metadata GetByDefId(Guid defid);

    }
}
