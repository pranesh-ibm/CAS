using CAS.Models;
using CAS.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CAS.Controllers;

[Authorize(Roles = "Physician")]
public class PhysicianController : Controller
{
    private readonly CasContext _context;

    public PhysicianController(CasContext context)
    {
        _context = context;
    }

    private int? GetPhysicianId()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return null;
        var user = _context.Users.FirstOrDefault(u => u.UserName == username);
        return user?.RoleReferenceId;
    }

    public async Task<IActionResult> Index()
    {
        var physicianId = GetPhysicianId();
        if (physicianId == null) return RedirectToAction("Login", "CrediMgr");

        var physician = await _context.Physicians.FindAsync(physicianId.Value);
        if (physician == null) return NotFound();

        var vm = new PhysicianProfileViewModel
        {
            PhysicianId = physician.PhysicianId,
            PhysicianName = physician.PhysicianName,
            Specialization = physician.Specialization,
            Address = physician.Address,
            Phone = physician.Phone,
            Email = physician.Email,
            Summary = physician.Summary
        };

        return View(vm);
    }

    // GET: Edit Profile
    public async Task<IActionResult> EditProfile()
    {
        var physicianId = GetPhysicianId();
        if (physicianId == null) return RedirectToAction("Login", "CrediMgr");

        var physician = await _context.Physicians.FindAsync(physicianId.Value);
        if (physician == null) return NotFound();

        var vm = new PhysicianProfileViewModel
        {
            PhysicianId = physician.PhysicianId,
            PhysicianName = physician.PhysicianName,
            Specialization = physician.Specialization,
            Address = physician.Address,
            Phone = physician.Phone,
            Email = physician.Email,
            Summary = physician.Summary
        };

        return View(vm);
    }

    // POST: Edit Profile
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(PhysicianProfileViewModel model)
    {
        var physicianId = GetPhysicianId();
        if (physicianId == null) return RedirectToAction("Login", "CrediMgr");

        var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        if (!ModelState.IsValid)
        {
            if (isAjax)
            {
                var first = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault() ?? "Validation failed.";
                return BadRequest(new { success = false, message = first });
            }
            return View(model);
        }

        var physician = await _context.Physicians.FindAsync(physicianId.Value);
        if (physician == null) return NotFound();

        // Update allowed fields only
        physician.PhysicianName = model.PhysicianName;
        physician.Specialization = model.Specialization;
        physician.Address = model.Address;
        physician.Phone = model.Phone;
        physician.Email = model.Email;
        physician.Summary = model.Summary;

        _context.Update(physician);
        await _context.SaveChangesAsync();

        if (isAjax)
        {
            return Json(new { success = true, message = "Profile updated successfully." });
        }

        TempData["SuccessMessage"] = "Profile updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // View Patients assigned to this physician (via Schedules -> Appointment -> Patient)
    public async Task<IActionResult> ViewPatients()
    {
        var physicianId = GetPhysicianId();
        if (physicianId == null) return RedirectToAction("Login", "CrediMgr");

        var patients = await _context.Schedules
            .Where(s => s.PhysicianId == physicianId)
            .Include(s => s.Appointment)
                .ThenInclude(a => a.Patient)
            .Select(s => s.Appointment.Patient)
            .Distinct()
            .ToListAsync();

        var vm = new PatientListViewModel { Patients = patients };
        return View(vm);
    }

    public async Task<IActionResult> PatientDetails(int id)
    {
        var physicianId = GetPhysicianId();
        if (physicianId == null) return RedirectToAction("Login", "CrediMgr");

        var patient = await _context.Schedules
            .AsNoTracking()
            .Where(s => s.PhysicianId == physicianId && s.Appointment.PatientId == id)
            .Select(s => s.Appointment.Patient)
            .FirstOrDefaultAsync();

        if (patient == null)
        {
            return NotFound();
        }

        return View(patient);
    }

    // GET: Create Prescription
    public async Task<IActionResult> CreatePrescription()
    {
        var physicianId = GetPhysicianId();
        if (physicianId == null) return RedirectToAction("Login", "CrediMgr");

        // schedules for this physician (upcoming/past) with patient info
        var schedules = await _context.Schedules
            .Where(s => s.PhysicianId == physicianId)
            .Include(s => s.Appointment)
                .ThenInclude(a => a.Patient)
            .ToListAsync();

        var drugs = await _context.Drugs
            .Where(d => d.DrugStatus == "Active")
            .ToListAsync();

        var vm = new CreatePrescriptionViewModel
        {
            Schedules = schedules,
            Drugs = drugs
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePrescription(CreatePrescriptionFormModel form)
    {
        var physicianId = GetPhysicianId();
        if (physicianId == null) return RedirectToAction("Login", "CrediMgr");

        if (!ModelState.IsValid)
        {
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var first = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault() ?? "Validation failed.";
                return BadRequest(new { success = false, message = first });
            }

            // reload lists
            var vm = new CreatePrescriptionViewModel
            {
                Schedules = await _context.Schedules
                    .Where(s => s.PhysicianId == physicianId)
                    .Include(s => s.Appointment).ThenInclude(a => a.Patient).ToListAsync(),
                Drugs = await _context.Drugs.Where(d => d.DrugStatus == "Active").ToListAsync()
            };
            return View(vm);
        }

        // create PhysicianAdvice then PhysicianPrescrip
        var advice = new PhysicianAdvice
        {
            ScheduleId = form.ScheduleId,
            Advice = form.Advice,
            Note = form.Note
        };

        _context.PhysicianAdvices.Add(advice);
        await _context.SaveChangesAsync();

        var prescrip = new PhysicianPrescrip
        {
            PhysicianAdviceId = advice.PhysicianAdviceId,
            DrugId = form.DrugId,
            Prescription = form.Prescription,
            Dosage = form.Dosage
        };

        _context.PhysicianPrescrips.Add(prescrip);
        await _context.SaveChangesAsync();

        var isAjaxResp = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        if (isAjaxResp)
        {
            return Json(new { success = true, message = "Prescription created successfully." });
        }

        TempData["SuccessMessage"] = "Prescription created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ViewPrescriptions()
    {
        var physicianId = GetPhysicianId();
        if (physicianId == null) return RedirectToAction("Login", "CrediMgr");

        var prescriptions = await _context.PhysicianPrescrips
            .Include(pp => pp.Drug)
            .Include(pp => pp.PhysicianAdvice)
                .ThenInclude(pa => pa.Schedule)
                    .ThenInclude(s => s.Appointment)
                        .ThenInclude(a => a.Patient)
            .Where(pp => pp.PhysicianAdvice.Schedule.PhysicianId == physicianId)
            .ToListAsync();

        var vm = new PrescriptionListViewModel { Prescriptions = prescriptions };
        return View(vm);
    }

    public async Task<IActionResult> ViewAppointments()
    {
        var physicianId = GetPhysicianId();
        if (physicianId == null) return RedirectToAction("Login", "CrediMgr");

        var schedules = await _context.Schedules
            .Where(s => s.PhysicianId == physicianId)
            .Include(s => s.Appointment)
                .ThenInclude(a => a.Patient)
            .ToListAsync();

        var vm = new AppointmentListViewModel { Schedules = schedules };
        return View(vm);
    }

    public async Task<IActionResult> AppointmentDetails(int id)
    {
        var physicianId = GetPhysicianId();
        if (physicianId == null) return RedirectToAction("Login", "CrediMgr");

        var appointment = await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Schedules)
                .ThenInclude(s => s.PhysicianAdvices)
                    .ThenInclude(pa => pa.PhysicianPrescrips)
                        .ThenInclude(pp => pp.Drug)
            .FirstOrDefaultAsync(a => a.AppointmentId == id);

        if (appointment == null)
        {
            return NotFound();
        }

        // Ensure the current physician is associated with at least one schedule for this appointment
        var hasAccess = appointment.Schedules.Any(s => s.PhysicianId == physicianId);
        if (!hasAccess)
        {
            return Forbid();
        }

        return View(appointment);
    }

    public IActionResult ViewDrugs()
    {
        var drugs = _context.Drugs.ToList();
        return View(drugs);
    }

    // View drug requests made by this physician
    public async Task<IActionResult> ViewDrugRequests()
    {
        var physicianId = GetPhysicianId();
        if (physicianId == null) return RedirectToAction("Login", "CrediMgr");

        var requests = await _context.DrugRequests
            .Where(r => r.PhysicianId == physicianId)
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync();

        var vm = new DrugRequestListViewModel { Requests = requests };
        return View(vm);
    }

    // GET Order Drugs
    public IActionResult OrderDrugs()
    {
        var physicianId = GetPhysicianId();
        if (physicianId == null) return RedirectToAction("Login", "CrediMgr");

        var vm = new OrderDrugsViewModel();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OrderDrugs(OrderDrugsFormModel form)
    {
        var physicianId = GetPhysicianId();
        if (physicianId == null) return RedirectToAction("Login", "CrediMgr");

        if (!ModelState.IsValid)
        {
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var first = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault() ?? "Validation failed.";
                return BadRequest(new { success = false, message = first });
            }

            return View(new OrderDrugsViewModel { Form = form });
        }

        var req = new DrugRequest
        {
            PhysicianId = physicianId.Value,
            DrugsInfoText = form.DrugsInfoText,
            RequestDate = DateTime.UtcNow,
            RequestStatus = "Pending"
        };

        _context.DrugRequests.Add(req);
        await _context.SaveChangesAsync();

        var isAjax2 = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        if (isAjax2)
        {
            return Json(new { success = true, message = "Drug request submitted successfully." });
        }

        TempData["SuccessMessage"] = "Drug request submitted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
