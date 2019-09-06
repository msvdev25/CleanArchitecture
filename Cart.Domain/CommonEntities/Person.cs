using RS2.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MFS.Domain
{
    public abstract class Person:EntityBase<int>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EMailAddress { get; set; }

        public string ContactNumber { get; set; }

    }
}
