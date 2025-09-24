using System.Reflection.Emit;
using CategoriesWithProducts.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CategoriesWithProducts.Data
{
    public class CategoryAuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public CategoryAuthDbContext(DbContextOptions<CategoryAuthDbContext> options) : base(options)
        {
        }
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var readerRoleId = "c9139e79-84a5-4432-b07a-0fa1108816d7";
            var writerRoleId = "2411b3be-5a24-44bc-899c-be0708413bde";
            var adminRoleId = "9242dc24-7706-4e24-bf4a-e2f6373dbf3f";


            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = readerRoleId,
                    ConcurrencyStamp = readerRoleId,
                    Name = "Reader",
                    NormalizedName = "Reader".ToUpper()
                },

                new IdentityRole
                {
                    Id = writerRoleId,
                    ConcurrencyStamp = writerRoleId,
                    Name = "Writer",
                    NormalizedName = "WRITER"
                },
                new IdentityRole
                {
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId,
                    Name= "Admin",
                    NormalizedName = "ADMIN"
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);

            var adminUserId = "50a23dd6-bd3a-40ab-b842-67bfbc99852f";
            var adminEmail = "admin@thelist.com";
            var adminPassword = "Admin123*";

            var hasher = new PasswordHasher<ApplicationUser>();
            var adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = adminEmail,
                NormalizedEmail = "ADMIN@THELIST.COM",
                Email = adminEmail,
                NormalizedUserName = "ADMIN@THELIST.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, adminPassword),
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            builder.Entity<ApplicationUser>().HasData(adminUser);

            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = adminUserId, RoleId = adminRoleId },
                new IdentityUserRole<string> { UserId = adminUserId, RoleId = writerRoleId },
                new IdentityUserRole<string> { UserId = adminUserId, RoleId = readerRoleId }
            );

            
        }
    }
}
