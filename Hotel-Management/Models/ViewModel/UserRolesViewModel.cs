using System.Collections.Generic;

namespace Hotel_Management.Models.ViewModel
{
    public class RoleSelection
    {
        public string Name { get; set; } = string.Empty;
        public bool Selected { get; set; }
    }

    public class UserRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<RoleSelection> Roles { get; set; } = new();
        public string SelectedRoleName { get; set; } = string.Empty;
    }
}