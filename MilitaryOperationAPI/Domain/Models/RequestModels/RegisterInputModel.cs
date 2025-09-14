using MilitaryOperationAPI.Domain.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace MilitaryOperationAPI.Domain.Models.RequestModels
{
    public class RegisterInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [MaxLength(32)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(64)]
        public string LastName { get; set; }

        //[Required]
        //[UserRoleValidation]
        //public string Role {  get; set; }

        [Required]
        [MinLength(5), MaxLength(64)]
        public string Title { get; set; }
    }
}
