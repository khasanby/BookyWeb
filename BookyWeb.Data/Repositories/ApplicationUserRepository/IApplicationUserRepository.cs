using BookyWeb.Data.Migrations;
using BookyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookyWeb.Data.Repositories.ApplicationUserRepository
{
    public interface IApplicationUserRepository
    {
        Task<ServiceResponse<List<ApplicationUser>>> AddApplicationUser(ApplicationUser applicationUser);
        Task<ServiceResponse<List<ApplicationUser>>> GetAllApplicationUsers();
        Task<ServiceResponse<ApplicationUser>> GetSingleApplicationUser(int? id);
        Task<ServiceResponse<List<ApplicationUser>>> DeleteApplicationUser(int? id);
        Task<ServiceResponse<ApplicationUser>> UpdateApplicationUser(ApplicationUser applicationUser);
    }
}
