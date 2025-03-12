using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Depository.Api.Extentions.Enums
{
    public enum OperationTypes
    {
        OPEN_OPERATION_DAY=1,
        CLOSE_OPERATION_DAY=2,
        IMPORT_STANDART_SECURITY=3,
        EXPORT_STANDART_SECURITY=4,
        IMPORT_GOV_SECURITY_NB_LOCATION = 5,
        EXPORT_GOV_SECURITY_NB_LOCATION = 6,
        IMPORT_CURRENCY=7,
        EXPORT_CURRENCY=8,
        IMPORT_GOV_SECURITY_KSE_LOCATION=9,
        EXPORT_GOV_SECURITY_KSE_LOCATION=10
    }
}
