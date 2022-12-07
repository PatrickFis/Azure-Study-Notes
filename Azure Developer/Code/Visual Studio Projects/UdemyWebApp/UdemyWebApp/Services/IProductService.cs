using UdemyWebApp.Models;

namespace UdemyWebApp.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProducts();
        Task<bool> IsBeta();
    }
}