using MSF.Domain;
using MSF.Repo;
using Core.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSF.Service
{

    public interface IProductService
    {
        Task<List<Product>> GetProducts();

        Task<Product> GetProductById(long Id);

        Task<long> SaveProduct(Product product);

        Task<bool> DeleteProduct(long Id);
    }

    internal class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;
        private readonly IUnitOfWork unitOfWork;

        public ProductService(IProductRepository productRepository,
            IUnitOfWork unitOfWork )
        {
            this.productRepository = productRepository;
            this.unitOfWork = unitOfWork;
        }

        async Task<bool> IProductService.DeleteProduct(long Id)
        {
            await productRepository.SoftDelete(Id);
            int result = await unitOfWork.CommitAsync();
            return result > 0;
        }

        async Task<Product> IProductService.GetProductById(long Id)
        {
            return await productRepository.GetAsync(Id);
        }

        async Task<List<Product>> IProductService.GetProducts()
        {
            return await productRepository.GetAllAsync();
        }

        async Task<long> IProductService.SaveProduct(Product product)
        {
            await productRepository.SaveAsync(product);
            int result = await unitOfWork.CommitAsync();

            if (result > 0) return product.ID;

            return 0;
        }
    }
}
