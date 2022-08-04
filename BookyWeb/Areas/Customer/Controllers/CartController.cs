using BookyWeb.Data.Repositories.ApplicationUserRepository;
using BookyWeb.Data.Repositories.OrderDetailRepository;
using BookyWeb.Data.Repositories.OrderHeaderRepository;
using BookyWeb.Data.Repositories.ShoppingCartRepository;
using BookyWeb.Models;
using BookyWeb.Models.ViewModels;
using BookyWeb.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BookyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IShoppingCartRepository shoppingCartRepository, 
            IApplicationUserRepository applicationUserRepository,
            IOrderHeaderRepository orderHeaderRepository,
            IOrderDetailRepository orderDetailRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _applicationUserRepository = applicationUserRepository;
            _orderHeaderRepository = orderHeaderRepository;
            _orderDetailRepository = orderDetailRepository;
        }

      //  [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var shoppingCartResponse = await _shoppingCartRepository.GetAllShoppingCarts(claim);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = shoppingCartResponse.Data,
                OrderHeader = new()
                
            };

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.PriceDiscount);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Count * cart.Product.Price);
            }

            return View(ShoppingCartVM);
        }

        [HttpGet]
        public async Task<IActionResult> Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var cartResponse = await _shoppingCartRepository.GetAllShoppingCarts(claim);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = cartResponse.Data,
                OrderHeader = new()
            };
            var applicationUserResponse = await _applicationUserRepository.GetSingleApplicationUser(claim);
            ShoppingCartVM.OrderHeader.ApplicationUser = applicationUserResponse.Data;

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.Region;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.PriceDiscount);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var cartResponse = await _shoppingCartRepository.GetAllShoppingCarts(claim);
            ShoppingCartVM.ListCart = cartResponse.Data;

            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.PriceDiscount);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            await _orderHeaderRepository.AddOrderHeader(ShoppingCartVM.OrderHeader);

            foreach(var cart in ShoppingCartVM.ListCart)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                await _orderDetailRepository.AddOrderDetail(orderDetail);
            }

            // Stripe Settings
            var domain = "https://localhost:44320/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain+$"customer/cart/OrderConfirmation?id{ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain+"customer/cart/index",
            };

            foreach(var item in ShoppingCartVM.ListCart)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)item.Price*100,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title,
                        },

                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            };

            var service = new SessionService();
            Session session = service.Create(options);
            await _orderHeaderRepository.UpdateSessionPaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            Response.Headers.Add("Location", session.Url);
            
            return new StatusCodeResult(303);
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

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var orderHeaderResponse = await _orderHeaderRepository.GetSingleOrderHeader(id);
            OrderHeader orderHeader = orderHeaderResponse.Data;
            var service = new SessionService();
            Session session = await service.GetAsync(orderHeader.SessionId);
            
            if(session.PaymentStatus.ToLower() == "paid")
            {
                _orderHeaderRepository.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
            }
            List<ShoppingCart> shoppingCarts = await _shoppingCartRepository
                .GetAllShoppingCartsByApplicationUserId(orderHeader.ApplicationUserId);
            await _shoppingCartRepository.RemoveRange(shoppingCarts);
            return View();
        }
    }
}
