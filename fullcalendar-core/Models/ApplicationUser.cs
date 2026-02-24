using Microsoft.AspNetCore.Identity;

namespace fullcalendarcore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }
    }
}
