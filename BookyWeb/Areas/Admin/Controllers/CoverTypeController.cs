using AutoMapper;
using BookyWeb.Data.Dtos.CoverTypeDtos;
using BookyWeb.Data.Repositories.CoverTypeRepository;
using BookyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly ICoverTypeRepository _coverTypeRepository;
        private readonly IMapper _mapper;

        public CoverTypeController(ICoverTypeRepository coverTypeRepository, IMapper mapper)
        {
            _coverTypeRepository = coverTypeRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            ServiceResponse<List<GetCoverTypeDto>> response = await _coverTypeRepository.GetAllCoverTypes();
            List<CoverType> coverTypes = response.Data.Select(c => _mapper.Map<CoverType>(c)).ToList();
            return View(coverTypes);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                await _coverTypeRepository.AddCoverType(coverType);
                return RedirectToAction("Index");
            }
            return View(coverType);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coverTypeDto = await _coverTypeRepository.GetSingleCoverType(id);
            var coverType = _mapper.Map<CoverType>(coverTypeDto.Data);
            if (coverType == null)
            {
                return NotFound();
            }
            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                await _coverTypeRepository.EditCoverType(coverType);
                return RedirectToAction("Index");
            }
            return View(coverType);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coverTypeDto = await _coverTypeRepository.GetSingleCoverType(id);
            var coverType = _mapper.Map<CoverType>(coverTypeDto.Data);
            if (coverType == null)
            {
                return NotFound();
            }
            return View(coverType);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coverType = await _coverTypeRepository.GetSingleCoverType(id);
            if (coverType == null)
            {
                return NotFound();
            }
            await _coverTypeRepository.DeleteCoverType(id);
            return RedirectToAction("Index");
        }
    }
}
