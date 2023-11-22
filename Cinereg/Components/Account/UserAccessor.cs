using Cinereg.Data;
using Microsoft.AspNetCore.Identity;

namespace Cinereg.Components.Account
{
    internal sealed class UserAccessor(UserManager<ApplicationUser> userManager, IdentityRedirectManager redirectManager)
    {
        public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
        {
            var user = await userManager.GetUserAsync(context.User);

            if (user is null)
            {
                redirectManager.RedirectTo("/");
            }

            return user;
        }
    }
}
