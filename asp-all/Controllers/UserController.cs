    using asp_all.Data;
using asp_all.Data.Entities;
using asp_all.Models.Home;
using asp_all.Models.User;
using asp_all.Services.Kdf;
using asp_all.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Buffers.Text;
using System.Text;
using System.Text.Json;

namespace asp_all.Controllers
{
    public class UserController(DataContext dataContext, IStorageService storageService, IKdfService kdfService) : Controller
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IStorageService _storageService = storageService;
        private readonly IKdfService _kdfService = kdfService;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Profile()
        {
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                return View();
            }
            return Redirect("/");
        }

        public IActionResult SignUp()
        {
            UserSignUpViewModel viewModel = new();
            if (HttpContext.Session.Keys.Contains(nameof(UserSignUpFormModel)))
            {
                viewModel.FormModel = JsonSerializer.Deserialize<UserSignUpFormModel>(
                    HttpContext.Session.GetString(nameof(UserSignUpFormModel))!
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
                viewModel.IsSignUpSuccessful = modelState.IsValid;
                if (viewModel.IsSignUpSuccessful)
                {
                    Guid userId = Guid.NewGuid();
                    _dataContext.UsersData.Add(new()
                    {
                        Id = userId,
                        Name = viewModel.FormModel!.UserName,
                        Email = viewModel.FormModel!.UserEmail,
                        Birthdate = viewModel.FormModel!.UserBirthdate!.Value,
                    });
                    String salt = Guid.NewGuid().ToString();
                    _dataContext.UserAccesses.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        UserRoleId = _dataContext.UserRoles.First(r => r.Name == "Self Registered").Id,
                        Salt = salt,
                        Dk = _kdfService.Dk(salt, viewModel.FormModel!.UserPassword),
                        Login = viewModel.FormModel!.UserLogin,
                        AvatarFilename = viewModel.FormModel!.SavedFilename,
                        CreatedAt = DateTime.Now
                    });
                    _dataContext.SaveChanges();
                }

                HttpContext.Session.Remove(nameof(UserSignUpFormModel));
                HttpContext.Session.Remove(nameof(ModelState));
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult SignUpForm(UserSignUpFormModel formModel)
        {
            if (formModel.UserBirthdate != null && (DateTime.Now - formModel.UserBirthdate!.Value).Days < 3650)
            {
                ModelState.AddModelError("user-birthdate", "Вік замалий для реєстрації");
            }
            if (formModel.UserPassword != null)
            {
                string password = formModel.UserPassword;

                if (password.Length < 6)
                {
                    ModelState.AddModelError("user-password", "Пароль має бути мінімум 6 символів");
                }

                if (!password.Any(char.IsDigit))
                {
                    ModelState.AddModelError("user-password", "Пароль має містити хоча б одну цифру");
                }

                if (!password.Any(char.IsLower))
                {
                    ModelState.AddModelError("user-password", "Пароль має містити хоча б одну маленьку літеру");
                }

                if (!password.Any(char.IsUpper))
                {
                    ModelState.AddModelError("user-password", "Пароль має містити хоча б одну велику літеру");
                }

                if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                {
                    ModelState.AddModelError("user-password", "Пароль має містити хоча б один спецсимвол");
                }
            }
            if (formModel.UserPassword != formModel.UserRepeat)
            {
                ModelState.AddModelError("user-repeat", "Повтор не збігається з паролем");
            }
            if (formModel.UserAvatar != null && formModel.UserAvatar.Length > 0)
            {
                var ext = Path.GetExtension(formModel.UserAvatar.FileName).ToLower();

                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif")
                {
                    ModelState.AddModelError("user-avatar", "Дозволені формати: .png, .jpg, .jpeg, .gif");
                }
            }
            if (formModel.UserLogin != null)
            {
                if (_dataContext.UserAccesses.Any(ua => ua.Login == formModel.UserLogin))
                {
                    ModelState.AddModelError("user-login", "Даний логін вже у вжитку");
                }
            }


            if (ModelState.IsValid && formModel.UserAvatar != null && formModel.UserAvatar.Length > 0)
            {
                formModel.SavedFilename = _storageService.Save(formModel.UserAvatar);
            }

            HttpContext.Session.SetString(
                nameof(ModelState),
                JsonSerializer.Serialize(ModelState)
            );

            HttpContext.Session.SetString(
                nameof(UserSignUpFormModel),
                JsonSerializer.Serialize(formModel)
            );
            return RedirectToAction(nameof(SignUp));
        }

        [HttpGet]
        public JsonResult SignIn()
        {
            String authHeader = Request.Headers.Authorization.ToString();
            if (String.IsNullOrEmpty(authHeader))
            {
                return Json(new
                {
                    status = 401,
                    data = "Missing 'Authorization' header"
                });
            }
            String scheme = "Basic ";
            if (!authHeader.StartsWith(scheme))
            {
                return Json(new
                {
                    status = 401,
                    data = "Invalid 'Authorization' scheme. Must be " + scheme
                });
            }
            String basicCredentials = authHeader[scheme.Length..];
            String userPass;
            try
            {
                userPass = Encoding.UTF8.GetString(Convert.FromBase64String(basicCredentials));
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = 401,
                    data = "Credentials decode error " + ex.Message
                });
            }
            String[] parts = userPass.Split(':', 2);
            if (parts.Length != 2 )
            {
                return Json(new
                {
                    status = 401,
                    data = "user-pass invalid format. Missing ':'?"
                });
            }
            UserAccess? userAccess = _dataContext.UserAccesses.Include(u => u.UserData).FirstOrDefault(u => u.Login == parts[0]);
            if (userAccess == null)
            {
                return Json(new
                {
                    status = 401,
                    data = "Authentication rejected"
                });
            }
            if (_kdfService.Dk(userAccess.Salt, parts[1]) != userAccess.Dk)
            {
                return Json(new
                {
                    status = 401,
                    data = "Authentication rejected."
                });
            }

            HttpContext.Session.SetString("UserAccess", JsonSerializer.Serialize(userAccess));

            return Json(new
            {
                status = 200,
                data = userAccess
            });
        }
    }
}
