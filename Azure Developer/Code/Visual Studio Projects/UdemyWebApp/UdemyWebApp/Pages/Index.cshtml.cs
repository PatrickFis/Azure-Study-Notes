using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UdemyWebApp.Models;
using UdemyWebApp.Services;

namespace UdemyWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public List<Product> Products;
        private readonly IProductService _productService;
        public bool IsBeta = false;
        public IndexModel(ILogger<IndexModel> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public void OnGet()
        {
            Products = _productService.GetProducts().GetAwaiter().GetResult();
            IsBeta = _productService.IsBeta().Result;
        }
    }
}