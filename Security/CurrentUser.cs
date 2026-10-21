using System.Web;

namespace Student_Att.Security
{
    // Gives any page the logged-in user for the current request.
    // JwtAuthModule fills it in at the start of every request.
    public static class CurrentUser
    {
        private const string ItemKey = "CurrentUser";

        public static void Set(HttpContext context, TokenInfo user)
        {
            context.Items[ItemKey] = user;
        }

        // Returns null when nobody is logged in
        public static TokenInfo Get()
        {
            return HttpContext.Current.Items[ItemKey] as TokenInfo;
        }

        public static bool IsAdmin()
        {
            TokenInfo user = Get();
            return user != null && user.Role == "Admin";
        }
    }
}
