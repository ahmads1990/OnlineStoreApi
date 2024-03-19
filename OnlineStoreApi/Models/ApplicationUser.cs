using Microsoft.AspNetCore.Identity;
using Serilog;

namespace OnlineStoreApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime LastLoginTime { get; set; }
    }
}
