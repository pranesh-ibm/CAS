using CAS.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CAS.Controllers
{
    public class CrediMgrController : Controller
    {
        private readonly CasContext _context;
        public CrediMgrController(CasContext context)
        {
            _context = context;
        }

        public IActionResult RegisterPatient()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPatient([Bind("PatientId,PatientName,Dob,Gender,Address,Phone,Email,Summary")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                patient.PatientStatus = "Pending";
                _context.Add(patient);
                await _context.SaveChangesAsync();
                TempData["ShowSuccess"] = true;
                TempData["SuccessMessage"] = "Registered successfully, Your Password will be First Five characters of your username and @ followed by last Four digits of your mobile number";
                return RedirectToAction("Index","Home");
            }
            return View(patient);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult RegisterPatient(PatientRegisterModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Only add credentials in User table
        //        var user = new User
        //        {
        //            UserName = model.UserName,
        //            Password = HashPassword(model.Password),
        //            Role = "Patient",
        //            ApprovalStatus = "Pending" // Only for self-registered patients
        //        };

        //        _dbContext.Users.Add(user);
        //        _dbContext.SaveChanges();

        //        TempData["Success"] = "Patient registered successfully! Waiting for admin approval.";
        //        return RedirectToAction("Index", "Admin");
        //    }

        //    return View(model);
        //}


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

                    
            //}


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
            return RedirectToAction("Login","CrediMgr");
        }


        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }

    }
}







