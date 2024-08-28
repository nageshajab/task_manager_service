using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using TodoListClient;
using Client.Models;

namespace WebApp_OpenIDConnect_DotNet.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ITokenAcquisition tokenAcquisition;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserValidate UserValidate { get; set; }

        public HomeController(ITokenAcquisition tokenAcquisition, IHttpContextAccessor httpContextAccessor)
        {
            this.tokenAcquisition = tokenAcquisition;
            this._httpContextAccessor = httpContextAccessor;
            UserValidate = new UserValidate(_httpContextAccessor);
        }

        public IActionResult Index()
        {
            ViewBag.ErrorMsg=string.Empty;
            string errormsg= UserValidate.ValidateUser();
            ViewBag.Color = UserValidate.Color;
            ViewBag.ClientCode=UserValidate.ClientCode;

            if (string.IsNullOrEmpty(errormsg))
            {
                return View();
            }
            else
            {
                ViewBag.ErrorMsg=errormsg;
                return View();
            }
        }

      

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}