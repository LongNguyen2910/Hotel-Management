using System.Collections.Generic;

namespace Hotel_Management.Models.ViewModel
{
    public class UserWithRolesViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}