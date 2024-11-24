using System.ComponentModel.DataAnnotations;

namespace NewPayGenixAPI.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }  // Primary Key

        [Required]
        public string RoleName { get; set; }

        //Navigation Property
        public ICollection<User> Users { get; set; } // one to many
    }
}
