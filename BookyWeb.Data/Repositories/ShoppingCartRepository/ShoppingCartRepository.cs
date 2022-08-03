using BookyWeb.Data.Data;
using BookyWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BookyWeb.Data.Repositories.ShoppingCartRepository
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ShoppingCartRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<ServiceResponse<List<ShoppingCart>>> AddShoppingCart(ShoppingCart shoppingCart)
        {
            var response = new ServiceResponse<List<ShoppingCart>>();
            await _dbContext.ShoppingCarts.AddAsync(shoppingCart);
            await _dbContext.SaveChangesAsync();
            response.Data = await _dbContext.ShoppingCarts.ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<List<ShoppingCart>>> DeleteShoppingCart(int? id)
        {
            var response = new ServiceResponse<List<ShoppingCart>>();
            if (id == null)
            {
                response.Status = false;
                response.Message = "ShoppingCart Not Found";
                return response;
            }
            var shoppingCart = await _dbContext.ShoppingCarts.FirstOrDefaultAsync(c => c.Id == id);
            if (shoppingCart == null)
            {
                response.Status = false;
                response.Message = "ShoppingCart Not Found";
                return response;
            }
            _dbContext.ShoppingCarts.Remove(shoppingCart);
            await _dbContext.SaveChangesAsync();
            response.Data = await _dbContext.ShoppingCarts.ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<List<ShoppingCart>>> GetAllShoppingCarts(Claim? claim)
        {
            var response = new ServiceResponse<List<ShoppingCart>>();

            if (claim == null)
            {
                response.Data = await _dbContext.ShoppingCarts.ToListAsync();
            }
            else
            {
                response.Data = _dbContext.ShoppingCarts.
                    Include(c => c.Product)
                    .Where(c => c.ApplicationUserId == claim.Value).ToList();
            }

            return response;
        }

        public async Task<ServiceResponse<ShoppingCart>> GetSingleShoppingCart(int? id)
        {
            var response = new ServiceResponse<ShoppingCart>();
            if (id == null)
            {
                response.Status = false;
                response.Message = "ShoppingCart Not Found";
                return response;
            }
            var shoppingCart = await _dbContext.ShoppingCarts.FirstOrDefaultAsync(c => c.Id == id);
            if (shoppingCart == null)
            {
                response.Status = false;
                response.Message = "ShoppingCart Not Found";
                return response;
            }
            response.Data = shoppingCart;
            return response;
        }

        public async Task<ServiceResponse<ShoppingCart>> GetCartByProductId(int? productId, Claim claim)
        {
            var response = new ServiceResponse<ShoppingCart>();
            if (productId == null)
            {
                response.Status = false;
                response.Message = "ShoppingCart Not Found";
                return response;
            }
            var shoppingCart = await _dbContext.ShoppingCarts
                .FirstOrDefaultAsync(c => c.ProductId == productId && c.ApplicationUserId == claim.Value);
            if (shoppingCart == null)
            {
                response.Status = false;
                response.Message = "ShoppingCart Not Found";
                return response;
            }
            response.Data = shoppingCart;
            return response;
        }

        public async Task<ServiceResponse<int>> IncrementCount(ShoppingCart shoppingCart, int count)
        {
            var response = new ServiceResponse<int>();
            var shoppingCartFromDb = await _dbContext.ShoppingCarts.FirstOrDefaultAsync(c => c.Id == shoppingCart.Id);
            if (shoppingCartFromDb != null)
            {
                shoppingCartFromDb.Count += count;
                _dbContext.Update(shoppingCart);
            }
            response.Data = shoppingCartFromDb.Count;
            return response;
        }

        public async Task<ServiceResponse<int>> DecrementCount(ShoppingCart shoppingCart, int count)
        {
            var response = new ServiceResponse<int>();
            var shoppingCartFromDb = await _dbContext.ShoppingCarts.FirstAsync(c => c.Id == shoppingCart.Id);
            shoppingCartFromDb.Count -= count;
            _dbContext.Update(shoppingCart);
            return response;
        }

        public async Task<ServiceResponse<ShoppingCart>> UpdateShoppingCart(ShoppingCart shoppingCart)
        {
            var response = new ServiceResponse<ShoppingCart>();
            var shoppingCartFromDb = await _dbContext.ShoppingCarts.FirstOrDefaultAsync(c => c.Id == shoppingCart.Id);
            if (shoppingCartFromDb != null)
            {
                _dbContext.ShoppingCarts.Update(shoppingCart);
            }
            await _dbContext.SaveChangesAsync();
            response.Data = shoppingCart;
            return response;
        }

        public async Task<ServiceResponse<List<ShoppingCart>>> GetAllByUserId(string? applicationUserId)
        {
            var response = new ServiceResponse<List<ShoppingCart>>();
            if(applicationUserId != null)
            {
                response.Data = await _dbContext.ShoppingCarts.Where(c => c.ApplicationUserId == applicationUserId).ToListAsync();
            }
            return response;
        }
    }
}
