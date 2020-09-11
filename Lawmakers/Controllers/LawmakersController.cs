using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lawmakers.Models;
using Lawmakers.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Lawmakers.Controllers
{
    public class LawmakersController : Controller
    {
        private IConfiguration _config;
        private readonly IWebHostEnvironment _host;
        private string _Token="";

        public LawmakersController(IWebHostEnvironment host, IConfiguration config)
        {
            _config = config;
            _host = host;
        }

        public IActionResult Index()
        {
            List<LawmakerDocument> lawmakerDocuments = new List<LawmakerDocument>();

            // get a list of Lawmakers. Currently don't have a method for 
            // retrieving a list of lawmakers via a query, so I'll just pull the first 537
            try
            {
                for (int id = 1; id < 10; id++)
                {
                    lawmakerDocuments.Add(Services.Astra.GetLawmaker(_config, GetToken(), id));
                }
                return View(lawmakerDocuments);

            }
            catch (Exception ex)
            {
                return View();
            }        
        }

        public IActionResult Details(int id)
        {
            // get the lawmaker document
            var lawmakerDocument = Services.Astra.GetLawmaker(_config, GetToken(), id);
            return View(lawmakerDocument);
        }

        public IActionResult LoadLawmakers()
        {
            Astra.LoadLawmakers(_host, _config);

            return View();
        }

        private string GetToken()
        {
            if (_Token.Length == 0)
            {
                var authUrl = _config.GetSection("Astra").GetSection("AuthUrl").Value;
                var username = _config.GetSection("Astra").GetSection("Username").Value;
                var password = _config.GetSection("Astra").GetSection("Password").Value;

                _Token = Services.Astra.GetToken(authUrl, username, password);
            }

            return _Token;
        }

    }
}
