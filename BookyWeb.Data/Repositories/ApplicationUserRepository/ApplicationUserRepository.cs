using BookyWeb.Data.Data;
using BookyWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BookyWeb.Data.Repositories.ApplicationUserRepository
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ApplicationUserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ServiceResponse<List<ApplicationUser>>> AddApplicationUser(ApplicationUser applicationUser)
        {
            var response = new ServiceResponse<List<ApplicationUser>>();
            await _dbContext.ApplicationUsers.AddAsync(applicationUser);
            await _dbContext.SaveChangesAsync();
            return response;
        }

        public async Task<ServiceResponse<List<ApplicationUser>>> DeleteApplicationUser(int? id)
        {
            var response = new ServiceResponse<List<ApplicationUser>>();
            if (id == null)
            {
                response.Status = false;
                response.Message = "ApplicationUsers Not Found";
                return response;
            }
            var applicationUser = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(c => c.Id == id.ToString());
            if (applicationUser == null)
            {
                response.Status = false;
                response.Message = "ApplicationUsers Not Found";
                return response;
            }
            _dbContext.ApplicationUsers.Remove(applicationUser);
            await _dbContext.SaveChangesAsync();
            return response;
        }

        public async Task<ServiceResponse<List<ApplicationUser>>> GetAllApplicationUsers()
        {
            var response = new ServiceResponse<List<ApplicationUser>>();
            response.Data = await _dbContext.ApplicationUsers.ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<List<ApplicationUser>>> GetAllApplicationUsers(Claim? claim)
        {
            var response = new ServiceResponse<List<ApplicationUser>>();

            if (claim == null)
            {
                response.Data = await _dbContext.ApplicationUsers.ToListAsync();
            }
            else
            {
                response.Data = await _dbContext.ApplicationUsers.Where(c => c.Id == claim.Value).ToListAsync();
            }

            return response;
        }

        public async Task<ServiceResponse<ApplicationUser>> GetSingleApplicationUser(int? id)
        {
            var response = new ServiceResponse<ApplicationUser>();
            if (id == null)
            {
                response.Status = false;
                response.Message = "ApplicationUser Not Found";
                return response;
            }
            var applicationUser = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(c => c.Id == id.ToString());
            if (applicationUser == null)
            {
                response.Status = false;
                response.Message = "ApplicationUser Not Found";
                return response;
            }
            return response;
        }

        public async Task<ServiceResponse<ApplicationUser>> GetSingleApplicationUser(Claim? claim)
        {
            var response = new ServiceResponse<ApplicationUser>();
            response.Data = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(c => c.Id == claim.Value);
            return response;
        }

        public async Task<ServiceResponse<ApplicationUser>> UpdateApplicationUser(ApplicationUser applicationUser)
        {
            var response = new ServiceResponse<ApplicationUser>();
            var applicationUserFromDb = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(c => c.Id == applicationUser.Id);
            if (applicationUserFromDb != null)
            {
                _dbContext.ApplicationUsers.Update(applicationUser);
            }
            await _dbContext.SaveChangesAsync();
            return response;
        }
    }
}
