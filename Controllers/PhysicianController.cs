using CAS.Models;
using CAS.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CAS.Controllers
{
    [Authorize(Roles = "Physician")]
    public class PhysicianController : Controller
    {
        private readonly CasContext _context;

        public PhysicianController(CasContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var username = User.Identity?.Name;

            var user = _context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.UserName == username);

            if (user == null || user.RoleReferenceId == null)
            {
                return RedirectToAction("Login", "CrediMgr");
            }

            var physician = _context.Physicians
                .AsNoTracking()
                .FirstOrDefault(p => p.PhysicianId == user.RoleReferenceId);

            if (physician == null)
            {
                return Content("Physician profile not found. Please contact admin.");
            }

            var patients = _context.Schedules
                .AsNoTracking()
                .Where(s => s.PhysicianId == physician.PhysicianId)
                .Select(s => s.Appointment.Patient)
                .GroupBy(p => p.PatientId)
                .Select(g => g.First())
                .OrderBy(p => p.PatientName)
                .ToList();

            var viewModel = new PhysicianHomeViewModel
            {
                PhysicianName = physician.PhysicianName,
                Patients = patients
            };

            return View(viewModel);
        }
    }
}
