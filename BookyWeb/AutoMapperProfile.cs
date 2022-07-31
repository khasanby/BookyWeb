using AutoMapper;
using BookyWeb.Data.Dtos.CategoryDtos;
using BookyWeb.Data.Dtos.CoverTypeDtos;
using BookyWeb.Data.Dtos.ProductDtos;
using BookyWeb.Models;

namespace BookyWeb
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Category, GetCategoryDto>();
            CreateMap<GetCategoryDto, Category>();
            CreateMap<GetCoverTypeDto, CoverType>();
            CreateMap<CoverType, GetCoverTypeDto>();
            CreateMap<Product, GetProductDto>();
            CreateMap<GetProductDto, Product>();
        }
    }
}
