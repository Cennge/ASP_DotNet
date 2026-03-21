using asp_all.Models.Home;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace asp_all.Models.User
{
    public class UserSignUpViewModel
    {
        public UserSignUpFormModel? FormModel { get; set; }

        public ModelStateDictionary? FormModelState { get; set; }

        public bool IsSignUpSuccessful { get; set; }
    }
}
