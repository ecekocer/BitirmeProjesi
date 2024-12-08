using BitirmeProjesi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BitirmeProjesi.Controllers
{
    public class PollutionEntryController : Controller
    {
        private readonly BitirmeProjesiiContext _context;

        public PollutionEntryController(BitirmeProjesiiContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Latitude,Longitude,MetalType,Value")] PollutionData pollutionData)
        {
            if (ModelState.IsValid)
            {
                pollutionData.DataRecorded = DateTime.Now;
                pollutionData.EnteredBy = User.Identity.Name;
                
                _context.Add(pollutionData);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Veri başarıyla kaydedildi!";
                return RedirectToAction(nameof(Create));
            }
            return View(pollutionData);
        }
    }
} 