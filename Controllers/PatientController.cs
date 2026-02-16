using CAS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult ViewInventory()
        {
            var drugs = _context.Drugs.ToList();
            return View(drugs);
        }


        public IActionResult PhysicianDetails()
        { 
            var username = User.Identity?.Name;
            var user = _context.Users
                .FirstOrDefault(u => u.UserName == username);

            if (user == null)
                return RedirectToAction("Login", "CrediMgr");

            if (user.RoleReferenceId == null)
            {
                ViewBag.Message = "Invalid patient mapping.";
                return View(new List<Physician>());
            }

            int patientId = user.RoleReferenceId.Value;

            var physicians = (
                from a in _context.Appointments
                join s in _context.Schedules on a.AppointmentId equals s.AppointmentId
                join p in _context.Physicians on s.PhysicianId equals p.PhysicianId
                where a.PatientId == patientId
                      && a.ScheduleStatus == "Scheduled"
                      && s.ScheduleStatus == "Completed"
                select p
            ).AsNoTracking().ToList();

            if (!physicians.Any())
                ViewBag.Message = "No physician found.";
            ViewBag.Count = physicians.Count;

            return View(physicians);
        }


        public IActionResult PhysicianAdvice()
        {
            var username = User.Identity?.Name;

            var user = _context.Users
                .FirstOrDefault(u => u.UserName == username);

            if (user == null)
                return RedirectToAction("Login", "CrediMgr");

            if (user.RoleReferenceId == null)
            {
                ViewBag.Message = "Invalid patient mapping.";
                return View();
            }

            int patientId = user.RoleReferenceId.Value;

            var adviceList = (
    from a in _context.Appointments
    join s in _context.Schedules
        on a.AppointmentId equals s.AppointmentId
    join pa in _context.PhysicianAdvices
        on s.ScheduleId equals pa.ScheduleId
    where a.PatientId == patientId
          && a.ScheduleStatus == "Scheduled"
          && s.ScheduleStatus == "Completed"
    select new
    {
        s.ScheduleId,   
        s.ScheduleDate,
        s.ScheduleTime,
        pa.Advice,
        pa.Note
    }
)
.AsNoTracking()
.ToList();


            if (!adviceList.Any())
                ViewBag.Message = "No physician advice found.";

            ViewBag.AdviceList = adviceList;
            ViewBag.Count = adviceList.Count;

            return View();
        }

        public IActionResult ViewPrescription(int scheduleId)
        {
            var prescriptionList = (
                from pa in _context.PhysicianAdvices
                join s in _context.Schedules
                    on pa.ScheduleId equals s.ScheduleId
                join p in _context.Physicians
                    on s.PhysicianId equals p.PhysicianId
                join pp in _context.PhysicianPrescrips
                    on pa.PhysicianAdviceId equals pp.PhysicianAdviceId
                join d in _context.Drugs
                    on pp.DrugId equals d.DrugId
                where pa.ScheduleId == scheduleId
                select new
                {
                    p.PhysicianName,
                    pa.Advice,
                    pa.Note,
                    d.DrugTitle,
                    d.Description,
                    pp.Prescription,
                    pp.Dosage
                }
            )
            .AsNoTracking()
            .ToList();

            if (!prescriptionList.Any())
                ViewBag.Message = "No prescription found.";

            ViewBag.Data = prescriptionList;

            return View();
        }


        public IActionResult MakeAppointment()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MakeAppointment(Appointment appointment, DateTime preferredDate, TimeSpan preferredTime)
        {
            var username = User.Identity?.Name;

            var user = _context.Users
                .FirstOrDefault(u => u.UserName == username);

            if (user == null)
                return RedirectToAction("Login", "CrediMgr");

            int patientId = user.RoleReferenceId.Value;

            DateTime mergedDateTime = preferredDate.Date.Add(preferredTime);

            appointment.PatientId = patientId;
            appointment.AppointmentDate = mergedDateTime;
            appointment.ScheduleStatus = "Pending";

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            // Redirect to success page
            return RedirectToAction("AppointmentSuccess");
        }
        public IActionResult AppointmentSuccess()
        {
            return View();
        }

        public IActionResult CheckAppointment()
        {
            var username = User.Identity?.Name;

            var user = _context.Users
                .FirstOrDefault(u => u.UserName == username);

            if (user == null)
                return RedirectToAction("Login", "CrediMgr");

            int patientId = user.RoleReferenceId.Value;

            var appointmentList = (
                from a in _context.Appointments
                where a.PatientId == patientId
                select new
                {
                    a.AppointmentId,
                    a.Criticality,
                    a.Reason,
                    a.Note,
                    a.ScheduleStatus
                }
            ).ToList();

            var resultList = new List<object>();

            foreach (var a in appointmentList)
            {
                // CASE 1: Pending
                if (a.ScheduleStatus == "Pending")
                {
                    resultList.Add(new
                    {
                        a.Criticality,
                        a.Reason,
                        a.Note,
                        Status = "Yet to assign an appointment",
                        ScheduleDate = (DateOnly?)null,
                        ScheduleTime = (TimeOnly?)null,
                        PhysicianName = (string?)null
                    });
                }

                // CASE 2: Scheduled
                else if (a.ScheduleStatus == "Scheduled")
                {
                    var schedule = _context.Schedules
                        .FirstOrDefault(s => s.AppointmentId == a.AppointmentId);

                    if (schedule != null)
                    {
                        var physician = _context.Physicians
                            .FirstOrDefault(p => p.PhysicianId == schedule.PhysicianId);

                        resultList.Add(new
                        {
                            a.Criticality,
                            a.Reason,
                            a.Note,
                            Status = schedule.ScheduleStatus,
                            ScheduleDate = schedule.ScheduleDate,
                            ScheduleTime = schedule.ScheduleTime,
                            PhysicianName = physician != null ? physician.PhysicianName : "Not Assigned"
                        });
                    }
                }
            }

            if (!resultList.Any())
                ViewBag.Message = "No appointments found.";

            ViewBag.Data = resultList;

            return View();
        }




    }


}

