﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class partner_types:Entity
    {
        public string name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
