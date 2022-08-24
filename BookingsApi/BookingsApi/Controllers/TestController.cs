using BookingsApi.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BookingsApi.Controllers
{
    [AllowAnonymous]
    public class TestController : Controller
    {
        public BookingsDbContext _context;

        public TestController(BookingsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Test()
        {
            //var allocations = _context.Allocations.ToList();
            var VhoWorkHours = _context.VhoWorkHours.ToList();
            var justiceUsers = _context.JusticeUsers.ToList();
            var VhoNonAvailabilities = _context.VhoNonAvailabilities.ToList();
            var DaysOfWeek = _context.DaysOfWeek.ToList();

            return Ok();
        }
    }
}
