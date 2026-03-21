using Microsoft.AspNetCore.Mvc;

namespace asp_all.Models.Home
{
    public class DerivedKeyFormModel
    {
        [FromForm(Name = "dk-salt")]
        public String? DkSalt { get; set; }

        [FromForm(Name = "dk-password")]
        public String DkPassword { get; set; } = null!;

        [FromForm(Name = "dk-button")]
        public String DkButton { get; set; } = null!;
    }
}
