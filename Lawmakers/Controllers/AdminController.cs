using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lawmakers.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc;

namespace Lawmakers.Controllers
{
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _host;

        public AdminController(IWebHostEnvironment host)
        {
            _host = host;
        }
        public IActionResult Index()
        {
            return View();
        }

    }
}
