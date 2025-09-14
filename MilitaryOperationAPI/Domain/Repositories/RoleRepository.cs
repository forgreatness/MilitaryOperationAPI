using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MilitaryOperationAPI.Data;
using MilitaryOperationAPI.Domain.Models.Entities;

namespace MilitaryOperationAPI.Domain.Repositories
{
    public class RoleRepository
    {
        /* Access Modifier [public, private, internal, protected, private protected, protected internal]
         * protected => member is accessible to class and derived child class. 
         * protected internal ==> member is accessible to own assembly and dervived class 
         * private protected ==> member is accessible to within class or even derived class
         * Member MOdifier [readonly, static, virtual, sealed, abstract, override, async]
         */

        private readonly AppDBContext _dbContext;
        private readonly ILogger _logger;
        private readonly IMapper _autoMapperProfile;

        public RoleRepository(AppDBContext dbContext, ILogger logger, IMapper mapper)
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._autoMapperProfile = mapper;
        }

        public async Task<Role> GetRoleByName(string roleName)
        {
            var roleEntity = await this._dbContext.Roles.FirstAsync(r => string.Equals(r.RoleName, roleName, StringComparison.OrdinalIgnoreCase));

            return roleEntity;
        }
    }
}
