using System.ComponentModel.DataAnnotations;

namespace NewPayGenixAPI.DTO
{
    public class RoleAssignmentDTO
    {
        [Required]
        public int UserId { get; set; } // ID of the user to whom the role is assigned

        [Required]
        public int RoleId { get; set; } // ID of the role being assigned
    }
}
