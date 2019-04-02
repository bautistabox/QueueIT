using Microsoft.AspNetCore.Identity;

namespace QueueIT.Identity
{
    public class QueueItUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}