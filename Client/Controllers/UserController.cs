using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using TodoListClient;
using TodoListClient.Services;
using System.Threading.Tasks;
using Client.Models;

namespace WebApp_OpenIDConnect_DotNet.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private IUserService _userService;
        private readonly ITokenAcquisition tokenAcquisition;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserValidate UserValidate { get; set; }

        public UserController(ITokenAcquisition tokenAcquisition, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            this.tokenAcquisition = tokenAcquisition;
            this._httpContextAccessor = httpContextAccessor;
            UserValidate = new UserValidate(_httpContextAccessor);
            _userService = userService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            string errormsg = "";
            if (!UserValidate.IsAdminUser())
                errormsg = "You need atleast admin role.";

            ViewBag.ErrorMsg = string.Empty;
            ViewBag.Color = UserValidate.Color;
            ViewBag.ClientCode = UserValidate.ClientCode;

            if (string.IsNullOrEmpty(errormsg))
            {
                var lst = await _userService.GetAsync();
                return View(lst);
            }
            else
            {
                ViewBag.ErrorMsg = errormsg;
                return View();
            }
        }

        // GET: TodoList/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            User user = await this._userService.GetAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: TodoList/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, User user)
        {
            user.ClientCode = user.ClientCode == "Select" ? "" : user.ClientCode;

            await _userService.EditAsync(user);
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}