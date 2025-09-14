using Microsoft.EntityFrameworkCore;
using MilitaryOperationAPI.Domain.Models.Entities;
using BC = BCrypt.Net.BCrypt;

namespace MilitaryOperationAPI.Data
{
    /*
     * DbContext (functionalities)
     * Provides ORM to access DB
     * Helps abstract establishment of the connection layer so user don't have to implement it
     * ... more wrirtten in notes
     * migrations
     * linq query
     * change trackiung
     */
    public class AppDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles {  get; set; }

        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }

        /*
         * Protected means derived class or current class can use or call it
         * 
         * PURPOSE: The purpose of OnMOdelCreating is to cuztomize how the devloper want to map the C# class /object to database schema. 
         * SCHEMA DECLARATION
         * ==> What is the primary key of the properties in the class
         * ==> Wath are the foreign keys of the model
         * ==> what field must be required
         */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /*
             * Create the default ROLES the system expects
             */
            //var roles = new List<Role>()
            //{
            //    new Role(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Admin"),
            //}

            var roles = new Role[]
            {
                new Role(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Admin"),
                new Role(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Dev"),
                new Role(Guid.Parse("33333333-3333-3333-3333-333333333333"), "President"),
                new Role(Guid.Parse("44444444-4444-4444-4444-444444444444"), "Vice President"),
                new Role(Guid.Parse("55555555-5555-5555-5555-555555555555"), "Secretary of Defense"),
                new Role(Guid.Parse("66666666-6666-6666-6666-666666666666"), "Country Man"),
            };
            var adminRole = roles.Single(r => r.RoleName.ToLower() == "admin");

            // Role Constraints
            modelBuilder.Entity<Role>(r =>
            {
                r.HasKey(r => r.RoleID);
                r.HasData(roles);
            }); // using the modelBuilder to create the model in db, we want to create entity of generic type ROle with a passed in function where each role, is defeined by roleid as PK

            var defaultAdminPWD = "ILoveTennis";
            var defaultAdminID = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";

            var defaultAdmin = new User(
                userID: Guid.Parse(defaultAdminID ?? "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                email: "danh3nguyen15@gmail.com",
                password: BC.HashPassword(defaultAdminPWD),
                firstName: "Danh",
                lastName: "Nguyen",
                title: "Software Architect"
            );

            // This is saying that we are configuring the User entity 
            modelBuilder.Entity<User>().HasKey(r => r.UserID);
            modelBuilder.Entity<User>().Property(u => u.Email).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<User>().Property(u => u.Password).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<User>().HasData(defaultAdmin);

            // UserRole Constraints
            modelBuilder.Entity<UserRole>(uRole =>
            {
                // UserRole has 1 assignedUserID and AssignedRoleID
                uRole.HasKey(ur => new { ur.AssignedUserID, ur.AssignedRoleID });

                // A UserRole has one AssignedUser object which within it has many roles of type UserRole and has a foreignkey referencing AssignedUserID
                uRole.HasOne(ur => ur.AssignedUser)
                     .WithMany(u => u.Roles)
                     .HasForeignKey(ur => ur.AssignedUserID);

                // A UserRole has One AssignedRole Object which within it has many users
                uRole.HasOne(ur => ur.AssignedRole)
                     .WithMany(r => r.Users)
                     .HasForeignKey(ur => ur.AssignedRoleID);

                uRole.HasData(new UserRole { AssignedRoleID = adminRole.RoleID, AssignedUserID = defaultAdmin.UserID, AssignedDate = DateTime.UtcNow, AssignedByUserID = null });
            });

            // These are constraint for Operation
            modelBuilder.Entity<Operation>().HasKey(op => new { op.OperationID });
            modelBuilder.Entity<Operation>().HasOne(op => op.AssignedByUser).WithMany().HasForeignKey("AssignedByUserID").OnDelete(DeleteBehavior.Cascade); // configure the operationeneity hasONe() related user who assigned operation. While the user assigned the operation can assign many different operation
            modelBuilder.Entity<Operation>().Property(op => op.ActionType).HasConversion<string>(); //This convert the ActionType Enum into string values to be store in DB, and vice versa DB string value to ActionType Enum
        }
    }
}
