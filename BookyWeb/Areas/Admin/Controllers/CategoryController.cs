using AutoMapper;
using BookyWeb.Data.Dtos.CategoryDtos;
using BookyWeb.Data.Repositories.CategoryRepository;
using BookyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            ServiceResponse<List<GetCategoryDto>> response = await _categoryRepository.GetAllCategories();
            List<Category> Categories = response.Data.Select(c => _mapper.Map<Category>(c)).ToList();
            return View(Categories);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category newCategory)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.AddCategory(newCategory);
                return RedirectToAction("Index");
            }
            return View(newCategory);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var categoryResponse = await _categoryRepository.GetSingleCategory(id);
            var category = _mapper.Map<Category>(categoryResponse.Data);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category editedCategory)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.EditCategory(editedCategory);
                return RedirectToAction("Index");
            }
            return View(editedCategory);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var categoryResponse = await _categoryRepository.GetSingleCategory(id);
            var category = _mapper.Map<Category>(categoryResponse.Data);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _categoryRepository.GetSingleCategory(id);
            if (category.Data == null)
            {
                return NotFound();
            }
            await _categoryRepository.DeleteCategory(id);
            return RedirectToAction("Index");
        }
    }
}
