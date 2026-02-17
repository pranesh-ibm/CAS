using System.ComponentModel.DataAnnotations;

namespace CAS.Models
{
    public class PatientRegisterModel
    {
        [Required]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }
    }
}
