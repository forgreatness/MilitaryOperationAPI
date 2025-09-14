namespace MilitaryOperationAPI.Domain.Models.Entities
{
    public class UserRole
    {
        public Guid AssignedRoleID {  get; set; }
        public Guid AssignedUserID { get; set; }
        public Guid? AssignedByUserID { get; set; }
        public User AssignedUser { get; set; } = null!;
        public Role AssignedRole { get; set; } = null!;
        public DateTime AssignedDate { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}
