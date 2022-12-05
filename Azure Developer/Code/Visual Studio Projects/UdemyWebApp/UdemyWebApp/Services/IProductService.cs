using UdemyWebApp.Models;

namespace UdemyWebApp.Services
{
    public interface IProductService
    {
        List<Product> GetProducts();
        Task<bool> IsBeta();
    }
}