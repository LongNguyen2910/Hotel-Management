using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management.Models
{
    public class UserContext : IdentityDbContext<ApplicationUser>
    {
        public UserContext(DbContextOptions<UserContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Đổi tên bảng cho gọn
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");

                // Chỉ giữ lại các cột cần thiết
                entity.Ignore(u => u.Email);
                entity.Ignore(u => u.PhoneNumber);
                entity.Ignore(u => u.PhoneNumberConfirmed);
                entity.Ignore(u => u.TwoFactorEnabled);
                entity.Ignore(u => u.LockoutEnabled);
                entity.Ignore(u => u.LockoutEnd);
                entity.Ignore(u => u.AccessFailedCount);

                // Tuỳ chỉnh độ dài
                entity.Property(u => u.UserName).HasMaxLength(100);
            });

            builder.Entity<IdentityRole>(entity => entity.ToTable("Roles"));
            builder.Entity<IdentityUserRole<string>>(entity => entity.ToTable("UserRoles"));
            builder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("UserClaims"));
            builder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("UserLogins"));
            builder.Entity<IdentityRoleClaim<string>>(entity => entity.ToTable("RoleClaims"));
            builder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("UserTokens"));
        }
    }
}
