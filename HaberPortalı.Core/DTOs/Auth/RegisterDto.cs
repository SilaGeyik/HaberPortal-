using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaberPortal.Core.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Ad Gereklidir")]
        public string FirstName {  get; set; } = string.Empty;

        [Required(ErrorMessage ="Soyad Gereklidir")]
        public string LastName { get; set; }

        [Required(ErrorMessage ="Kullanıcı Adı Gereklidir")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email Gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre Gereklidir")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]

        public string Password { get; set; } = string.Empty;
    }
}
