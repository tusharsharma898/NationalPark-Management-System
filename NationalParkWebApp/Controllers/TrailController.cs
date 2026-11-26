using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NationalParkWebApp.Models;
using NationalParkWebApp.Models.ViewModels;
using NationalParkWebApp.Repository.IRepository;

namespace NationalParkWebApp.Controllers
{
    public class TrailController : Controller
    {
        private readonly ITrailRepository _trailRepository;
        private readonly INationalParkRepository _nationalParkRepository;
        public TrailController(ITrailRepository trailRepository, INationalParkRepository nationalParkRepository)
        {
            _trailRepository = trailRepository;
            _nationalParkRepository= nationalParkRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Upsert(int? id)
        {
            var NationalParkInDb = await _nationalParkRepository.GetAllAsync(SD.NationalParkAPIPath);
            TrailVM trailVM = new TrailVM()
            {
                NationalParkList = NationalParkInDb.Select(np => new SelectListItem()
                {
                    Text = np.Name,
                    Value = np.Id.ToString()
                }),
                Trail = new Trail()
            };
            if (id == null) return View(trailVM);
            trailVM.Trail = await _trailRepository.GetAsync(SD.TrailAPIPath, id.GetValueOrDefault());
            if (trailVM.Trail == null) return NotFound();
            return View(trailVM);
        }
        [HttpPost]
        public async Task<IActionResult> Upsert(TrailVM trailVM)
        {
            if (trailVM == null) return NotFound();
            if(!ModelState.IsValid)
            {
                return View(trailVM);
            }
            if (trailVM.Trail.Id == 0)
              await _trailRepository.CreateAsync(SD.TrailAPIPath, trailVM.Trail);
            else
           await _trailRepository.UpdateAsync(SD.TrailAPIPath, trailVM.Trail);
            return RedirectToAction("Index");
        }
        #region APIs
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _trailRepository.GetAllAsync(SD.TrailAPIPath) });
        }
        #endregion
    }
}
