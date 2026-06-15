using CenngeShop.Models.Home;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CenngeShop.Models.User
{
    public class UserSignUpViewModel
    {
        public UserSignUpFormModel? FormModel { get; set; }

        public ModelStateDictionary? FormModelState { get; set; }

        public bool IsSignUpSuccessful { get; set; }
    }
}
