using System.Linq;
using RS2.Core;
using Microsoft.EntityFrameworkCore;
using MFS.Domain;
using Microsoft.Extensions.Logging;

namespace MFS.Repo
{
	public interface IProductRepository : IRepositoryBase<Product, long> { }

	internal class ProductRepository : RepositoryBase<Product, long>, IProductRepository
	{
		public ProductRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
		{
		}

		protected override IQueryable<Product> EntitySet(bool excludeDeleted = true)
		{			
			return Entity.Include(e => e.Category).Where(p => !excludeDeleted || p.IsActive.GetValueOrDefault(true));
		}

        
	}
}
