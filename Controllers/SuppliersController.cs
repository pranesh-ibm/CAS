using CAS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CAS_Project.Controllers
{
    public class SupplierController : Controller
    {
        private readonly CasContext _context;

        public SupplierController(CasContext context)
        {
            _context = context;
        }

        // 1️⃣ View Pending Orders
        public IActionResult PendingOrders()
        {
            var orders = _context.DrugRequests
                .Where(o => o.RequestStatus == "Pending")
                .OrderByDescending(o => o.RequestDate)
                .ToList();

            return View(orders);
        }

        // 2️⃣ Approve Order
        public IActionResult Approve(int id)
        {
            var order = _context.DrugRequests.Find(id);

            if (order != null)
            {
                order.RequestStatus = "Approved";
                _context.SaveChanges();
            }

            return RedirectToAction("PendingOrders");
        }

        // 3️⃣ Reject Order
        public IActionResult Reject(int id)
        {
            var order = _context.DrugRequests.Find(id);

            if (order != null)
            {
                order.RequestStatus = "Rejected";
                _context.SaveChanges();
            }

            return RedirectToAction("PendingOrders");
        }

        // 4️⃣ Order History
        public IActionResult OrderHistory()
        {
            var orders = _context.DrugRequests
                .Where(o => o.RequestStatus != "Pending")
                .OrderByDescending(o => o.RequestDate)
                .ToList();

            return View(orders);
        }
        public IActionResult Dashboard()
        {
            var totalOrders = _context.DrugRequests.Count();
            var pendingOrders = _context.DrugRequests
                .Count(o => o.RequestStatus == "Pending");
            var approvedOrders = _context.DrugRequests
                .Count(o => o.RequestStatus == "Approved");
            var rejectedOrders = _context.DrugRequests
                .Count(o => o.RequestStatus == "Rejected");

            ViewBag.TotalOrders = totalOrders;
            ViewBag.PendingOrders = pendingOrders;
            ViewBag.ApprovedOrders = approvedOrders;
            ViewBag.RejectedOrders = rejectedOrders;

            return View();
        }

    }
}
