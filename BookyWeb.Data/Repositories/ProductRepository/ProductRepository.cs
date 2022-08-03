using AutoMapper;
using BookyWeb.Data.Data;
using BookyWeb.Data.Dtos.ProductDtos;
using BookyWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookyWeb.Data.Repositories.ProductRepository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProductRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetProductDto>>> AddUpdateProduct(Product product)
        {
            var response = new ServiceResponse<List<GetProductDto>>();
            if (product.Id != 0)
            {
                _dbContext.Products.Update(product);
            }
            else
            {
                await _dbContext.Products.AddAsync(product);
            }
            await _dbContext.SaveChangesAsync();
            response.Data = await _dbContext.Products.Select(c => _mapper.Map<GetProductDto>(c)).ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<List<GetProductDto>>> DeleteProduct(int? id)
        {
            var response = new ServiceResponse<List<GetProductDto>>();
            if (id == null)
            {
                response.Status = false;
                response.Message = "Product Not Found";
                return response;
            }
            var product = await _dbContext.Products.FirstOrDefaultAsync(c => c.Id == id);
            if (product == null)
            {
                response.Status = false;
                response.Message = "Product Not Found";
                return response;
            }
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
            response.Data = await _dbContext.Products.Select(c => _mapper.Map<GetProductDto>(c)).ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<List<GetProductDto>>> GetAllProducts()
        {
            var response = new ServiceResponse<List<GetProductDto>>();
            response.Data = await _dbContext.Products
                .Include(c => c.Category)
                .Include(c => c.CoverType)
                .Select(c => _mapper.Map<GetProductDto>(c))
                .ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<GetProductDto>> GetSingleProduct(int? id)
        {
            var response = new ServiceResponse<GetProductDto>();
            if (id == null)
            {
                response.Status = false;
                response.Message = "Product Not Found";
                return response;
            }
            var product = await _dbContext.Products
                .Include(c => c.CoverType)
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (product == null)
            {
                response.Status = false;
                response.Message = "Product Not Found";
                return response;
            }
            response.Data = _mapper.Map<GetProductDto>(product);
            return response;
        }
    }
}
