using BookyWeb.Data.Dtos.CompanyDtos;
using BookyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookyWeb.Data.Repositories.CompanyRepository
{
    public interface ICompanyRepository
    {
        Task<ServiceResponse<List<GetCompanyDto>>> AddUpdateCompany(Company company);
        Task<ServiceResponse<IEnumerable<Company>>> GetAllCompanies();
        Task<ServiceResponse<GetCompanyDto>> GetSingleCompany(int? id);
        Task<ServiceResponse<List<GetCompanyDto>>> DeleteCompany(int? id);
    }
}
