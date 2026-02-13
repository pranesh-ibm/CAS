using CAS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CAS_Project.Controllers
{
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

        [HttpPost]
        [HttpPost]
        [HttpPost]
        public IActionResult OrderDrug(int SupplierId, List<int> DrugIds, List<int> Quantities)
        {
            int count = _context.PurchaseOrderHeaders.Count() + 1;
            string poNumber = "PO-00" + count;

            var orderHeader = new PurchaseOrderHeader
            {
                Pono = poNumber,
                Podate = DateOnly.FromDateTime(DateTime.Now),
                SupplierId = SupplierId
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




    }

}
