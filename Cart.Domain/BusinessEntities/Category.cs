using System;
using RS2.Core;

namespace MFS.Domain
{
    public class Category : EntityBase<int>
    {
        public string CategoryName { get; set; }

        public string ImageURL { get; set; }
    }
}
