using AutoMapper;
using BookyWeb.Data.Data;
using BookyWeb.Data.Dtos.CategoryDtos;
using BookyWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookyWeb.Data.Repositories.CategoryRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public CategoryRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetCategoryDto>>> AddCategory(Category newCategory)
        {
            var response = new ServiceResponse<List<GetCategoryDto>>();
            await _dbContext.Categories.AddAsync(newCategory);
            await _dbContext.SaveChangesAsync();
            response.Data = await _dbContext.Categories.Select(c => _mapper.Map<GetCategoryDto>(c)).ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<List<GetCategoryDto>>> DeleteCategory(int? id)
        {
            var response = new ServiceResponse<List<GetCategoryDto>>();
            if(id == null)
            {
                response.Status = false;
                response.Message = "Category Not Found";
                return response;
            }
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if(category == null)
            {
                response.Status = false;
                response.Message = "Category Not Found";
                return response;
            }
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
            response.Data = await _dbContext.Categories.Select(c => _mapper.Map<GetCategoryDto>(c)).ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<GetCategoryDto>> EditCategory(Category category)
        {
            var response = new ServiceResponse<GetCategoryDto>();
            _dbContext.Categories.Update(category);
            _dbContext.SaveChanges();
            response.Data = _mapper.Map<GetCategoryDto>(category);
            return response;
        }

        public async Task<ServiceResponse<List<GetCategoryDto>>> GetAllCategories()
        {
            var response = new ServiceResponse<List<GetCategoryDto>>();
            response.Data = await _dbContext.Categories.Select(c => _mapper.Map<GetCategoryDto>(c)).ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<GetCategoryDto>> GetSingleCategory(int? id)
        {
            var response = new ServiceResponse<GetCategoryDto>();
            if (id == null)
            {
                response.Status = false;
                response.Message = "Category Not Found";
                return response;
            }
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if(category == null)
            {
                response.Status = false;
                response.Message = "Category Not Found";
                return response;
            }
            response.Data = _mapper.Map<GetCategoryDto>(category);
            return response;
        }
    }
}
