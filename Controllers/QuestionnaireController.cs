
using EIAS.Models;
using Microsoft.AspNetCore.Mvc;

namespace EIAS.Controllers
{
    public class QuestionnaireController : Controller
    {
        public IActionResult Index()
        {
            return View(new Questionnaire());
        }

        [HttpPost]
        public IActionResult SaveHousehold(Questionnaire model)
        {
            if (ModelState.IsValid)
            {
                // Handle Household data
                TempData["Questionnaire"] = model; // Store model temporarily
                return RedirectToAction("Car");
            }

            return View("Index", model);
        }

        public IActionResult Car()
        {
            var model = TempData["Questionnaire"] as Questionnaire ?? new Questionnaire();
            return View(model);
        }

        [HttpPost]
        public IActionResult SaveCar(Questionnaire model)
        {
            if (ModelState.IsValid)
            {
                // Handle Car data
                TempData["Questionnaire"] = model; // Store model temporarily
                return RedirectToAction("Travel");
            }

            return View("Car", model);
        }

        public IActionResult Travel()
        {
            var model = TempData["Questionnaire"] as Questionnaire ?? new Questionnaire();
            return View(model);
        }

        [HttpPost]
        public IActionResult SaveTravel(Questionnaire model)
        {
            if (ModelState.IsValid)
            {
                // Handle Travel data
                // Save all data to the database or process as needed
                return RedirectToAction("Success");
            }

            return View("Travel", model);
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
