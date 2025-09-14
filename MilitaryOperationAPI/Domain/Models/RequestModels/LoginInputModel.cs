using System.ComponentModel.DataAnnotations;

namespace MilitaryOperationAPI.Domain.Models.RequestModels
{
    public class LoginInputModel
    {
        /*
         * Attributes examples:
         * [Required] | [EmailAddress] | [phone] | [Stringlength(100, MinLength=3)] | [Range(3,100)]"int" | [url]
         */
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
