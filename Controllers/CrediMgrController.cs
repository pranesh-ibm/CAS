using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CAS.Controllers
{
    public class CrediMgrController : Controller
    {
     
            [HttpGet]
            public IActionResult Login()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> Login(string username, string password)
            {

                Models.CasContext db = new Models.CasContext();

                var usr = db.Users.FirstOrDefault(u => u.UserName == username && u.Password == password);



                if (usr != null)
                {
                    var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, usr.Role)
        };

                    var identity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal);

                    return RedirectToAction("Index", usr.Role);
                
            }

                ModelState.AddModelError("", "Invalid credentials");

                return View();
            }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> AccessDenied()
        {

            return View();
        }

    }
}
