using Microsoft.AspNetCore.Mvc;
using NationalParkWebApp.Models;
using NationalParkWebApp.Models.ViewModels;
using NationalParkWebApp.Repository.IRepository;
using System.Diagnostics;

namespace NationalParkWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly INationalParkRepository _NationalParkRepository;
        private readonly ITrailRepository _TrailRepository;
        public HomeController(INationalParkRepository nationalParkRepository, ITrailRepository trailRepository)
        {
            _NationalParkRepository = nationalParkRepository;
            _TrailRepository = trailRepository;
        }
        public async Task<IActionResult> Index()
        {
            IndexVM indexVM = new IndexVM()
            {
                NationalParkList = await _NationalParkRepository.GetAllAsync(SD.NationalParkAPIPath),
                TrailList = await _TrailRepository.GetAllAsync(SD.TrailAPIPath)
            };
            return View(indexVM);
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
