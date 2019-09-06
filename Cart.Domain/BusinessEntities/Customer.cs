using System;
using System.Collections.Generic;
using System.Text;

namespace MFS.Domain
{
    public class Customer : Person
    {
        public int Age { get; set; }

        public Gender Gender { get; set; }
    }
}
