using GymManagement.BLL.Services.interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymCore_Project.Controllers
{
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var Sessions= await _sessionService.GetAllSessionsAsync(ct);
            return View(Sessions);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
         await PopulateDropDownListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model,CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
             await PopulateDropDownListAsync();
                return View(model);
            }

          var reslut= await _sessionService.CreateSessionAsync(model,ct);
            if(reslut.success)
            {
                TempData["SuccessMessage"] = "Session Created";
                return RedirectToAction(nameof(Index));
            }           
                TempData["ErrorMessage"] =reslut.erorr;
            await PopulateDropDownListAsync();
            return View(model);
            


        }
        private async Task PopulateDropDownListAsync()
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
            ViewBag.Categoris = new SelectList(await _sessionService.GetCategorisForDropDownAsync(), "Id", "CategoryName");
        }
     [HttpGet]
        public async Task<IActionResult> Details(int id,CancellationToken ct)
        {
            var result=await _sessionService.GetSessionByIdAsync(id,ct);
            if (result.success)
            return View(result.value);

            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id ,CancellationToken ct)
        {
          var result=await _sessionService.GetSessionToUpdateAsync(id,ct);
            if (result.success)
            {
              ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
            return View(result.value);
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id,UpdateSessionViewModel model , CancellationToken ct)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
                return View(model);
            }
            var result= await _sessionService.UpdateSessionAsync(id,model,ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session Update";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"]=result.erorr;
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id,CancellationToken ct)
        {
            var result=await _sessionService.GetSessionByIdAsync(id, ct);
            if(result.success)
            {
                return View(result.value);
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id,CancellationToken ct)
        {
            var result=await _sessionService.RemoveSessionAsync(id,ct);
            if(result.success)
            {
                TempData["SuccessMessage"] = "Session Deleted";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.erorr;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
