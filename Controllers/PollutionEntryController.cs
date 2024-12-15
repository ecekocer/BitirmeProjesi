using BitirmeProjesi.Models;
using BitirmeProjesi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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
                Longitude = longitude.HasValue ? (float)longitude.Value : 0f,
                Year = DateTime.Now.Year
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
                        Year = viewModel.Year,
                        DataRecorded = DateTime.Now,
                        EnteredById = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        City = "",
                        Region = ""
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
                // İç exception'ı da göster
                var message = ex.InnerException != null 
                    ? $"Veri kaydedilirken bir hata oluştu: {ex.Message} - İç Hata: {ex.InnerException.Message}"
                    : $"Veri kaydedilirken bir hata oluştu: {ex.Message}";
                    
                ModelState.AddModelError(string.Empty, message);
                return View(viewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApplyFilters([FromBody] PollutionFilterViewModel filters)
        {
            try
            {
                var query = _context.PollutionDatas.AsQueryable();

                // Metal türüne göre filtrele
                if (!string.IsNullOrEmpty(filters.MetalType))
                {
                    query = query.Where(p => p.MetalType == filters.MetalType);
                }

                // Yıla göre filtrele
                if (!string.IsNullOrEmpty(filters.Year))
                {
                    int year = int.Parse(filters.Year);
                    query = query.Where(p => p.Year == year);
                }

                // Bölgeye göre filtrele
                if (!string.IsNullOrEmpty(filters.Region))
                {
                    query = query.Where(p => p.Region.ToLower() == filters.Region.ToLower());
                }

                // Şehre göre filtrele
                if (!string.IsNullOrEmpty(filters.City))
                {
                    query = query.Where(p => p.City.ToLower() == filters.City.ToLower());
                }

                // Kirlilik seviyesine göre filtrele
                if (!string.IsNullOrEmpty(filters.PollutionLevel))
                {
                    query = filters.PollutionLevel switch
                    {
                        "low" => query.Where(p => p.Value < 50),
                        "medium" => query.Where(p => p.Value >= 50 && p.Value < 100),
                        "high" => query.Where(p => p.Value >= 100 && p.Value < 150),
                        "critical" => query.Where(p => p.Value >= 150),
                        _ => query
                    };
                }

                var results = await query
                    .Select(p => new
                    {
                        p.Id,
                        p.MetalType,
                        PollutionValue = p.Value,
                        Date = p.DataRecorded,
                        p.Year,
                        p.Latitude,
                        p.Longitude,
                        p.City,
                        p.Region,
                        PollutionLevel = p.Value < 50 ? "Düşük" :
                                        p.Value < 100 ? "Orta" :
                                        p.Value < 150 ? "Yüksek" : "Kritik"
                    })
                    .ToListAsync();

                // İstatistiksel verileri hesapla
                var statistics = new
                {
                    AveragePollution = await query.AverageAsync(p => p.Value),
                    MaxPollution = await query.MaxAsync(p => p.Value),
                    MinPollution = await query.MinAsync(p => p.Value),
                    TotalEntries = await query.CountAsync(),
                    PollutionLevels = await query
                        .GroupBy(p => p.Value < 50 ? "Düşük" :
                                    p.Value < 100 ? "Orta" :
                                    p.Value < 150 ? "Yüksek" : "Kritik")
                        .Select(g => new { Level = g.Key, Count = g.Count() })
                        .ToListAsync()
                };

                return Json(new { 
                    success = true, 
                    data = results,
                    statistics = statistics
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "Filtreleme işlemi sırasında bir hata oluştu.",
                    error = ex.Message 
                });
            }
        }
    }
} 