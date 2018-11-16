using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SqlServerEFCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index([FromServices] Test1Context context1, [FromServices] Test2Context context2)
        {
            var connection1 = context1.Database.GetDbConnection();
            var connection2 = context2.Database.GetDbConnection();
            Console.WriteLine($"Retrieving data from {connection1.DataSource}/{connection1.Database}");
            Console.WriteLine($"Retrieving data from {connection2.DataSource}/{connection2.Database}");
            return View((context1.Test1Data.ToList(), context2.Test2Data.ToList()));
        }
    }
}
