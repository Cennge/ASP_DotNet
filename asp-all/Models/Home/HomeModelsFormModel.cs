using Microsoft.AspNetCore.Mvc;

namespace asp_all.Models.Home
{
    public class HomeModelsFormModel
    {
        [FromForm(Name = "user-login")]
        public String UserLogin { get; set; } = null!;

        [FromForm(Name = "user-password")]
        public String UserPassword { get; set; } = null!;

        [FromForm(Name = "user-button")]
        public String UserButton { get; set; } = null!;
    }
}

/*
 * Для моделей форм є принцип зв'язування: дані автоматично
 * потрапляють до моделі за умови що назва властивості збігається
 * з іменем від яким передаються дані (ім'я inpu)
 * Якщо збіг неможливий, зокрема, через "kebab-case", назва
 * встановлюється ерез атрибут
 */