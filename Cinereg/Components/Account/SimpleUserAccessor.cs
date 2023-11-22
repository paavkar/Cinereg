using Cinereg.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace Cinereg.Components.Account
{
    internal sealed class SimpleUserAccessor(
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager,
        SimpleRedirectManager redirectManager)
    {
        public async Task<ApplicationUser> GetRequiredUserAsync()
        {
            var principal = httpContextAccessor.HttpContext?.User ??
                throw new InvalidOperationException($"{nameof(GetRequiredUserAsync)} requires access to an {nameof(HttpContext)}.");

            var user = await userManager.GetUserAsync(principal);

            if (user is null)
            {
                redirectManager.RedirectTo("/");
            }

            return user;
        }
    }
}
