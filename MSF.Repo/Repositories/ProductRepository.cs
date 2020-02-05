using System.Linq;
using Core.Data;
using Microsoft.EntityFrameworkCore;
using MSF.Domain;
using Microsoft.Extensions.Logging;

namespace MSF.Repo
{
	public interface IProductRepository : IRepository<Product, long> { }

	internal class ProductRepository : BaseRepository<Product, long>, IProductRepository
	{
		public ProductRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
		{
		}

		protected override IQueryable<Product> GetEntitySet(bool incluedeDelete = true)
		{			
			return Entity.Include(e => e.Category).Where(p => !incluedeDelete || !p.InActive);
		}
        
	}
}
