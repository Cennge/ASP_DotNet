using Microsoft.AspNetCore.Mvc;

namespace asp_all.Models.Home
{
    public class RegisterModel
    {
        [FromForm(Name = "user-email")]
        public String UserEmail { get; set; } = null!;

        [FromForm(Name = "user-password")]
        public String UserPassword { get; set; } = null!;

        [FromForm(Name = "user-repeat")]
        public String UserRepeat { get; set; } = null!;

        [FromForm(Name = "user-birthdate")]
        public String UserBirthdate { get; set; } = null!;

        [FromForm(Name = "user-button")]
        public String UserButton { get; set; } = null!;
    }
}
