using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaberPortal.Core.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage ="Email gereklidir")]
        [EmailAddress(ErrorMessage ="Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gereklidir")]
        public string Password { get; set; } = string.Empty;
    }
}
