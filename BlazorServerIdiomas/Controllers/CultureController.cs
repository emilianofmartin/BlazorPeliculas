using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorServerIdiomas.Controllers {
    [Route("[controller]/[action]")]
    public class CultureController: Controller {
        public IActionResult SetCulture(string culture, string redirectURL) {
            if(culture != null) {
                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(
                        new RequestCulture(culture)
                    )
                );
            }

            return LocalRedirect(redirectURL);
        }
    }
}
