using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models.DTO
{
    public class instruction_statusDTO
    {
        public string instruction_status { get; }
        public int status_old { get; }
        public int status_new { get; }

        public instruction_statusDTO(string _instruction_status)
        {
            instruction_status = _instruction_status;
            if (instruction_status.Equals("Введено --> На исполнении"))
            {
                status_old = 0;
                status_new = 1;
            }
            else if (instruction_status.Equals("На исполнении --> Исполнено"))
            {
                status_old = 1;
                status_new = 2;
            }
            else if (instruction_status.Equals("На исполнении --> Отменено"))
            {
                status_old = 1;
                status_new = 3;
            }
        }

        public instruction_statusDTO(int _status_old, int _status_new)
        {
            if ((_status_old == 0) && (_status_new == 1))
            {
                instruction_status = "Введено --> На исполнении";
            }
            else if ((_status_old == 1) && (_status_new == 2))
            {
                instruction_status = "На исполнении --> Исполнено";
            }
            else if ((_status_old == 1) && (_status_new == 3))
            {
                instruction_status = "На исполнении --> Отменено";
            }
        }
    }
}
