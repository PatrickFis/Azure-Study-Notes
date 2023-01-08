using Microsoft.AspNetCore.Mvc;
using UdemyAuthApi.Models;
using UdemyAuthApi.Services;

namespace UdemyAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly ProductService productService;

        public ProductController()
        {
            productService = new ProductService();
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(productService.GetProducts());
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(string id)
        {
            return Ok(productService.GetProductByIDFromDB(Int32.Parse(id)));
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            int rowsAffected = productService.AddProduct(product);
            if(rowsAffected > 0)
            {
                return Ok("Added");
            }
            else
            {
                return Ok("Error occurred");
            }
        }
    }
}
