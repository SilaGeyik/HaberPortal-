using HaberPortal.Core.DTOs.Comment;
using HaberPortal.Core.DTOs.Category;
using HaberPortal.Core.DTOs.Tag;
using System;
using System.Collections.Generic;

namespace HaberPortal.Core.DTOs.News
{
    public class NewsDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }

        // Kategori bilgisi
        public CategoryDto Category { get; set; } = new();

        // Yazar bilgisi
        public string AuthorId { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;

        // Tarihler
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public bool IsPublished { get; set; }

        // Çoklu görseller
        public List<NewsImageDto> Images { get; set; } = new();
        public string DefaultImageUrl { get; set; } = string.Empty;

        // İlişkili veriler
        public List<TagDto> Tags { get; set; } = new();
        public List<CommentDto> Comments { get; set; } = new();
    }

    public class NewsImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsDefault { get; set; }
    }
}