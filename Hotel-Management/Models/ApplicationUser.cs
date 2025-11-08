using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_Management.Models
{
    public class ApplicationUser : IdentityUser
    {
        [NotMapped]
        public override string Email { get; set; }

        [NotMapped]
        public override bool EmailConfirmed { get; set; }

        [NotMapped]
        public override string PhoneNumber { get; set; }

        [NotMapped]
        public override bool PhoneNumberConfirmed { get; set; }

        [NotMapped]
        public override bool TwoFactorEnabled { get; set; }

        [NotMapped]
        public override bool LockoutEnabled { get; set; }

        [NotMapped]
        public override DateTimeOffset? LockoutEnd { get; set; }

        [NotMapped]
        public override int AccessFailedCount { get; set; }
    }
}
