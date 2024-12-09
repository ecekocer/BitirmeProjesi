using BitirmeProjesi.Models;
using BitirmeProjesi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BitirmeProjesi.Controllers
{
    [Authorize]
    public class PollutionEntryController : Controller
    {
        private readonly BitirmeProjesiiContext _context;

        public PollutionEntryController(BitirmeProjesiiContext context)
        {
            _context = context;
        }

        public IActionResult Create(double? latitude, double? longitude)
        {
            var viewModel = new PollutionEntryViewModel
            {
                Latitude = latitude.HasValue ? (float)latitude.Value : 0f,
                Longitude = longitude.HasValue ? (float)longitude.Value : 0f
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PollutionEntryViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var pollutionData = new PollutionData
                    {
                        Latitude = viewModel.Latitude,
                        Longitude = viewModel.Longitude,
                        MetalType = viewModel.MetalType,
                        Value = viewModel.Value,
                        DataRecorded = DateTime.Now,
                        EnteredById = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        City = null,
                        Region = null
                    };
                    
                    _context.Add(pollutionData);
                    await _context.SaveChangesAsync();
                    
                    TempData["ToastMessage"] = "Veri başarıyla kaydedildi!";
                    TempData["ToastType"] = "success";
                    return RedirectToAction("Index", "Home");
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Veri kaydedilirken bir hata oluştu: " + ex.Message);
                return View(viewModel);
            }
        }
    }
} 