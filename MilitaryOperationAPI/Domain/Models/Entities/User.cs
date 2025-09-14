using System.ComponentModel.DataAnnotations;

namespace MilitaryOperationAPI.Domain.Models.Entities
{
    public class User
    {
        public Guid UserID { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(12)]
        [MaxLength(50)]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName {  get; set; }
        public string Title { get; set; }   

        public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();

        public User(Guid userID, string email, string password, string firstName, string lastName, string title)
        {
            UserID = userID;
            Email = email;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            Title = title;
        }
    }
}
