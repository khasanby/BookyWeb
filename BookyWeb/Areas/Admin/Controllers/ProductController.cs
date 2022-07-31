using AutoMapper;
using BookyWeb.Data.Dtos.CategoryDtos;
using BookyWeb.Data.Dtos.CoverTypeDtos;
using BookyWeb.Data.Dtos.ProductDtos;
using BookyWeb.Data.Repositories.CategoryRepository;
using BookyWeb.Data.Repositories.CoverTypeRepository;
using BookyWeb.Data.Repositories.ProductRepository;
using BookyWeb.Models;
using BookyWeb.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productReposiotry;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICoverTypeRepository _coverTypeRepository;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;

        public ProductController(IProductRepository productRepository, IMapper mapper, IWebHostEnvironment hostEnvironment, ICategoryRepository categoryRepository, ICoverTypeRepository coverTypeRepository)
        {
            _productReposiotry = productRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _coverTypeRepository = coverTypeRepository;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            ServiceResponse<List<GetProductDto>> response = await _productReposiotry.GetAllProducts();
            List<Product> Products = response.Data.Select(c => _mapper.Map<Product>(c)).ToList();
            return View(Products);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            ServiceResponse<List<GetCategoryDto>> categoryResponse = await _categoryRepository.GetAllCategories();
            ServiceResponse<List<GetCoverTypeDto>> covertypeResponse = await _coverTypeRepository.GetAllCoverTypes();

            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = categoryResponse.Data.Select(c => _mapper.Map<Category>(c))
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    }),
                CoverList = covertypeResponse.Data.Select(c => _mapper.Map<CoverType>(c))
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    })
            };

            if (id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                ServiceResponse<GetProductDto> productResponse = await _productReposiotry.GetSingleProduct(productVM.Product.Id);
                productVM.Product = _mapper.Map<Product>(productResponse.Data);
                return View(productVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if(file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    using (var fileStreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    productVM.Product.ImageUrl = @"images\products\" + filename + extension;
                }
                await _productReposiotry.AddUpdateProduct(productVM.Product);
                return RedirectToAction("Index");
            }
            return View(productVM);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productResponse = await _productReposiotry.GetSingleProduct(id);
            var product = _mapper.Map<Product>(productResponse.Data);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productResponse = await _productReposiotry.GetSingleProduct(id);
            if (productResponse.Data == null)
            {
                return NotFound();
            }
            await _productReposiotry.DeleteProduct(id);
            return RedirectToAction("Index");
        }
    }
}
