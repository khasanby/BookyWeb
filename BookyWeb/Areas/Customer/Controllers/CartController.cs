using BookyWeb.Data.Repositories.ShoppingCartRepository;
using BookyWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IShoppingCartRepository shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }

      //  [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var shoppingCartResponse = await _shoppingCartRepository.GetAllShoppingCarts(claim);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = shoppingCartResponse.Data
            };

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.PriceDiscount);
                ShoppingCartVM.CartTotal += (cart.Count * cart.Product.Price);
            }

            return View(ShoppingCartVM);
        }

        [HttpGet]
        public async Task<IActionResult> Summary()
        {
            return View();
        }

        private double GetPriceBasedOnQuantity(double quantity, double price, double priceDiscount)
        {
            if (quantity <= 2)
            {
                return price;
            }
            else
            {
                return priceDiscount;
            }
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await _shoppingCartRepository.GetSingleShoppingCart(cartId);
            await _shoppingCartRepository.IncrementCount(cart.Data, 1);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _shoppingCartRepository.GetSingleShoppingCart(cartId);
            if (cart.Data.Count <= 1)
            {
                await _shoppingCartRepository.DeleteShoppingCart(cart.Data.Id);
               // var count = _shoppingCartRepository.GetAllByUserId(cart.Data.ApplicationUserId);
               // HttpContext.Session.SetInt32(SD.SessionCart, count);
            }
            else
            {
                await _shoppingCartRepository.DecrementCount(cart.Data, 1);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
