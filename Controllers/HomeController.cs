using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EIAS.Models;

namespace EIAS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(int householdSize, decimal electricity, decimal naturalGas, decimal coal, decimal solar, decimal petroleum, decimal wind)
        {
            // Basic validation (optional)
            if (householdSize <= 0)
            {
                ModelState.AddModelError("HouseholdSize", "Household size must be greater than zero.");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            // Process the data and calculate the environmental impact
            // ...

            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Handle registration logic here
                return RedirectToAction("Index", "Home");
            }

            return View(model);
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