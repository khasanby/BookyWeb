using BookyWeb.Data.Dtos.CoverTypeDtos;
using BookyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookyWeb.Data.Repositories.CoverTypeRepository
{
    public interface ICoverTypeRepository
    {
        Task<ServiceResponse<List<GetCoverTypeDto>>> AddCoverType(CoverType CoverType);
        Task<ServiceResponse<List<GetCoverTypeDto>>> GetAllCoverTypes();
        Task<ServiceResponse<GetCoverTypeDto>> GetSingleCoverType(int? id);
        Task<ServiceResponse<List<GetCoverTypeDto>>> DeleteCoverType(int? id);
        Task<ServiceResponse<GetCoverTypeDto>> EditCoverType(CoverType coverType);
    }
}
