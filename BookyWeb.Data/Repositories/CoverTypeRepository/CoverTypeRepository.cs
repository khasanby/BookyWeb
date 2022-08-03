using AutoMapper;
using BookyWeb.Data.Data;
using BookyWeb.Data.Dtos.CoverTypeDtos;
using BookyWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookyWeb.Data.Repositories.CoverTypeRepository
{
    public class CoverTypeRepository : ICoverTypeRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public CoverTypeRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetCoverTypeDto>>> AddCoverType(CoverType newCoverType)
        {
            var response = new ServiceResponse<List<GetCoverTypeDto>>();
            await _dbContext.CoverTypes.AddAsync(newCoverType);
            await _dbContext.SaveChangesAsync();
            response.Data = await _dbContext.CoverTypes.Select(c => _mapper.Map<GetCoverTypeDto>(c)).ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<List<GetCoverTypeDto>>> DeleteCoverType(int? id)
        {
            var response = new ServiceResponse<List<GetCoverTypeDto>>();
            if (id == null)
            {
                response.Status = false;
                response.Message = "CoverType Not Found";
                return response;
            }
            var coverType = await _dbContext.CoverTypes.FirstOrDefaultAsync(c => c.Id == id);
            if (coverType == null)
            {
                response.Status = false;
                response.Message = "CoverType Not Found";
                return response;
            }
            _dbContext.CoverTypes.Remove(coverType);
            await _dbContext.SaveChangesAsync();
            response.Data = await _dbContext.CoverTypes.Select(c => _mapper.Map<GetCoverTypeDto>(c)).ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<GetCoverTypeDto>> EditCoverType(CoverType coverType)
        {
            var response = new ServiceResponse<GetCoverTypeDto>();
            var id = coverType.Id;
            _dbContext.CoverTypes.Update(coverType);
            await _dbContext.SaveChangesAsync();
            response.Data =  _mapper.Map<GetCoverTypeDto>(coverType);
            return response;
        }

        public async Task<ServiceResponse<List<GetCoverTypeDto>>> GetAllCoverTypes()
        {
            var response = new ServiceResponse<List<GetCoverTypeDto>>();
            response.Data = await _dbContext.CoverTypes.Select(c => _mapper.Map<GetCoverTypeDto>(c)).ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<GetCoverTypeDto>> GetSingleCoverType(int? id)
        {
            var response = new ServiceResponse<GetCoverTypeDto>();
            if (id == null)
            {
                response.Status = false;
                response.Message = "CoverType Not Found";
                return response;
            }
            var coverType = await _dbContext.CoverTypes.FirstOrDefaultAsync(c => c.Id == id);
            if (coverType == null)
            {
                response.Status = false;
                response.Message = "CoverType Not Found";
                return response;
            }
            response.Data = _mapper.Map<GetCoverTypeDto>(coverType);
            return response;
        }
    }
}
