using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilitaryOperationAPI.Domain;
using MilitaryOperationAPI.Domain.Models.Entities;
using MilitaryOperationAPI.Domain.Models.RequestModels;
using MilitaryOperationAPI.Domain.Repositories;

namespace MilitaryOperationAPI.Controllers
{
    /*
     * The AuthenticationController should be a controller that does following:
     * 1. Login User
     * 2. Renew User token
     * 3. Revoke User access (this one we should have a table to keep track of all token that was assigned earlier and revoke that token whenever user violates something)
     *      3a. Usually revoke token is rare but cases like (hitting request limit, if new role is given to account the old token should be revoke)
     */
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : Controller
    {
        // Access Modifier [public, private, internal, protected, internal protected, private protected]
        // Member MOdifer [override, virtual, abstract, static, readonly, sealed (unchangable)]
        private readonly ILogger _logger;
        private readonly UserRepository _userRepository;
        private readonly UserDomain _userDomain;
        private readonly IMapper _autoMapperProfile;

        public AuthenticationController(ILogger logger, UserRepository userRepository, UserDomain userDomain, IMapper autoMapperProfile)
        {
            _logger = logger;
            _userRepository = userRepository;
            _autoMapperProfile = autoMapperProfile;
            _userDomain = userDomain;
        }

        /*
         * LOGIN 
         * 1. Loggin in requires that we call the domain to do it for us. Domain will take care of business logic on 
         * a) Verifying credentials
         * b) Obtaining userDetails
         * c) generating jwtAuthToken
         * #1. We should check if the user email exist, if they exist compare the passed in passwords to the passwords of the user that was first found
         * #2. If we don't find the user the controller job is to return Unauthenticated(401) rather then bad request because we want to hide implementation
         * #3. If the password doesn't match we should do the same
         *
         */
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(LoginInputModel loginInput)
        {
            try
            {
                if (ModelState.IsValid) // This verify that the model based in from the reqeust is valid based on the attributes defined from LoginInputModel.
                {
                    var authorizationToken = this._userDomain.Authenticate(loginInput);

                    if (authorizationToken == null)
                    {
                        return Unauthorized("Invalid Credentials");
                    }

                    return Ok(authorizationToken);
                }

                // All actionResult [BadRequest, Ok, BadRequestResult, CreatedRequest, NotFoundResult(404), OkObjectResult]
                // IActionResult [BadRequest, Ok, StatusCode, CreatedRequest, OkObjectREsult, NotFound
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        /*
         * This is regular register a new account: Therefore you can only start out as "Country Man" if it is any other role, it is not allowed
         */
        [AllowAnonymous] //Don't apply any authorize rule for this endpoint. KNown as "Authorization Attribute"
        [HttpPost("Register")] // known as Action method attributes
        public async Task<ActionResult<string>> Register(RegisterInputModel requestInputModel)
        {
            // This is the middleware acting as the handler for this HTTP request. THe job of this middleware is 1 thing, organize services to get response
            // If error encounters, handle them as well and return appropriate response. This middleware must have response for all scenarios
            try
            {
                // For the parameters passed into this middleware does it meet the validation attributes (validation attribute are a type of attributes with values: [range, maxlength, minlength, required, emailadress]
                if (ModelState.IsValid)
                {
                    // Step #1: We have a requestInputModel but then the UserRepository expect a User model to add/register therefore:
                    // goal: "we gotta convert requestInputModel from RegisterInputModel to User"
                    // Maybe we can pass the requestInputModel to the UserDomain to convert it and then have UserDomain call the userRepository. 

                    var registerUserResult = await this._userDomain.RegisterUser(requestInputModel);

                    if (registerUserResult != null) return Ok(registerUserResult);
                }

                return BadRequest(ModelState);


                /*
                 * These are all possibl actionresult
                 * 1. OkResult, OkObjectResult (200)
                 * 2. CreatedResult (201)
                 * 3. NOContent (204)
                 * 4. BadRequestREsult (400) | BadRequest()
                 * 5. NotFound(404)
                 * 6. UnAuthorized (401)
                 * 7. Forbidden (403)
                 * 8. Statuscode (...)
                 * 9. Statuscode(500) 
                 */
            }
            catch (Exception ex)
            {
                // If its certain exception you can return certin Actionresult
                /*
                 * 1. CreatedResult (201)
                 * 2. UnauthorizedResult(401)
                 * 3. Forbidden (403)
                 * 4. 
                 */
                
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }
    }
}
