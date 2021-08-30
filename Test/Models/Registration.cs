using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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

        //[NotMapped]
        //[Required]
        //[DataType(DataType.Password)]
        //[Compare("Password",
        //    ErrorMessage = "Password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; }

        public bool? IsSigned { get; set; }
    }
}
