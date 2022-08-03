using BookyWeb.Data.Dtos.CategoryDtos;
using BookyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookyWeb.Data.Repositories.CategoryRepository
{
    public interface ICategoryRepository
    {
        Task<ServiceResponse<List<GetCategoryDto>>> AddCategory(Category newCategory);
        Task<ServiceResponse<List<GetCategoryDto>>> GetAllCategories();
        Task<ServiceResponse<GetCategoryDto>> GetSingleCategory(int? id);
        Task<ServiceResponse<List<GetCategoryDto>>> DeleteCategory(int? id);
        Task<ServiceResponse<GetCategoryDto>> UpdateCategory(Category category);
    }
}
