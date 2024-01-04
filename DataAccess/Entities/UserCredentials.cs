using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class UserCredentials
    {
        [Required]
        public int empCode { get; set; }
        [Required]
        public string password { get; set; }
    }
}
