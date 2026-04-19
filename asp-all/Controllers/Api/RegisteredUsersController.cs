using asp_all.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Collections.Specialized.BitVector32;

namespace asp_all.Controllers.Api
{
    [Route("api/registeredusers")]
    [ApiController]
    public class RegisteredUsersController(DataContext dataContext) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;

        [HttpGet]
        public IActionResult DoGet()
        {
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                String role = HttpContext.User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? String.Empty;
                if (role == "Admin")
                {
                    var users = _dataContext
                    .UsersData
                    .Where(u => u.DeletedAt == null)
                    .AsEnumerable();
                    return Ok(users);
                }
                else
                {
                    return Forbid();
                }
            } 
            return Unauthorized();
        }

        [HttpGet("{idOrLogin}")]
        public IActionResult DoGetById(String idOrLogin)
        {
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                String role = HttpContext.User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? String.Empty;
                if (role == "Admin")
                {
                    var user = _dataContext
                    .UserAccesses
                    .FirstOrDefault(u => u.DeletedAt == null && u.UserData.Id.ToString() == idOrLogin || u.Login == idOrLogin);
                    return Ok(user);
                }
                else
                {
                    return Forbid();
                }
            }
            return Unauthorized();
        }
    }
}
