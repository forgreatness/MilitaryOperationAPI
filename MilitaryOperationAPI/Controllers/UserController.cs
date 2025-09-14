using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilitaryOperationAPI.Domain.Models;
using MilitaryOperationAPI.Domain.Models.RequestModels;
using MilitaryOperationAPI.Domain.Repositories;
using MilitaryOperationAPI.Helpers;

namespace MilitaryOperationAPI.Controllers
{
    /*
     * The User controller job will be to:
     * 1. CreateUser [AddUser] this should have authorization for only certain priviledge permission
     * 2. Sus
     */
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly IMapper _autoMapperProfile;
        private readonly ILogger _logger;

        public UserController(ILogger logger, IMapper mapper, UserRepository userRepository)
        {
            _logger = logger;
            _autoMapperProfile = mapper;
            _userRepository = userRepository;
        }

        
    }
}
