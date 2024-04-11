using System.Security.Claims;
using ForumApi.Data.Models;

namespace ForumApi.Utils.Extensions
{
    public static class UserClaims
    {
        public static int GetId(this ClaimsPrincipal user)
        {
            var sub = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(sub?.Value))
                throw new ArgumentNullException("User claims does not contain Id");

            return int.Parse(sub.Value);
        }

        public static bool IsAdminOrModer(this ClaimsPrincipal user) =>
            user.Identity?.IsAuthenticated == true &&
            (user.IsInRole(Role.Admin) || user.IsInRole(Role.Moder));

        public static bool IsAuthed(this ClaimsPrincipal user) =>
            user.Identity?.IsAuthenticated == true;
    }
}