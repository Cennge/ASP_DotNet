using asp_all.Data.Entities;
using asp_all.Services.Hash;
using asp_all.Services.Scoped;
using asp_all.Services.Transient;
using System.Text.Json;
using System.Security.Claims;

namespace asp_all.Middleware.Auth.Session
{
    public class AuthSessionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Query.ContainsKey("logout"))
            {
                context.Session.Remove("UserAccess");
                context.Response.Redirect(context.Request.Path);
                return;
            }
            if (context.Session.Keys.Contains("UserAccess"))
            {
                if (JsonSerializer.Deserialize<UserAccess>
                    (context.Session.GetString("UserAccess")!) is UserAccess userAccess)
                {
                    context.User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            [
                                new Claim(ClaimTypes.Name, userAccess.UserData.Name),
                                new Claim(ClaimTypes.Email, userAccess.UserData.Email),
                                new Claim(ClaimTypes.NameIdentifier, userAccess.Login),
                                new Claim(ClaimTypes.DateOfBirth, userAccess.UserData.Birthdate.ToShortDateString()),
                                new Claim(ClaimTypes.Thumbprint, userAccess.AvatarFilename ?? ""),
                                new Claim(ClaimTypes.Role, 
                                userAccess.UserRoleId == Guid.Parse("56D473BA-ED6B-4695-AEBF-439E2102F2C3")
                                ? "Admin"
                                : "Guest"),
                            ],
                            nameof(AuthSessionMiddleware)
                        )
                    );
                }
            }

            await _next(context);
        }
    }
}
