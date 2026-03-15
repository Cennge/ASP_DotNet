using asp_all.Models;
using asp_all.Models.Home;
using asp_all.Services.DateTime;
using asp_all.Services.Hash;
using asp_all.Services.Scoped;
using asp_all.Services.Transient;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace asp_all.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDateTimeService _dateTimeService;
        private readonly IHashService _hashService;
        private readonly ScopedService _scopedService;
        private readonly TransientService _transientService;

        public HomeController(IDateTimeService dateTimeService, IHashService hashService, ScopedService scopedService, TransientService transientService)
        {
            _hashService = hashService;
            _scopedService = scopedService;
            _transientService = transientService;
            _dateTimeService = dateTimeService;
        }

        public IActionResult Middleware()
        {

            return View();
        }

        public IActionResult Forms()
        {
            HomeFormsViewModel viewModel = new();
            if (HttpContext.Session.Keys.Contains(nameof(HomeFormsFormModel)))
            {
                // є збережені у сесії дані, тоді відновлюємо, використовуємо та видаляємо
                viewModel.FormModel = JsonSerializer.Deserialize<HomeFormsFormModel>(
                    HttpContext.Session.GetString(nameof(HomeFormsFormModel))!
                );
                HttpContext.Session.Remove(nameof(HomeFormsFormModel));
            }

            return View(viewModel);
        }

        // метод для прийому даних форми, збереження у сесії та передачі редирект
        public IActionResult FormsReceiver(HomeFormsFormModel formModel)
        {
            HttpContext.Session.SetString(
                nameof(HomeFormsFormModel),
                JsonSerializer.Serialize(formModel)
            );

            return RedirectToAction(nameof(Forms));   //  /Home/Forms - формує ASP
        }

        public IActionResult IoC()
        {
            ViewData["hash"] = _hashService.Digest("123");
            ViewData["hashCode"] = _hashService.GetHashCode();
            ViewData["ControllerScopedHash"] = _scopedService.GetHashCode();
            ViewData["ControllerTransientHash"] = _transientService.GetHashCode();
            ViewData["time"] = _dateTimeService.GetTime(DateTime.Now);
            ViewData["date"] = _dateTimeService.GetDate(DateTime.Now);
            ViewData["ControllerTicks"] = DateTime.Now.Ticks;

            return View();
        }

        public IActionResult Intro()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Pricing()
        {
            return View();
        }

        public IActionResult Razor()
        {
            return View();
        }

        public IActionResult Models(RegisterModel registerModel)
        {
            RegisterViewModel viewModel = new();
            if (registerModel.UserButton != null)
            {
                viewModel.RegisterModel = registerModel;
            }
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
