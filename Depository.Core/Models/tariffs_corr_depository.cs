﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class tariffs_corr_depository:Entity
    {
        public int service_type { get; set; }
        public double? tariff_percent { get; set; }
        public double? tariff_min { get; set; }
        public DateTime? date_start { get; set; }
        public DateTime? date_end { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public int corr_depository { get; set; }
        public int? currency { get; set; }
        public int? security { get; set; }
        public bool? vat { get; set; }
        public double sales_tax { get; set; }
    }
}
