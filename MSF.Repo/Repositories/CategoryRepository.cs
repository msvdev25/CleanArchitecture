using MSF.Domain;
using MSF.Core;

namespace MSF.Repo
{
	public interface ICategoryRepository : IBaseRepository<Category, int> { }

	internal class CategoryRepository : BaseRepository<Category, int>, ICategoryRepository
	{
		public CategoryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
		{
            
		}
	}
}
