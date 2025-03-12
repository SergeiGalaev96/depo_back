using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models.DTO
{
    public class InstructionWithUserDTO
    {
        public instructions _instruction { get; set; }
        public Guid _user_guid { get; set; }

        public InstructionWithUserDTO(instructions instruction, Guid user_guid)
        {
            _instruction = instruction;
            _user_guid = user_guid;
        }
    }
}
