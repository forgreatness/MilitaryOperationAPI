using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using MilitaryOperationAPI.Domain.Models.Entities;
using MilitaryOperationAPI.Domain.Models.RequestModels;
using MilitaryOperationAPI.Domain.Repositories;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace MilitaryOperationAPI.Domain
{
    public class UserDomain
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly UserRepository _userRepository;
        private readonly RoleRepository _roleRepository;
        private readonly IMapper _autoMapperProfile;

        private const string _registerNewUserDefaultRoleName = "country man";

        public UserDomain(ILogger logger, IConfiguration configuration, UserRepository userRepository, RoleRepository roleRepository, IMapper autoMapperProfile)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._userRepository = userRepository;
            this._roleRepository = roleRepository;
            this._autoMapperProfile = autoMapperProfile;
        }

        /*
         * Input: loginInput {email, password (unhashed form)}
         * Output: token (throw error if doesn't work)
         * Mental Model: {
         *      1. walking into authenticate with a model, we must verify an account exist with email
         *      2. We must then obtain the userDetail for the account
         *      3. we then confirm whether they are authenticated by compared hashedpassword to the password input after hashing
         *      4. We then generate token for user
         *      
         *      JWT token breakdown {
         *          Part 1: Header
         *          Part 2: Payload [claims
         *              Claims Type:
         *                  1. registered (iss, iat, sub, exp, 
         *          Part 3: Signature
         *      }
         * }
         */
        public async Task<string> Authenticate(LoginInputModel loginInput)
        {
            try
            {
                var isAuthenticated = await this._userRepository.Login(loginInput.Email, loginInput.Password);
                if (!isAuthenticated)
                {
                    throw new AuthenticationFailureException("Invalid Credentials");
                }

                var userDetail = await this._userRepository.GetUserByEmail(loginInput.Email);

                var singingKey = this._configuration["Jwt:Key"] ?? throw new ApplicationException("There is no JWT Token Signing Key");
                var issuer = this._configuration["Jwt:Issuer"];
                var audience = this._configuration["Jwt:Audience"];
                var key = Encoding.ASCII.GetBytes(singingKey);

                var claims = new List<Claim>
                {
                    new Claim("Id", Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, $"{userDetail.FirstName} {userDetail.LastName}"),
                    new Claim(JwtRegisteredClaimNames.Email, userDetail.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //represent the id of the generated Token
                };

                claims.AddRange(userDetail.Roles.Select(role => new Claim(ClaimTypes.Role, role.AssignedRole.RoleName)));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            } catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<User> RegisterUser(RegisterInputModel registerInputDTO)
        {
            /*
             * GOAL: We want to add this RegisterInputModel as User model in the User table
             * BEGIN: We start from RegisterInputModel it contains ==> [email, firstname, lastname, password, title] ==> missing [UserID]
             * #1. We nee to verify that the email has not been taken (this is already done in the userRepository.register) ✅
             * #2. We need to map to user object  ✅
             * #3. we need to create a userID ✅
             * #3a. We need to hash the password ✅
             * #4. We need to call userRepository
             */
            var adminID = this._configuration["SystemDataConstant:adminDefaultGUID"];
            var newUserID = Guid.NewGuid();
            var mappedNewUserModel = this._autoMapperProfile.Map<RegisterInputModel, User>(registerInputDTO);
            mappedNewUserModel.Password = BC.HashPassword(mappedNewUserModel.Password);

            var existingUserWithEmail = await this._userRepository.GetUserByEmail(registerInputDTO.Email);

            if (existingUserWithEmail != null) throw new ValidationException("Can't use that email to register");

            var existingUserWithNewID = await this._userRepository.GetUserByID(newUserID);

            while (existingUserWithNewID != null)
            {
                newUserID = Guid.NewGuid();
                existingUserWithNewID = await this._userRepository.GetUserByID(newUserID);
            }

            /* 
             * PURPOSE: Create a new role that is basic called country man and assign it
             * #1. A new role is of type UserRole which demands the following [AssignedRoleID, AssignedUserID, AssignedByUserID, AssignedDate, ExpiredDate]
             * #2. Before we can have this route work, a User (userID, danh1nguyen23@live.com, {password}, Danh, Nguyen, "Admin") ==> AssignedByUserID
             * #2 (continue) A Role (roleID, "Admin") ==> AssignedRoleID
             */

            // #1 Obtain the admin.userID ==> assingedBYUserID & coutnrymanRole.roleID for the UserRole.assignedByUserID and 
            var countryManRole = await this._roleRepository.GetRoleByName(_registerNewUserDefaultRoleName);
            var admin = await this._userRepository.GetUserByID(Guid.Parse(adminID ?? ""));

            var assignedCountryMapRole = new UserRole()
            {
                AssignedRoleID = countryManRole.RoleID,
                AssignedUserID = newUserID,
                AssignedByUserID = admin.UserID,
                AssignedDate = DateTime.UtcNow,
                ExpiredDate = DateTime.UtcNow.AddYears(2)
            };

            var registrationRoles = new List<UserRole>() { assignedCountryMapRole };
            mappedNewUserModel.Roles = registrationRoles;

            var addNewUserResult = await this._userRepository.AddNewUser(mappedNewUserModel);

            return addNewUserResult;
        }
    }
}
