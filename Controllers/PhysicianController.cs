using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CAS.Controllers
{
    [Authorize(Roles = "Physician")]
    public class PhysicianController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Username = User.Identity?.Name ?? "Physician";
            return View();
        }
    }
}
