using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace asp_all.Models.Home
{
    public class HomeFormsViewModel
    {
        public HomeFormsFormModel? FormModel { get; set; }

        public ModelStateDictionary? FormModelState { get; set; }
    }
}
