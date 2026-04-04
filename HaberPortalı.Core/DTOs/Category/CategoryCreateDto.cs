using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaberPortal.Core.DTOs.Category
{
    public class CategoryCreateDto
    {
        [Required(ErrorMessage ="Kategori adı gereklidir")]
        [MinLength(2,ErrorMessage ="Kategori adı en az 2 karakter olmalıdır")]
        public string Name {  get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
