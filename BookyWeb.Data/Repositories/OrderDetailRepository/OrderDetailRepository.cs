using BookyWeb.Data.Data;
using BookyWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookyWeb.Data.Repositories.OrderDetailRepository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderDetailRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ServiceResponse<List<OrderDetail>>> AddOrderDetail(OrderDetail orderDetail)
        {
            var response = new ServiceResponse<List<OrderDetail>>();
            await _dbContext.OrderDetails.AddAsync(orderDetail);
            await _dbContext.SaveChangesAsync();
            response.Data = await _dbContext.OrderDetails.ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<List<OrderDetail>>> DeleteOrderDetail(int? id)
        {
            var response = new ServiceResponse<List<OrderDetail>>();
            if(id == null)
            {
                response.Status = false;
                response.Message = "OrderDetail Not Found";
                return response;
            }
            var orderDetail = await _dbContext.OrderDetails.FirstOrDefaultAsync(c => c.Id == id);
            if(orderDetail == null)
            {
                response.Status = false;
                response.Message = "OrderDetail Not Found";
                return response;
            }

            _dbContext.OrderDetails.Remove(orderDetail);
            await _dbContext.SaveChangesAsync();
            response.Data = await _dbContext.OrderDetails.ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<List<OrderDetail>>> GetAllOrderDetails()
        {
            var response = new ServiceResponse<List<OrderDetail>>();
            response.Data = await _dbContext.OrderDetails.ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<OrderDetail>> GetSingleOrderDetail(int? id)
        {
            var response = new ServiceResponse<OrderDetail>();
            if (id == null)
            {
                response.Status = false;
                response.Message = "OrderDetail Not Found";
                return response;
            }

            var orderDetail = await _dbContext.OrderDetails.FirstOrDefaultAsync(c => c.Id == id);
            
            if(orderDetail == null)
            {
                response.Status = false;
                response.Message = "OrderDetail Not Found";
                return response;
            }
            response.Data = orderDetail;
            return response;
        }

        public async Task<ServiceResponse<OrderDetail>> UpdateOrderDetail(OrderDetail orderDetail)
        {
            var response = new ServiceResponse<OrderDetail>();
            var orderDetailFromDb = await _dbContext.OrderDetails.FirstOrDefaultAsync(c => c.Id == orderDetail.Id);
            if (orderDetailFromDb != null)
            {
                _dbContext.OrderDetails.Update(orderDetail);
            }
            await _dbContext.SaveChangesAsync();
            response.Data = orderDetail;
            return response;
        }
    }
}
