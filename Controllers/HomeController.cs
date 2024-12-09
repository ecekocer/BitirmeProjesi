using BitirmeProjesi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace BitirmeProjesi.Controllers
{
    public class HomeController : Controller
    {
        private readonly BitirmeProjesiiContext _context;

        public HomeController(BitirmeProjesiiContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPollutionData()
        {
            try
            {
                var data = await _context.PollutionDatas
                    .Select(p => new
                    {
                        p.Latitude,
                        p.Longitude,
                        p.MetalType,
                        p.Value,
                        p.DataRecorded
                    })
                    .ToListAsync();

                return Json(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Veriler alınırken bir hata oluştu");
            }
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
