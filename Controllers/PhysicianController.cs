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

        public async Task<IActionResult> Index()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
            {
                return RedirectToAction("Login", "CrediMgr");
            }

            var physicianUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == username && u.Role == "Physician");

            if (physicianUser?.RoleReferenceId == null)
            {
                return Forbid();
            }

            var patients = await _context.Schedules
                .AsNoTracking()
                .Where(s => s.PhysicianId == physicianUser.RoleReferenceId)
                .Select(s => s.Appointment.Patient)
                .Distinct()
                .OrderBy(p => p.PatientName)
                .ToListAsync();

            var model = new PhysicianHomeViewModel
            {
                UserName = username,
                Patients = patients
            };

            return View(model);
        }

        public async Task<IActionResult> PatientDetails(int id)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
            {
                return RedirectToAction("Login", "CrediMgr");
            }

            var physicianUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == username && u.Role == "Physician");

            if (physicianUser?.RoleReferenceId == null)
            {
                return Forbid();
            }

            var patient = await _context.Schedules
                .AsNoTracking()
                .Where(s => s.PhysicianId == physicianUser.RoleReferenceId && s.Appointment.PatientId == id)
                .Select(s => s.Appointment.Patient)
                .FirstOrDefaultAsync();

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }
    }
}
