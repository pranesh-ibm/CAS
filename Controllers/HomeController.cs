using CAS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CAS.Controllers
{
    public class HomeController : Controller
    {
        private readonly CasContext _context;

        public HomeController(CasContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var drugs = _context.Drugs.ToList();
            return View(drugs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
