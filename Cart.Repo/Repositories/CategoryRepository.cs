using Cart.Domain;
using RS2.Core;

namespace Cart.Repo
{
	public interface ICategoryRepository : IRepositoryBase<Category, int> { }

	internal class CategoryRepository : RepositoryBase<Category, int>, ICategoryRepository
	{
		public CategoryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
		{

		}
	}
}
