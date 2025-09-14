namespace MilitaryOperationAPI.Domain.Models.Entities
{
    public class Role
    {
        public Guid RoleID {  get; set; }
        public string RoleName { get; set; } = string.Empty;
        public ICollection<UserRole> Users { get; set; } = new List<UserRole>();

        public Role(Guid roleId, string roleName)
        {
            RoleID = roleId;
            RoleName = roleName;
        }
    }
}