using System.ComponentModel.DataAnnotations;

namespace MilitaryOperationAPI.Domain.ValidationAttributes
{
    public class UserRoleValidation : ValidationAttribute
    {
        //private static readonly string[] _allowedRoles = new[] { "Admin", "Dev", "President", "Vice President", "Secretary of Defense", "Country Man" };
        private static readonly string[] _allowedRoles;

        static UserRoleValidation()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            _allowedRoles = config.GetSection("UserRoles").Get<string[]>() ?? Array.Empty<string>();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string role && _allowedRoles.Contains(role))
            {
                return ValidationResult.Success ?? new ValidationResult("Sucess");
            }

            return new ValidationResult($"Role must be one of the following: {string.Join(",", _allowedRoles)}");
        }

    }
}
