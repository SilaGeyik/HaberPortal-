using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UygAPI.Interfaces;
using UygAPI.Models;

namespace UygAPI.Controllers 
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> GetAllComments()
        {
            var comments = await _unitOfWork.Comments.GetAllAsync(); 

            var result = comments.Select(c => new {
                Id = c.Id,
                Content = c.Content,
                UserName = c.User?.UserName ?? "Kullanıcı",
                CreatedAt = c.CreatedAt
            });

            return Ok(result);
        }

        [HttpGet("user/my")]
        [Authorize]
        public async Task<IActionResult> GetMyComments()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var comments = await _unitOfWork.Comments.FindAsync(c => c.UserId == userId);

            var result = comments.Select(c => new {
                Id = c.Id,
                Content = c.Content,
                UserName = c.User?.UserName ?? "Kullanıcı",
                CreatedAt = c.CreatedAt
            });

            return Ok(result);
        }

        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> AddComment([FromBody] CommentAddDto request)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { message = "Kullanıcı doğrulanamadı. Lütfen tekrar giriş yapın." });
            }

            var newComment = new Comment
            {
                NewsId = request.NewsId,
                Content = request.Content,
                UserId = userId,
                CreatedAt = DateTime.UtcNow 
            };

            await _unitOfWork.Comments.AddAsync(newComment);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Yorum başarıyla paylaşıldı." });
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(id);
            if (comment == null) return NotFound();

            _unitOfWork.Comments.Remove(comment);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Silindi" });
        }
    }

  
    public class CommentAddDto
    {
        public int NewsId { get; set; }
        public string Content { get; set; }
    }
}