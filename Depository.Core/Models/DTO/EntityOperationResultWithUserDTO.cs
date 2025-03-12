using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models.DTO
{
    public class EntityOperationResultWithUserDTO
    {
        public string _entityOperationResult { get; set; }
        public Guid _user_guid { get; set; }

        public EntityOperationResultWithUserDTO(string entityOperationResult, Guid user_guid)
        {
            _entityOperationResult = entityOperationResult;
            _user_guid = user_guid;
        }
    }
}
