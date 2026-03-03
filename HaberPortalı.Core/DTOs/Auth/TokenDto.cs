using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaberPortal.Core.DTOs.Auth
{
    public class TokenDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; }= string.Empty;
        public string Email {  get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }

}
