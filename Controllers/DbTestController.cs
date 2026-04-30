using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;

// This controller is for testing the Database Connection only
// Route is /home/dbtest

namespace Omni_Flex.Controllers
{
    public class DbTestController(IDbConnection db) : Controller
    {
        private readonly IDbConnection _db = db;

        public IActionResult Index()
        {
            var result = _db.Query("SELECT * FROM Users").ToList();
            return Json(result);
        }
    }
}
