using CAS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CAS.Controllers
{
    [Authorize(Roles = "Supplier")]
    public class SupplierController : Controller
    {
        private readonly CasContext _context;

        public SupplierController(CasContext context)
        {
            _context = context;
        }

        // 🟢 Supplier Profile Page (Like Chemist)
        public IActionResult Index()
        {
            var username = User.Identity?.Name;

            var user = _context.Users
                .FirstOrDefault(u => u.UserName == username);

            if (user == null)
                return RedirectToAction("Login", "CrediMgr");

            var supplier = _context.Suppliers
                .FirstOrDefault(s => s.SupplierId == user.RoleReferenceId);

            return View(supplier);
        }
        public IActionResult EditProfile()
        {
            var username = User.Identity?.Name;

            var user = _context.Users
                .FirstOrDefault(u => u.UserName == username);

            var supplier = _context.Suppliers
                .FirstOrDefault(s => s.SupplierId == user.RoleReferenceId);

            return View(supplier);
        }


        public IActionResult PendingOrders()
        {
            var username = User.Identity?.Name;

            var user = _context.Users
                .FirstOrDefault(u => u.UserName == username);

            var orders = _context.PurchaseOrderHeaders
                .Where(o => o.SupplierId == user.RoleReferenceId
                         && o.PoStatus == "Pending")
                .Include(o => o.PurchaseProductLines)
                    .ThenInclude(p => p.Drug)
                .ToList();

            return View(orders);
        }


        // 🟢 Order History (Approved + Rejected)

  
         public IActionResult OrderHistory()
        {
            var username = User.Identity?.Name;
            var user = _context.Users
                .FirstOrDefault(u => u.UserName == username);
            var orders = _context.PurchaseOrderHeaders
                .Where(o => o.SupplierId == user.RoleReferenceId
                && o.PoStatus != "Pending")
                .Include(o => o.PurchaseProductLines)
                .ThenInclude(p => p.Drug)
                .ToList();

            return View(orders);
         }





    // ✅ Approve
    public IActionResult Approve(int id)
        {
            var order = _context.PurchaseOrderHeaders.Find(id);

            if (order != null)
            {
                order.PoStatus = "Approved";
                _context.SaveChanges();
            }

            return RedirectToAction("PendingOrders");
        }

        // ❌ Reject
        public IActionResult Reject(int id)
        {
            var order = _context.PurchaseOrderHeaders.Find(id);

            if (order != null)
            {
                order.PoStatus = "Rejected";
                _context.SaveChanges();
            }

            return RedirectToAction("PendingOrders");
        }
        [HttpPost]
        public IActionResult EditProfile(Supplier model)
        {
            var supplier = _context.Suppliers.Find(model.SupplierId);

            if (supplier != null)
            {
                supplier.Email = model.Email;
                supplier.Phone = model.Phone;
                supplier.Address = model.Address;

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }


    }
}
