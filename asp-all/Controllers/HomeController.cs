using asp_all.Data;
using asp_all.Models;
using asp_all.Models.Home;
using asp_all.Services.DateTime;
using asp_all.Services.Hash;
using asp_all.Services.Scoped;
using asp_all.Services.Transient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;
using System.Text.Json;

namespace asp_all.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IDateTimeService _dateTimeService;
        private readonly IHashService _hashService;
        private readonly ScopedService _scopedService;
        private readonly TransientService _transientService;

        public HomeController(IDateTimeService dateTimeService, IHashService hashService, ScopedService scopedService, TransientService transientService, DataContext dataContext)
        {
            _hashService = hashService;
            _scopedService = scopedService;
            _transientService = transientService;
            _dateTimeService = dateTimeService;
            _dataContext = dataContext;
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
                ModelStateDictionary modelState = new();
                JsonElement savedState = JsonSerializer.Deserialize<JsonElement>(
                    HttpContext.Session.GetString(nameof(ModelState))!
                )!;
                foreach (var item in savedState.EnumerateObject())
                {
                    var errors = item.Value.GetProperty("Errors");
                    if (errors.GetArrayLength() > 0)
                    {
                        foreach (var err in errors.EnumerateArray())
                        {
                            modelState.AddModelError(item.Name, err.GetProperty("ErrorMessage").GetString()!);
                        }
                    }
                }

                viewModel.FormModelState = modelState;

                HttpContext.Session.Remove(nameof(HomeFormsFormModel));
                HttpContext.Session.Remove(nameof(ModelState));
            }

            return View(viewModel);
        }

        // метод для прийому даних форми, збереження у сесії та передачі редирект
        public IActionResult FormsReceiver(HomeFormsFormModel formModel)
        {
            if (_dataContext.UserAccesses.Any(ua => ua.Login == formModel.UserLogin))
            {
                ModelState.AddModelError("user-login", "Даний логін вже у вжитку");
            }
            //Валідація форми
            HttpContext.Session.SetString(
                nameof(ModelState),
                JsonSerializer.Serialize(ModelState)
            );

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

        public IActionResult DerivedKey()
        {
            DerivedKeyViewModel viewModel = new();
            if (HttpContext.Session.Keys.Contains(nameof(DerivedKeyFormModel)))
            {
                viewModel.DerivedKeyModel = JsonSerializer.Deserialize<DerivedKeyFormModel>(
                    HttpContext.Session.GetString(nameof(DerivedKeyFormModel))!
                );
                HttpContext.Session.Remove(nameof(DerivedKeyFormModel));
            }
            return View(viewModel);
        }

        public IActionResult DerivedKeyFormReceiver(DerivedKeyFormModel derivedKeyModel)
        {
            HttpContext.Session.SetString(
                nameof(DerivedKeyFormModel),
                JsonSerializer.Serialize(derivedKeyModel)
            );

            return RedirectToAction(nameof(DerivedKey));
        }

        public IActionResult Models()
        {
            RegisterViewModel viewModel = new();
            if (HttpContext.Session.Keys.Contains(nameof(RegisterModel)))
            {
                viewModel.RegisterModel = JsonSerializer.Deserialize<RegisterModel>(
                    HttpContext.Session.GetString(nameof(RegisterModel))!
                );
                HttpContext.Session.Remove(nameof(RegisterModel));
            }
            return View(viewModel);
        }

        public IActionResult RegisterFormReceiver(RegisterModel registerModel)
        {
            HttpContext.Session.SetString(
                nameof(RegisterModel),
                JsonSerializer.Serialize(registerModel)
            );

            return RedirectToAction(nameof(Models));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
