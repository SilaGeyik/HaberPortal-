using System.ComponentModel.DataAnnotations;

namespace UygAPI.DTOs.Auth
{
    public class RoleAssignDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string RoleName { get; set; } = string.Empty;
    }
}