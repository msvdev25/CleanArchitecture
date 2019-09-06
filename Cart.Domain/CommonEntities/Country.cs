using RS2.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MFS.Domain
{
    public class Country: EntityBase<int>
    {
        public string CountryName { get; set; }
    }
}
