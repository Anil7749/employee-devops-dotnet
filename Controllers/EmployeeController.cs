using Microsoft.AspNetCore.Mvc;
using employee_webapp.Models;

namespace employee_webapp.Controllers
{
    public class EmployeeController : Controller
    {
        static List<Employee> employees = new List<Employee>();

        public IActionResult Index()
        {
            return View(employees);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee emp)
        {
            employees.Add(emp);
            return RedirectToAction("Index");
        }
    }
}