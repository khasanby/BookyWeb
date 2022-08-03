using AutoMapper;
using BookyWeb.Data.Dtos.CompanyDtos;
using BookyWeb.Data.Repositories.CompanyRepository;
using BookyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompanyController(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            Company company = new();

            if (id == null || id == 0)
            {
                return View(company);
            }
            else
            {
                ServiceResponse<GetCompanyDto> companyResponse = await _companyRepository.GetSingleCompany(id);
                Company companyFromDb = _mapper.Map<Company>(companyResponse.Data);
                return View(companyFromDb);
            }
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Company company)
        {

            if (ModelState.IsValid)
            {
                await _companyRepository.AddUpdateCompany(company);
                return RedirectToAction("Index");
            }
            return View(company);
        }


        #region API CALLS
        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
            ServiceResponse<IEnumerable<Company>> companyResponse = await _companyRepository.GetAllCompanies();
            var companyList = companyResponse.Data;
            return Json(new { data = companyList });
        }
        //POST
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            var companyResponse = await _companyRepository.GetSingleCompany(id);
            var company = _mapper.Map<Company>(companyResponse.Data);
            if (company == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            await _companyRepository.DeleteCompany(company.Id);
            return Json(new { success = true, message = "Deleted Successful" });
        }
        #endregion
    }
}
