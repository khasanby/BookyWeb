using BookyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookyWeb.Data.Repositories.OrderHeaderRepository
{
    public interface IOrderHeaderRepository
    {
        Task AddOrderHeader(OrderHeader orderHeader);
        Task<List<OrderHeader>> GetAllOrderHeaders();
        Task<OrderHeader> GetSingleOrderHeader(int? id);
        Task DeleteOrderHeader(int? id);
        Task UpdateOrderHeader(OrderHeader orderHeader);
        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        Task UpdateSessionPaymentId(int id, string sessionId, string paymentIntentId);

    }
}
