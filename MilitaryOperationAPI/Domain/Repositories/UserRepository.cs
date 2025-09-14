using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MilitaryOperationAPI.Data;
using MilitaryOperationAPI.Domain.Models.Entities;
using MilitaryOperationAPI.Domain.Models.RequestModels;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace MilitaryOperationAPI.Domain.Repositories
{
    public class UserRepository
    {
        /*
         * DECLARING class property in .net core
         * 1. access modifier [public private internal(code in same assembly or dll) protected(code in same class or derived class), private protected, protected interal]
         * 2. Member modifier [readonly, sealed(can't be override), abstract (no default implemenation), override, virtual, static]
         */
        private readonly AppDBContext _appDBContext;
        private readonly IConfiguration _configuration;

        public UserRepository(AppDBContext appDBContext, IConfiguration configuration)
        {
            this._appDBContext = appDBContext;
            this._configuration = configuration;
        }

        /*
         * Register a NEW USER
         * #1. Does email already exist we don't allow same email || we also don't allow same ID
         * #2. Check if password meets criteria set out which we don't have any 
         * #3. Does the roles matches allow roles
         * #4. Save Changes()
         */
        public async Task<User> AddNewUser(User newUserRegistrationModel)
        {
            var newAddedUser = await this._appDBContext.Users.AddAsync(newUserRegistrationModel);
            await this._appDBContext.SaveChangesAsync();

            return newAddedUser.Entity;
        }

        /*
         * LoginUser.
         * #1 Check if the user exist with this email, then verify the password against stored hash
         */
        public async Task<bool> Login(string email, string password)
        {
            User user = await this.GetUserByEmail(email);

            if (user == null)
            {
                return false;
            }

            return BC.Verify(password, user.Password);
        }

        /*
         * Get User Details by ID
         */
        public async Task<User> GetUserByID(Guid userID)
        {
            User user = await this._appDBContext.Users.FindAsync(userID);

            return user;
        }

        /*
         * Get User Details by Email
         */
        public async Task<User> GetUserByEmail(string email)
        {
            return await this._appDBContext?.Users?.FirstOrDefaultAsync(u => u.Email == email);
        }

        /*
         * Get All User Details
         */
        public async Task<User[]> GetAllUsers()
        {
            return await this._appDBContext.Users.ToArrayAsync();
        }

        private string DecodeEmailFromToken(string token)
        {
            var decodedToken = new JwtSecurityTokenHandler();
            var indexOfTokenValue = 7;

            var subToken = decodedToken.ReadJwtToken(token.Substring(indexOfTokenValue));

            return subToken.Payload.FirstOrDefault(x => x.Key == "email").Value.ToString();
        }

        //private async Task<User> ChangeRole(string email, string role)
        //{
        //    var user = await this.GetUserByEmail(email);
        //    user.Role = role;
        //    this._appDBContext.SaveChanges();

        //    return user;
        //}
    }
}
