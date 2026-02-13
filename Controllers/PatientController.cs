using CAS.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CAS.Controllers
{
    public class PatientController : Controller
    {
        private readonly CasContext _context;

        public PatientController(CasContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewProfile()
        {
            var username = User.Identity?.Name;

            var user = _context.Users
                .FirstOrDefault(u => u.UserName == username);

            if (user == null)
                return RedirectToAction("Login", "CrediMgr");

            var patient = _context.Patients
                .FirstOrDefault(c => c.PatientId == user.RoleReferenceId);


            if (patient == null)
            {
                return Content("Chemist profile not found. Please contact admin.");
            }

            return View(patient);
        }
        
        public IActionResult EditProfile()
        {
            var username = User.Identity?.Name;
            var user = _context.Users
                .FirstOrDefault(u => u.UserName == username);

            if (user == null)
                return RedirectToAction("Login", "CrediMgr");
            var patient = _context.Patients
                                  .FirstOrDefault(p => p.PatientId == user.RoleReferenceId);

            if (patient == null) return NotFound();

            return View(patient);
        }

        // POST: Edit Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(Patient model)
        {
            var username = User.Identity?.Name;
            var user = _context.Users
                .FirstOrDefault(u => u.UserName == username);

            if (user == null)
                return RedirectToAction("Login", "CrediMgr");

            var patient = _context.Patients
                                  .FirstOrDefault(p => p.PatientId == user.RoleReferenceId);

            if (patient == null) return NotFound();

            // Update only allowed fields
            patient.Email = model.Email;
            patient.Address = model.Address;
            patient.Dob = model.Dob;
            patient.Gender = model.Gender;
            patient.Summary = model.Summary;

            _context.SaveChanges();

            return RedirectToAction("ViewProfile");
        }
    
}
}
