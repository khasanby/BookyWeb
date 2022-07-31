using BookyWeb.Data.Dtos.ProductDtos;
using BookyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookyWeb.Data.Repositories.ProductRepository
{
    public interface IProductRepository
    {
        Task<ServiceResponse<List<GetProductDto>>> AddUpdateProduct(Product newProduct);
        Task<ServiceResponse<List<GetProductDto>>> GetAllProducts();
        Task<ServiceResponse<GetProductDto>> GetSingleProduct(int? id);
        Task<ServiceResponse<List<GetProductDto>>> DeleteProduct(int? id);
    }
}
