using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UygAPI.DTOs.Comment;
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

        [HttpGet("news/{newsId}")]
        public async Task<IActionResult> GetByNews(int newsId)
        {
            var comments = await _unitOfWork.Comments.FindAsync(c => c.NewsId == newsId && c.IsApproved);

            var commentDtos = comments.OrderByDescending(c => c.CreatedAt).Select(c => new CommentListDto
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UserName = c.User?.UserName ?? "Silinmiş Kullanıcı",
                UserFirstName = c.User?.FirstName ?? "",
                UserLastName = c.User?.LastName ?? ""
            });

            return Ok(commentDtos);
        }

        [HttpGet("user/my")]
        [Authorize]
        public async Task<IActionResult> GetMyComments()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var comments = await _unitOfWork.Comments.FindAsync(c => c.UserId == userId);

            var commentDtos = comments.OrderByDescending(c => c.CreatedAt).Select(c => new CommentDetailDto
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UserId = c.UserId,
                UserName = c.User?.UserName ?? "",
                NewsId = c.NewsId,
                NewsTitle = c.News?.Title ?? ""
            });

            return Ok(commentDtos);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CommentCreateDto commentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var news = await _unitOfWork.News.GetByIdAsync(commentDto.NewsId);
            if (news == null)
                return NotFound(new { message = "Haber bulunamadı." });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var comment = new Comment
            {
                Content = commentDto.Content,
                NewsId = commentDto.NewsId,
                UserId = userId,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Comments.AddAsync(comment);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Yorum eklendi.", commentId = comment.Id });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] CommentUpdateDto commentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var comment = await _unitOfWork.Comments.GetByIdAsync(id);
            if (comment == null)
                return NotFound(new { message = "Yorum bulunamadı." });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var isAdmin = User.IsInRole("Admin");

            if (comment.UserId != userId && !isAdmin)
                return Forbid();

            comment.Content = commentDto.Content;
            _unitOfWork.Comments.Update(comment);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Yorum güncellendi." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(id);
            if (comment == null)
                return NotFound(new { message = "Yorum bulunamadı." });

            _unitOfWork.Comments.Remove(comment);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Yorum silindi." });
        }
    }
}