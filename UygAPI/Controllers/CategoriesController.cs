using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UygAPI.DTOs.Category;
using UygAPI.Interfaces;
using UygAPI.Models;

namespace UygAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var categoryDtos = categories.Select(c => new CategoryListDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                NewsCount = c.News?.Count(n => n.IsPublished) ?? 0
            });

            return Ok(categoryDtos);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var categories = await _unitOfWork.Categories.FindAsync(c => c.IsActive);
            return Ok(categories.Select(c => new { c.Id, c.Name, c.Slug }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Kategori bulunamadı." });

            var news = await _unitOfWork.News.FindAsync(n => n.CategoryId == id && n.IsPublished);

            var categoryDto = new CategoryDetailDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                News = news.Select(n => new CategoryNewsDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Summary = n.Summary,
                    ImageUrl = n.ImageUrl,
                    ViewCount = n.ViewCount,
                    CreatedAt = n.CreatedAt
                }).ToList()
            };

            return Ok(categoryDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _unitOfWork.Categories.FirstOrDefaultAsync(c => c.Name == categoryDto.Name);
            if (existing != null)
                return BadRequest(new { message = "Bu isimde bir kategori zaten var." });

            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                Slug = categoryDto.Name.ToLower().Replace(" ", "-"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Kategori oluşturuldu.", categoryId = category.Id });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Kategori bulunamadı." });

            var existing = await _unitOfWork.Categories.FirstOrDefaultAsync(c => c.Name == categoryDto.Name && c.Id != id);
            if (existing != null)
                return BadRequest(new { message = "Bu isimde bir kategori zaten var." });

            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;
            category.IsActive = categoryDto.IsActive;
            category.Slug = categoryDto.Name.ToLower().Replace(" ", "-");

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Kategori güncellendi." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Kategori bulunamadı." });

            var hasNews = await _unitOfWork.News.AnyAsync(n => n.CategoryId == id);
            if (hasNews)
                return BadRequest(new { message = "Bu kategoriye ait haberler var. Önce haberleri silmelisiniz." });

            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Kategori silindi." });
        }
    }
}