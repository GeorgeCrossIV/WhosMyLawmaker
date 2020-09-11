using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Lawmakers.Models;
using Lawmakers.Services;
using Microsoft.Extensions.Configuration;

namespace Lawmakers.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            var authUrl = _config.GetSection("Astra").GetSection("AuthUrl").Value;
            var username = _config.GetSection("Astra").GetSection("Username").Value;
            var password = _config.GetSection("Astra").GetSection("Password").Value;
            var token = Astra.GetToken(authUrl, username, password);

            if (token.Length > 0)
                ViewBag.Authenticated = true;
            else
                ViewBag.Authenticated = false;

            return View();
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
