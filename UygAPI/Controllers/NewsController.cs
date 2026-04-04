using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UygAPI.DTOs.News;
using UygAPI.Interfaces;
using UygAPI.Models;

namespace UygAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public NewsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var news = await _unitOfWork.News.GetNewsWithCategoryAsync();
            var newsDtos = news.Select(n => new NewsListDto
            {
                Id = n.Id,
                Title = n.Title,
                Summary = n.Summary,
                ImageUrl = n.ImageUrl,
                ViewCount = n.ViewCount,
                CategoryName = n.Category?.Name ?? string.Empty,
                CategoryId = n.CategoryId,
                AuthorName = (n.Author?.FirstName ?? "") + " " + (n.Author?.LastName ?? ""),
                PublishedAt = n.PublishedAt
            });

            return Ok(newsDtos);
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest([FromQuery] int count = 10)
        {
            var news = await _unitOfWork.News.GetLatestNewsAsync(count);
            var newsDtos = news.Select(n => new NewsListDto
            {
                Id = n.Id,
                Title = n.Title,
                Summary = n.Summary,
                ImageUrl = n.ImageUrl,
                ViewCount = n.ViewCount,
                CategoryName = n.Category?.Name ?? string.Empty,
                CategoryId = n.CategoryId,
                AuthorName = (n.Author?.FirstName ?? "") + " " + (n.Author?.LastName ?? ""),
                PublishedAt = n.PublishedAt
            });

            return Ok(newsDtos);
        }

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopular([FromQuery] int count = 5)
        {
            var news = await _unitOfWork.News.GetPopularNewsAsync(count);
            var newsDtos = news.Select(n => new NewsListDto
            {
                Id = n.Id,
                Title = n.Title,
                Summary = n.Summary,
                ImageUrl = n.ImageUrl,
                ViewCount = n.ViewCount,
                CategoryName = n.Category?.Name ?? string.Empty,
                CategoryId = n.CategoryId,
                AuthorName = (n.Author?.FirstName ?? "") + " " + (n.Author?.LastName ?? ""),
                PublishedAt = n.PublishedAt
            });

            return Ok(newsDtos);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var news = await _unitOfWork.News.GetNewsByCategoryAsync(categoryId);
            var newsDtos = news.Select(n => new NewsListDto
            {
                Id = n.Id,
                Title = n.Title,
                Summary = n.Summary,
                ImageUrl = n.ImageUrl,
                ViewCount = n.ViewCount,
                CategoryName = n.Category?.Name ?? string.Empty,
                CategoryId = n.CategoryId,
                AuthorName = (n.Author?.FirstName ?? "") + " " + (n.Author?.LastName ?? ""),
                PublishedAt = n.PublishedAt
            });

            return Ok(newsDtos);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            if (string.IsNullOrEmpty(q))
                return BadRequest("Arama terimi boş olamaz.");

            var news = await _unitOfWork.News.SearchNewsAsync(q);
            var newsDtos = news.Select(n => new NewsListDto
            {
                Id = n.Id,
                Title = n.Title,
                Summary = n.Summary,
                ImageUrl = n.ImageUrl,
                ViewCount = n.ViewCount,
                CategoryName = n.Category?.Name ?? string.Empty,
                CategoryId = n.CategoryId,
                AuthorName = (n.Author?.FirstName ?? "") + " " + (n.Author?.LastName ?? ""),
                PublishedAt = n.PublishedAt
            });

            return Ok(newsDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var news = await _unitOfWork.News.GetNewsWithDetailsAsync(id);
            if (news == null)
                return NotFound(new { message = "Haber bulunamadı." });

            await _unitOfWork.News.IncrementViewCountAsync(id);

            var newsDto = new NewsDetailDto
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                Summary = news.Summary,
                ImageUrl = news.ImageUrl,
                ViewCount = news.ViewCount,
                CategoryName = news.Category?.Name ?? string.Empty,
                CategoryId = news.CategoryId,
                AuthorName = (news.Author?.FirstName ?? "") + " " + (news.Author?.LastName ?? ""),
                CreatedAt = news.CreatedAt,
                PublishedAt = news.PublishedAt,
                Tags = news.NewsTags?.Select(nt => nt.Tag?.Name).Where(t => t != null).ToList() ?? new List<string>(),
                Comments = news.Comments?.Where(c => c.IsApproved).Select(c => new DTOs.Comment.CommentListDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserName = c.User?.UserName ?? string.Empty,
                    UserFirstName = c.User?.FirstName ?? string.Empty,
                    UserLastName = c.User?.LastName ?? string.Empty
                }).ToList() ?? new List<DTOs.Comment.CommentListDto>()
            };

            return Ok(newsDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> Create([FromBody] NewsCreateDto newsDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var news = new News
            {
                Title = newsDto.Title,
                Content = newsDto.Content,
                Summary = newsDto.Summary ?? (newsDto.Content.Length > 300 ? newsDto.Content.Substring(0, 300) + "..." : newsDto.Content),
                ImageUrl = newsDto.ImageUrl,
                Slug = newsDto.Title.ToLower().Replace(" ", "-"),
                CategoryId = newsDto.CategoryId,
                AuthorId = authorId,
                IsPublished = true,
                PublishedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.News.AddAsync(news);
            await _unitOfWork.CompleteAsync();

            if (newsDto.Tags != null && newsDto.Tags.Any())
            {
                foreach (var tagName in newsDto.Tags)
                {
                    var existingTag = await _unitOfWork.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (existingTag == null)
                    {
                        existingTag = new Tag
                        {
                            Name = tagName,
                            Slug = tagName.ToLower().Replace(" ", "-")
                        };
                        await _unitOfWork.Tags.AddAsync(existingTag);
                        await _unitOfWork.CompleteAsync();
                    }

                    var newsTag = new NewsTag
                    {
                        NewsId = news.Id,
                        TagId = existingTag.Id
                    };
                    await _unitOfWork.NewsTags.AddAsync(newsTag);
                }
                await _unitOfWork.CompleteAsync();
            }

            return Ok(new { message = "Haber oluşturuldu.", newsId = news.Id });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> Update(int id, [FromBody] NewsUpdateDto newsDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var news = await _unitOfWork.News.GetByIdAsync(id);
            if (news == null)
                return NotFound(new { message = "Haber bulunamadı." });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var isAdmin = User.IsInRole("Admin");

            if (news.AuthorId != userId && !isAdmin)
                return Forbid();

            news.Title = newsDto.Title;
            news.Content = newsDto.Content;
            news.Summary = newsDto.Summary ?? (newsDto.Content.Length > 300 ? newsDto.Content.Substring(0, 300) + "..." : newsDto.Content);
            news.ImageUrl = newsDto.ImageUrl;
            news.IsPublished = newsDto.IsPublished;
            news.CategoryId = newsDto.CategoryId;
            news.UpdatedAt = DateTime.UtcNow;
            news.Slug = newsDto.Title.ToLower().Replace(" ", "-");

            _unitOfWork.News.Update(news);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Haber güncellendi." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var news = await _unitOfWork.News.GetByIdAsync(id);
            if (news == null)
                return NotFound(new { message = "Haber bulunamadı." });

            _unitOfWork.News.Remove(news);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Haber silindi." });
        }

        [HttpPost("{id}/view")]
        public async Task<IActionResult> IncrementView(int id)
        {
            var result = await _unitOfWork.News.IncrementViewCountAsync(id);
            if (!result)
                return NotFound(new { message = "Haber bulunamadı." });

            return Ok(new { message = "Görüntülenme sayısı arttırıldı." });
        }
    }
}