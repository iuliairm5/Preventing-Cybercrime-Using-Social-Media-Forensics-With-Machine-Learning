using DisertatieIRIMIA.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace DisertatieIRIMIA.Controllers
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

        [HttpGet]
        public IActionResult Login()
        {
            // return Redirect("/Identity/Account/Login");
            return Redirect("/MyProfile");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return Redirect("/Identity/Account/Register");
        }


    }
}