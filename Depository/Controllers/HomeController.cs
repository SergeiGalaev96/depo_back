using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Depository.Models;

using Depository.DAL.DbContext;
using Depository.DAL;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Depository.Core.Models;

namespace Depository.Controllers
{
    public class HomeController : Controller
    {
        

        private readonly ApplicationDbContext _context;

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;

        public HomeController(ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
        }
        public ActionResult<IEnumerable<string>> Index()
        {
            var num = new System.Random().Next(9000);

            return new string[] { "value1", "value2" };
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
