using CAS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace CAS.Controllers
{
    [Authorize(Roles = "Chemist")]
    public class ChemistController : Controller
    {
        private readonly CasContext _context;

        public ChemistController(CasContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var username = User.Identity?.Name;

            var user = _context.Users
                .FirstOrDefault(u => u.UserName == username);

            if (user == null)
                return RedirectToAction("Login", "CrediMgr");

            var chemist = _context.Chemists
                .FirstOrDefault(c => c.ChemistId == user.RoleReferenceId);


            if (chemist == null)
            {
                return Content("Chemist profile not found. Please contact admin.");
            }

            return View(chemist);
        }
        // GET: Edit Profile
        [HttpGet]
        public IActionResult EditProfile()
        {
            var username = User.Identity?.Name;

            var user = _context.Users
                .FirstOrDefault(u => u.UserName == username);

            if (user == null)
                return RedirectToAction("Login", "CrediMgr");

            var chemist = _context.Chemists
                .FirstOrDefault(c => c.ChemistId == user.RoleReferenceId);

            return View(chemist);
        }


        public IActionResult ViewInventory()
        {
            var drugs = _context.Drugs.ToList();
            return View(drugs);
        }

        public IActionResult OrderDrug()
        {
            ViewBag.Drugs = _context.Drugs.ToList();
            ViewBag.Suppliers = _context.Suppliers.ToList();
            return View();
        }
        public IActionResult DrugRequests()
        {
            var requests = _context.DrugRequests
                .OrderByDescending(r => r.RequestDate)
                .ToList();

            return View(requests);
        }

        [HttpPost]
        public IActionResult EditProfile(Chemist model)
        {
            var chemist = _context.Chemists
                .FirstOrDefault(c => c.ChemistId == model.ChemistId);

            if (chemist != null)
            {
                chemist.Email = model.Email;
                chemist.Address = model.Address;
                chemist.Phone = model.Phone;

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult OrderDrug(int SupplierId, List<int> DrugIds, List<int> Quantities)
        {
            int count = _context.PurchaseOrderHeaders.Count() + 1;
            string poNumber = "PO-00" + count;

            var orderHeader = new PurchaseOrderHeader
            {
                Pono = poNumber,
                Podate = DateOnly.FromDateTime(DateTime.Now),
                SupplierId = SupplierId,
                PoStatus = "Pending" 
            };

            _context.PurchaseOrderHeaders.Add(orderHeader);
            _context.SaveChanges();

            for (int i = 0; i < DrugIds.Count; i++)
            {
                var productLine = new PurchaseProductLine
                {
                    Poid = orderHeader.Poid,
                    DrugId = DrugIds[i],
                    Qty = Quantities[i],
                   
                };

                _context.PurchaseProductLines.Add(productLine);
            }

            _context.SaveChanges();

            return RedirectToAction("MyOrders");
        }





        public IActionResult MyOrders()
        {
            var orders = _context.PurchaseOrderHeaders
                .Select(po => new
                {
                    po.Poid,
                    po.Pono,
                    po.Podate,
                    po.PoStatus,
                    SupplierName = po.Supplier.SupplierName, 
                    Items = po.PurchaseProductLines
                        .Select(p => new
                        {
                            DrugName = p.Drug.DrugTitle,  
                            p.Qty
                        }).ToList()
                })
                .ToList();

            return View(orders);
        }

        [HttpPost]
        public IActionResult OrderDrugs(DrugRequest order)
        {
    
            order.PhysicianId = 1; 

            order.RequestStatus = "Pending";
            order.RequestDate = DateTime.Now;

            _context.DrugRequests.Add(order);
            _context.SaveChanges();

            return RedirectToAction("MyOrders");
        }
        public IActionResult Approve(int id)
        {
            var request = _context.DrugRequests.Find(id);

            if (request != null)
            {
                request.RequestStatus = "Approved";
                _context.SaveChanges();
            }

            return RedirectToAction("DrugRequests");
        }
        public IActionResult Reject(int id)
        {
            var request = _context.DrugRequests.Find(id);

            if (request != null)
            {
                request.RequestStatus = "Rejected";
                _context.SaveChanges();
            }

            return RedirectToAction("DrugRequests");
        }







    }

}
