using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UygAPI.Interfaces;
using UygAPI.Models;

namespace UygAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LikesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public LikesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("news/{newsId}")]
        public async Task<IActionResult> LikeNews(int newsId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var existingLike = await _unitOfWork.Likes.FirstOrDefaultAsync(l => l.NewsId == newsId && l.UserId == userId);
            if (existingLike != null)
                return BadRequest(new { message = "Bu haberi zaten beğendiniz." });

            var news = await _unitOfWork.News.GetByIdAsync(newsId);
            if (news == null)
                return NotFound(new { message = "Haber bulunamadı." });

            var like = new Like
            {
                NewsId = newsId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Likes.AddAsync(like);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Haber beğenildi." });
        }

        [HttpDelete("news/{newsId}")]
        public async Task<IActionResult> UnlikeNews(int newsId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var like = await _unitOfWork.Likes.FirstOrDefaultAsync(l => l.NewsId == newsId && l.UserId == userId);
            if (like == null)
                return BadRequest(new { message = "Bu haberi daha önce beğenmemişsiniz." });

            _unitOfWork.Likes.Remove(like);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Beğeni kaldırıldı." });
        }

        [HttpGet("news/{newsId}/check")]
        public async Task<IActionResult> CheckLike(int newsId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasLiked = await _unitOfWork.Likes.AnyAsync(l => l.NewsId == newsId && l.UserId == userId);

            return Ok(new { hasLiked = hasLiked });
        }

        [HttpGet("user/my")]
        public async Task<IActionResult> GetMyLikes()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var likes = await _unitOfWork.Likes.FindAsync(l => l.UserId == userId);

            var likedNews = likes.Select(l => new
            {
                l.NewsId,
                Title = l.News?.Title ?? "Başlık yok",
                ImageUrl = l.News?.ImageUrl,
                l.CreatedAt
            });

            return Ok(likedNews);
        }
    }
}