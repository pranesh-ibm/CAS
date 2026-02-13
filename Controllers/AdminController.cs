using CAS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CAS.Controllers
{
    public class AdminController : Controller
    {
        private readonly CasContext _context;

        public AdminController(CasContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: patients detail
        public async Task<IActionResult> GetPatientsDetail()
        {
            return View(await _context.Patients.ToListAsync());
        }

        // GET: patient/Details/
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

        public IActionResult CreatePatient()
        {
            return View();
        }

        // POST: temp/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePatient([Bind("PatientId,PatientName,Dob,Gender,Address,Phone,Email,Summary,PatientStatus")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetPatientDetails));
            }
            return View(patient);
        }

        // GET: temp/Edit/5
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

        // POST: temp/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(int id, [Bind("PatientId,PatientName,Dob,Gender,Address,Phone,Email,Summary,PatientStatus")] Patient patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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

        // GET: temp/Delete/5
        public async Task<IActionResult> DeletePatient(int? id)
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

        // POST: temp/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient != null)
            {
                patient.PatientStatus = "Inactive";   // Change status instead of deleting
                                                      // _context.Update(patient);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(GetPatientsDetail));
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }

      ////
      ///AdminPhysician

        public async Task<IActionResult> GetPhysiciansDetail()
        {
            return View(await _context.Physicians.ToListAsync());
        }

        // GET: Physicians/Details/5
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

        // GET: Physicians/Create
        public IActionResult CreatePhysician()
        {
            return View();
        }

        // POST: Physicians/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePhysician([Bind("PhysicianId,PhysicianName,Specialization,Address,Phone,Email,Summary,PhysicianStatus")] Physician physician)
        {
            if (ModelState.IsValid)
            {
                _context.Add(physician);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetPhysiciansDetail));
            }
            return View(physician);
        }

        // GET: Physicians/Edit/5
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

        // POST: Physicians/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPhysician(int id, [Bind("PhysicianId,PhysicianName,Specialization,Address,Phone,Email,Summary,PhysicianStatus")] Physician physician)
        {
            if (id != physician.PhysicianId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(physician);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhysicianExists(physician.PhysicianId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(GetPhysiciansDetail));
            }
            return View(physician);
        }

        // GET: Physicians/Delete/5
        public async Task<IActionResult> DeletePhysician(int? id)
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

        // POST: Physicians/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePhysician(int id)
        {
            var physician = await _context.Physicians.FindAsync(id);
            if (physician != null)
            {
                physician.PhysicianStatus = "Inactive";   
                                                     
                await _context.SaveChangesAsync();
            }

            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetPhysiciansDetail));
        }


       
        private bool PhysicianExists(int id)
        {
            return _context.Physicians.Any(e => e.PhysicianId == id);
        }
        /////////////////
        ///



        public async Task<IActionResult> GetChemistsDetail()
        {
            return View(await _context.Chemists.ToListAsync());
        }

        // GET: Chemists/Details/5
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

        // GET: Chemists/Create
        public IActionResult CreateChemist()
        {
            return View();
        }

        // POST: Chemists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateChemist([Bind("ChemistId,ChemistName,Address,Phone,Email,Summary,ChemistStatus")] Chemist chemist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chemist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetChemistsDetail));
            }
            return View(chemist);
        }

        // GET: Chemists/Edit/5
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

        // POST: Chemists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditChemist(int id, [Bind("ChemistId,ChemistName,Address,Phone,Email,Summary,ChemistStatus")] Chemist chemist)
        {
            if (id != chemist.ChemistId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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

        // GET: Chemists/Delete/5
        public async Task<IActionResult> DeleteChemist(int? id)
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

        // POST: Chemists/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteChemist(int id)
        {
            var chemist = await _context.Chemists.FindAsync(id);
            if (chemist != null)
            {
                chemist.ChemistStatus = "Inactive";

                await _context.SaveChangesAsync();
            }

           
            return RedirectToAction(nameof(GetChemistsDetail));
        }

        private bool ChemistExists(int id)
        {
            return _context.Chemists.Any(e => e.ChemistId == id);
        }

        /////////////////////////
        ///SupplierAdmin
        ///

        public async Task<IActionResult> GetSuppliersDetail()
        {
            return View(await _context.Suppliers.ToListAsync());
        }

        // GET: Suppliers/Details/5
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

        // GET: Suppliers/Create
        public IActionResult CreateSupplier()
        {
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSupplier([Bind("SupplierId,SupplierName,Address,Phone,Email,SupplierStatus")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetSuppliersDetail));
            }
            return View(supplier);
        }

        // GET: Suppliers/Edit/5
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

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupplier(int id, [Bind("SupplierId,SupplierName,Address,Phone,Email,SupplierStatus")] Supplier supplier)
        {
            if (id != supplier.SupplierId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> DeleteSupplier(int? id)
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

        // POST: Suppliers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                supplier.SupplierStatus = "Inactive";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(GetSuppliersDetail));
        }

        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }

    }
}
