using AutoMapper;
using BookyWeb.Data.Data;
using BookyWeb.Data.Dtos.CompanyDtos;
using BookyWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookyWeb.Data.Repositories.CompanyRepository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public CompanyRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _dbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetCompanyDto>>> AddUpdateCompany(Company company)
        {
            var response = new ServiceResponse<List<GetCompanyDto>>();
            if (company.Id != 0)
            {
                _dbContext.Companies.Update(company);
            }
            else
            {
                await _dbContext.Companies.AddAsync(company);
            }
            await _dbContext.SaveChangesAsync();
            response.Data = await _dbContext.Companies.Select(c => _mapper.Map<GetCompanyDto>(c)).ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<List<GetCompanyDto>>> DeleteCompany(int? id)
        {
            var response = new ServiceResponse<List<GetCompanyDto>>();
            if (id == null)
            {
                response.Status = false;
                response.Message = "Company Not Found";
                return response;
            }
            var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Id == id);
            if (company == null)
            {
                response.Status = false;
                response.Message = "Company Not Found";
                return response;
            }
            _dbContext.Companies.Remove(company);
            await _dbContext.SaveChangesAsync();
            response.Data = await _dbContext.Companies.Select(c => _mapper.Map<GetCompanyDto>(c)).ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<IEnumerable<Company>>> GetAllCompanies()
        {
            var response = new ServiceResponse<IEnumerable<Company>>();
            response.Data = await _dbContext.Companies.ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<GetCompanyDto>> GetSingleCompany(int? id)
        {
            var response = new ServiceResponse<GetCompanyDto>();
            if (id == null)
            {
                response.Status = false;
                response.Message = "Company Not Found";
                return response;
            }
            var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Id == id);
            if (company == null)
            {
                response.Status = false;
                response.Message = "Company Not Found";
                return response;
            }
            response.Data = _mapper.Map<GetCompanyDto>(company);
            return response;
        }

    }
}
