﻿
using MSF.Core;

namespace MSF.Domain
{

    
    public class Tax: BaseEntity<int>
    {

        public decimal TaxValue { get; set; }

        public string TaxName { get; set; }
    }
}
