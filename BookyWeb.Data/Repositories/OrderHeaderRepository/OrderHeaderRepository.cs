using BookyWeb.Data.Data;
using BookyWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookyWeb.Data.Repositories.OrderHeaderRepository
{
    public class OrderHeaderRepository : IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderHeaderRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddOrderHeader(OrderHeader orderHeader)
        {
            var response = new ServiceResponse<List<OrderHeader>>();
            await _dbContext.OrderHeaders.AddAsync(orderHeader);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteOrderHeader(int? id)
        {
            if(id == null)
            {
                response.Status = false;
                response.Message = "OrderHeader Not Found";
                return response;
            }

            var orderHeader = await _dbContext.OrderHeaders.FirstOrDefaultAsync(c => c.Id == id);
            
            if(orderHeader == null)
            {
                response.Status = false;
                response.Message = "OrderHeader Not Found";
                return response;
            }

            _dbContext.OrderHeaders.Remove(orderHeader);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<OrderHeader>> GetAllOrderHeaders()
        {
            var list = await _dbContext.OrderHeaders.ToListAsync();
            return list;
        }

        public async Task<OrderHeader> GetSingleOrderHeader(int? id)
        {            
            var orderHeader = await _dbContext.OrderHeaders.FirstOrDefaultAsync(c => c.Id == id);
            return orderHeader;
        }

        public async Task UpdateOrderHeader(OrderHeader orderHeader)
        {
            var orderHeaderFromDb = await _dbContext.OrderHeaders.FirstOrDefaultAsync(c => c.Id == orderHeader.Id);
            if (orderHeaderFromDb != null)
            {
                _dbContext.OrderHeaders.Update(orderHeader);
            }
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = _dbContext.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (paymentStatus != null)
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
            _dbContext.SaveChanges();
        }

        public async Task UpdateSessionPaymentId(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = await _dbContext.OrderHeaders.FirstOrDefaultAsync(c => c.Id == id);
            orderFromDb.SessionId = sessionId;
            orderFromDb.PaymentIntentId = paymentIntentId;
            await _dbContext.SaveChangesAsync();
        }
    }
}
