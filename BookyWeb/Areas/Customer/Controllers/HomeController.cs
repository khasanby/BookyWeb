using AutoMapper;
using BookyWeb.Data.Repositories.ProductRepository;
using BookyWeb.Data.Repositories.ShoppingCartRepository;
using BookyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BookyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger,
            IProductRepository productRepository, 
            IMapper mapper,
            IShoppingCartRepository shoppingCartRepository)
        {
            _logger = logger;
            _shoppingCartRepository = shoppingCartRepository;
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            var productResponse = await _productRepository.GetAllProducts();
            List<Product> products = productResponse.Data.Select(c => _mapper.Map<Product>(c)).ToList();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int productId)
        {
            var productResponse = await _productRepository.GetSingleProduct(productId);

            ShoppingCart cartObj = new()
            {
                Count = 1,
                ProductId = productId,
                Product = _mapper.Map<Product>(productResponse.Data)
            };
            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            var cartResponse = await _shoppingCartRepository.GetCartByProductId(shoppingCart.ProductId, claim);
            ShoppingCart cartFromDb = cartResponse.Data;

            if (cartFromDb != null)
            {
                await _shoppingCartRepository.IncrementCount(cartFromDb, cartFromDb.Count);
            }
            else
            {
                await _shoppingCartRepository.AddShoppingCart(shoppingCart);
            }
            
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}