using BookyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BookyWeb.Data.Repositories.ShoppingCartRepository
{
    public interface IShoppingCartRepository
    {
        Task<ServiceResponse<List<ShoppingCart>>> AddShoppingCart(ShoppingCart shoppingCart);
        Task<ServiceResponse<List<ShoppingCart>>> GetAllShoppingCarts(Claim? claim);
        Task<ServiceResponse<ShoppingCart>> GetSingleShoppingCart(int? id);
        Task<ServiceResponse<List<ShoppingCart>>> DeleteShoppingCart(int? id);
        Task<ServiceResponse<ShoppingCart>> UpdateShoppingCart(ShoppingCart shoppingCart);
        Task<ServiceResponse<ShoppingCart>> GetCartByProductId(int? id, Claim claim);
        Task<ServiceResponse<int>> IncrementCount(ShoppingCart shoppingCart, int count);
        Task<ServiceResponse<int>> DecrementCount(ShoppingCart shoppingCart, int count);
        Task<ServiceResponse<List<ShoppingCart>>> GetAllByUserId(string? applicationUserId);
    }
}
