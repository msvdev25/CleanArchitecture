using System;
using RS2.Core;

namespace Cart.Domain
{
	public class Product : EntityBase<long>
	{

        public Product()
        {

        }

		public string ProductName { get; set; }

		public decimal Price { get;  set; }

		public int CategoryId { get;  set; }

		public virtual Category Category { get;  set; }
	}
}
