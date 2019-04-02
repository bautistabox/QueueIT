using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace QueueIT.Identity
{
    public class QueueItUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<QueueItUser>
    {
        public QueueItUserClaimsPrincipalFactory(UserManager<QueueItUser> userManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(QueueItUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("firstName", user.FirstName));
            identity.AddClaim(new Claim("lastName", user.LastName));
            
            return identity;
        }
    }
}