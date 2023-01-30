using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class Registration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
    }
}
