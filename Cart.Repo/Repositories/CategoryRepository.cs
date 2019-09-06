using MFS.Domain;
using RS2.Core;

namespace MFS.Repo
{
	public interface ICategoryRepository : IRepositoryBase<Category, int> { }

	internal class CategoryRepository : RepositoryBase<Category, int>, ICategoryRepository
	{
		public CategoryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
		{

		}
	}
}
