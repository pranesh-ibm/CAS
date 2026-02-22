
using CAS.Models;
using CAS.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
namespace Medi_Clinic.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private readonly CasContext _context;
        public AdminController(CasContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalPatients = await _context.Patients.CountAsync(),
                TotalPhysicians = await _context.Physicians.CountAsync(),
                TotalChemists = await _context.Chemists.CountAsync(),
                TotalSuppliers = await _context.Suppliers.CountAsync(),
                TodayAppointments = await _context.Appointments.CountAsync(),

                PendingPatients = await _context.Patients.CountAsync(p => p.PatientStatus == "Pending"),

                PendingAppointments = await _context.Appointments.CountAsync(a => a.ScheduleStatus == "Pending"),

                TotalAppointments = await _context.Schedules.CountAsync(s => s.ScheduleDate == DateOnly.FromDateTime(DateTime.Today))

            };
            return View(model);
        }

        //  GET: AdminPatient
        public async Task<IActionResult> GetPatientsDetail()
        {
            return View(await _context.Patients.ToListAsync());
        }

        // GET: AdminPatient/Details/5
        public async Task<IActionResult> GetPatientDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: AdminPatient/Create
        public IActionResult CreatePatient()
        {
            return View();
        }

        // POST: AdminPatient/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePatient(
    [Bind("PatientId,PatientName,Dob,Gender,Address,Phone,Email,Summary")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                patient.PatientStatus = "Active";
                _context.Add(patient);
                await _context.SaveChangesAsync();

                // Generate password
                string lastFour = patient.Phone!.Substring(patient.Phone.Length - 4);
                string generatedPassword = patient.PatientName + "@" + lastFour;

                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == patient.Email);

                if (existingUser == null)
                {
                    var user = new User
                    {
                        UserName = patient.Email!,
                        Password = generatedPassword,
                        Role = "Patient",
                        RoleReferenceId = patient.PatientId,
                        Status = patient.PatientStatus
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(GetPatientsDetail));
            }

            return View(patient);
        }


        public async Task<IActionResult> PendingPatients()
        {
            var pendingPatients = await _context.Patients
                .Where(p => p.PatientStatus == "Pending")
                .ToListAsync();

            return View(pendingPatients);
        }
        public async Task<IActionResult> Approve(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
                return NotFound();

            // Change status
            patient.PatientStatus = "Active";

            // Generate password
            string lastFour = patient.Phone!.Substring(patient.Phone.Length - 4);
            string generatedPassword = patient.PatientName + "@" + lastFour;

            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == patient.Email);

            if (existingUser == null)
            {
                var user = new User
                {
                    UserName = patient.Email!,
                    Password = generatedPassword,
                    Role = "Patient",
                    RoleReferenceId = patient.PatientId,
                    Status = patient.PatientStatus

                };

                _context.Users.Add(user);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PendingPatients));
        }
        public async Task<IActionResult> Deny(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
                return NotFound();

            patient.PatientStatus = "Inactive";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PendingPatients));
        }



        // GET: AdminPatient/Edit/5
        public async Task<IActionResult> EditPatient(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // POST: AdminPatient/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(
    int id,
    [Bind("PatientId,PatientName,Dob,Gender,Address,Phone,Email,Summary,PatientStatus")] Patient patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get existing patient from DB
                    var existingPatient = await _context.Patients
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.PatientId == id);

                    if (existingPatient == null)
                        return NotFound();

                    // Check if Email changed
                    if (existingPatient.Email != patient.Email)
                    {
                        var user = await _context.Users
                            .FirstOrDefaultAsync(u => u.UserName == existingPatient.Email);

                        if (user != null)
                        {
                            user.UserName = patient.Email!;
                        }
                    }

                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(GetPatientsDetail));
            }

            return View(patient);
        }


        public async Task<IActionResult> DeactivatePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
                return NotFound();

            patient.PatientStatus = "Inactive";

            // Also update user table
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RoleReferenceId == id && u.Role == "Patient");

            if (user != null)
                user.Status = "Inactive";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GetPatientsDetail));
        }

        public async Task<IActionResult> ActivatePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
                return NotFound();

            patient.PatientStatus = "Active";

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RoleReferenceId == id && u.Role == "Patient");

            if (user != null)
                user.Status = "Active";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GetPatientsDetail));
        }



        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }

        // GET: AdminPhysician
        public async Task<IActionResult> GetPhysiciansDetail()
        {
            return View(await _context.Physicians.ToListAsync());
        }

        // GET: AdminPhysician/Details/5
        public async Task<IActionResult> GetPhysicianDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physician = await _context.Physicians
                .FirstOrDefaultAsync(m => m.PhysicianId == id);
            if (physician == null)
            {
                return NotFound();
            }

            return View(physician);
        }

        // GET: AdminPhysician/Create
        public IActionResult CreatePhysician()
        {
            return View();
        }

        // POST: AdminPhysician/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePhysician([Bind("PhysicianId,PhysicianName,Specialization,Address,Phone,Email,Summary,PhysicianStatus")] Physician physician)
        {
            if (ModelState.IsValid)
            {
                // Change status
                physician.PhysicianStatus = "Active";
                _context.Add(physician);
                await _context.SaveChangesAsync();
                // Generate password
                string lastFour = physician.Phone!.Substring(physician.Phone.Length - 4);
                string generatedPassword = physician.PhysicianName + "@" + lastFour;

                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == physician.Email);

                if (existingUser == null)
                {
                    var user = new User
                    {
                        UserName = physician.Email!,
                        Password = generatedPassword,
                        Role = "Physician",
                        RoleReferenceId = physician.PhysicianId,
                        Status = physician.PhysicianStatus

                    };

                    _context.Users.Add(user);
                }



                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetPhysiciansDetail));
            }
            return View(physician);
            //return RedirectToAction(nameof(GetPhysiciansDetail));
        }

        // GET: AdminPhysician/Edit/5
        public async Task<IActionResult> EditPhysician(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physician = await _context.Physicians.FindAsync(id);
            if (physician == null)
            {
                return NotFound();
            }
            return View(physician);
        }

        // POST: AdminPhysician/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPhysician(
    int id,
    [Bind("PhysicianId,PhysicianName,Specialization,Address,Phone,Email,Summary,PhysicianStatus")] Physician physician)
        {
            if (id != physician.PhysicianId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 1️⃣ Get existing physician (no tracking)
                    var existingPhysician = await _context.Physicians
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.PhysicianId == id);

                    if (existingPhysician == null)
                        return NotFound();

                    // 2️⃣ If Email changed → update User table
                    if (existingPhysician.Email != physician.Email)
                    {
                        var user = await _context.Users
                            .FirstOrDefaultAsync(u =>
                                u.Role == "Physician" &&
                                u.RoleReferenceId == physician.PhysicianId);

                        if (user != null)
                        {
                            user.UserName = physician.Email!;
                        }
                    }

                    // 3️⃣ Update Physician
                    _context.Update(physician);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhysicianExists(physician.PhysicianId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(GetPhysiciansDetail));
            }

            return View(physician);
        }


      
        public async Task<IActionResult> TogglePhysicianStatus(int id)
        {
            var physician = await _context.Physicians.FindAsync(id);

            if (physician == null)
                return NotFound();

            // Toggle status
            physician.PhysicianStatus =
                physician.PhysicianStatus == "Active" ? "Inactive" : "Active";

            // Update Users table
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RoleReferenceId == id && u.Role == "Physician");

            if (user != null)
                user.Status = physician.PhysicianStatus;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GetPhysiciansDetail));
        }


        private bool PhysicianExists(int id)
        {
            return _context.Physicians.Any(e => e.PhysicianId == id);
        }

        // GET: AdminChemist
        public async Task<IActionResult> GetChemistsDetail()
        {
            return View(await _context.Chemists.ToListAsync());
        }

        // GET: AdminChemist/Details/5
        public async Task<IActionResult> GetChemistDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemist = await _context.Chemists
                .FirstOrDefaultAsync(m => m.ChemistId == id);
            if (chemist == null)
            {
                return NotFound();
            }

            return View(chemist);
        }

        // GET: AdminChemist/Create
        public IActionResult CreateChemist()
        {
            return View();
        }

        // POST: AdminChemist/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateChemist(
    [Bind("ChemistId,ChemistName,Address,Phone,Email,Summary","ChemistStatus")] Chemist chemist)
        {
            if (ModelState.IsValid)
            {
                // 1️⃣ Set Status
                chemist.ChemistStatus = "Active";

                // 2️⃣ Save Chemist
                _context.Add(chemist);
                await _context.SaveChangesAsync();

                // 3️⃣ Generate Password
                string lastFour = chemist.Phone!.Substring(chemist.Phone.Length - 4);
                string generatedPassword = chemist.ChemistName + "@" + lastFour;

                // 4️⃣ Check if User already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == chemist.Email);

                if (existingUser == null)
                {
                    var user = new User
                    {
                        UserName = chemist.Email!,
                        Password = generatedPassword,
                        Role = "Chemist",
                        RoleReferenceId = chemist.ChemistId,
                        Status = chemist.ChemistStatus
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(GetChemistsDetail));
            }

            return View(chemist);
        }
        public async Task<IActionResult> EditChemist(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemist = await _context.Chemists.FindAsync(id);
            if (chemist == null)
            {
                return NotFound();
            }
            return View(chemist);
        }

        // POST: AdminChemist/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditChemist(
    int id,
    [Bind("ChemistId,ChemistName,Address,Phone,Email,Summary,ChemistStatus")] Chemist chemist)
        {
            if (id != chemist.ChemistId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 1️⃣ Get existing chemist without tracking
                    var existingChemist = await _context.Chemists
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.ChemistId == id);

                    if (existingChemist == null)
                        return NotFound();

                    // 2️⃣ If Email changed → update Users table
                    if (existingChemist.Email != chemist.Email)
                    {
                        var user = await _context.Users
                            .FirstOrDefaultAsync(u =>
                                u.Role == "Chemist" &&
                                u.RoleReferenceId == chemist.ChemistId);

                        if (user != null)
                        {
                            user.UserName = chemist.Email!;
                        }
                    }

                    // 3️⃣ Update Chemist table
                    _context.Update(chemist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChemistExists(chemist.ChemistId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(GetChemistsDetail));
            }

            return View(chemist);
        }


        // GET: AdminChemist/Delete/5
        //public async Task<IActionResult> DeleteChemist(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var chemist = await _context.Chemists
        //        .FirstOrDefaultAsync(m => m.ChemistId == id);
        //    if (chemist == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(chemist);
        //}

        //// POST: AdminChemist/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteChemist(int id)
        //{
        //    var chemist = await _context.Chemists.FindAsync(id);
        //    if (chemist != null)
        //    {
        //        chemist.ChemistStatus = "Inactive";
        //        _context.Chemists.Update(chemist);
        //        await _context.SaveChangesAsync();
        //        var user = await _context.Users
        //    .FirstOrDefaultAsync(u => u.UserName == chemist.Email);

        //        if (user != null)
        //        {
        //            user.Status = "Inactive";
        //        }
        //        await _context.SaveChangesAsync();
        //    }



        //    return RedirectToAction(nameof(GetChemistsDetail));
        //}

        public async Task<IActionResult> ToggleChemistStatus(int id)
        {
            var chemist = await _context.Chemists.FindAsync(id);

            if (chemist == null)
                return NotFound();

            // Toggle status
            chemist.ChemistStatus =
                chemist.ChemistStatus == "Active" ? "Inactive" : "Active";

            // Update Users table
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RoleReferenceId == id && u.Role == "Chemist");

            if (user != null)
                user.Status = chemist.ChemistStatus;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GetChemistsDetail));
        }

        private bool ChemistExists(int id)
        {
            return _context.Chemists.Any(e => e.ChemistId == id);
        }

        // GET: AdminSupplier
        public async Task<IActionResult> GetSuppliersDetail()
        {
            return View(await _context.Suppliers.ToListAsync());
        }

        // GET: AdminSupplier/Details/5
        public async Task<IActionResult> GetSupplierDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // GET: AdminSupplier/Create
        public IActionResult CreateSupplier()
        {
            return View();
        }

        // POST: AdminSupplier/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSupplier(
    [Bind("SupplierId,SupplierName,Address,Phone,Email,SupplierStatus")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                // 1️⃣ Set Status
                supplier.SupplierStatus = "Active";

                // 2️⃣ Save Supplier
                _context.Add(supplier);
                await _context.SaveChangesAsync();

                // 3️⃣ Generate Password
                string lastFour = supplier.Phone!.Substring(supplier.Phone.Length - 4);
                string generatedPassword = supplier.SupplierName + "@" + lastFour;

                // 4️⃣ Check if User already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == supplier.Email);

                if (existingUser == null)
                {
                    var user = new User
                    {
                        UserName = supplier.Email!,
                        Password = generatedPassword,
                        Role = "Supplier",
                        RoleReferenceId = supplier.SupplierId,
                        Status = supplier.SupplierStatus
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(GetSuppliersDetail));
            }

            return View(supplier);
        }
        public async Task<IActionResult> EditSupplier(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: AdminSupplier/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupplier(
    int id,
    [Bind("SupplierId,SupplierName,Address,Phone,Email,SupplierStatus")] Supplier supplier)
        {
            if (id != supplier.SupplierId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 1️⃣ Get existing supplier without tracking
                    var existingSupplier = await _context.Suppliers
                        .AsNoTracking()
                        .FirstOrDefaultAsync(s => s.SupplierId == id);

                    if (existingSupplier == null)
                        return NotFound();

                    // 2️⃣ If Email changed → update Users table
                    if (existingSupplier.Email != supplier.Email)
                    {
                        var user = await _context.Users
                            .FirstOrDefaultAsync(u =>
                                u.Role == "Supplier" &&
                                u.RoleReferenceId == supplier.SupplierId);

                        if (user != null)
                        {
                            user.UserName = supplier.Email!;
                        }
                    }

                    // 3️⃣ Update Supplier table
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.SupplierId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(GetSuppliersDetail));
            }

            return View(supplier);
        }


        // GET: AdminSupplier/Delete/5
        public async Task<IActionResult> ToggleSupplierStatus(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null)
                return NotFound();

            // Toggle status
            supplier.SupplierStatus =
                supplier.SupplierStatus == "Active" ? "Inactive" : "Active";

            // Update user table
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RoleReferenceId == id && u.Role == "Supplier");

            if (user != null)
                user.Status = supplier.SupplierStatus;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GetSuppliersDetail));
        }


        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }

        
        // GET: AdminAppointment
        public async Task<IActionResult> GetAppointmentDetails()
        {
            var pendingAppointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.ScheduleStatus != "Pending")
                .ToListAsync();

            return View(pendingAppointments);
        }


        // GET: AdminAppointment/Details/5
        public async Task<IActionResult> GetAppointmentDetailsById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        public async Task<IActionResult> PendingAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.ScheduleStatus == "Pending")
                .ToListAsync();

            return View(appointments);
        }

        


        // GET: AdminSchedules
        public async Task<IActionResult> GetScheduleDetails()
        {
            var mediCure1Context = _context.Schedules
                .Include(s => s.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(s => s.Physician);

            return View(await mediCure1Context.ToListAsync());
        }


        // GET: AdminSchedules/Details/5
        public async Task<IActionResult> GetScheduleById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.Appointment)
                .Include(s => s.Physician)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // GET: AdminSchedules/Create
        public async Task<IActionResult> CreateSchedule(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
                return NotFound();

            ViewBag.Physicians = new SelectList(
                _context.Physicians
                    .Where(p => p.PhysicianStatus == "Active"),
                "PhysicianId",
                "PhysicianName");

            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> AssignDoctorPost(
            int AppointmentID,
            int PhysicianID,
            DateTime ScheduleDate,
            TimeSpan ScheduleTime)
            {
            // 1️⃣ Insert into Schedule table
            var schedule = new Schedule
            {
                AppointmentId = AppointmentID,
                PhysicianId = PhysicianID,
                ScheduleDate = DateOnly.FromDateTime(ScheduleDate),
                ScheduleTime = TimeOnly.FromTimeSpan(ScheduleTime),

                ScheduleStatus = "Scheduled"
            };

            _context.Schedules.Add(schedule);

            // 2️⃣ Update Appointment Status
            var appointment = await _context.Appointments.FindAsync(AppointmentID);

            appointment.ScheduleStatus = "Scheduled";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PendingAppointments));
        }


        // GET: AdminSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId", schedule.AppointmentId);
            ViewData["PhysicianId"] = new SelectList(_context.Physicians, "PhysicianId", "Address", schedule.PhysicianId);
            return View(schedule);
        }

        // POST: AdminSchedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScheduleId,PhysicianId,AppointmentId,ScheduleDate,ScheduleTime,ScheduleStatus")] Schedule schedule)
        {
            if (id != schedule.ScheduleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.ScheduleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId", schedule.AppointmentId);
            ViewData["PhysicianId"] = new SelectList(_context.Physicians, "PhysicianId", "Address", schedule.PhysicianId);
            return View(schedule);
        }

        // GET: AdminSchedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.Appointment)
                .Include(s => s.Physician)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: AdminSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedules.Any(e => e.ScheduleId == id);
        }
    }
}
