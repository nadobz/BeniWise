using Microsoft.AspNetCore.Identity;

namespace BeniWise.DataModel
{
    public class ApplicationUser : IdentityUser
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
    }
}
