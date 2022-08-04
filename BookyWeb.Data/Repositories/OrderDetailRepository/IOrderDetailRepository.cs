using BookyWeb.Data.Dtos.CategoryDtos;
using BookyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookyWeb.Data.Repositories.OrderDetailRepository
{
    public interface IOrderDetailRepository
    {
        Task<ServiceResponse<List<OrderDetail>>> AddOrderDetail(OrderDetail orderDetail);
        Task<ServiceResponse<List<OrderDetail>>> GetAllOrderDetails();
        Task<ServiceResponse<OrderDetail>> GetSingleOrderDetail(int? id);
        Task<ServiceResponse<List<OrderDetail>>> DeleteOrderDetail(int? id);
        Task<ServiceResponse<OrderDetail>> UpdateOrderDetail(OrderDetail orderDetail);
    }
}
