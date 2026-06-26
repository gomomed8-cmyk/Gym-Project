using GymManagement.BLL.Services.interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymCore_Project.Controllers
{
    [Authorize]
    public class PlansController : Controller
    {

        public readonly IPlanService _planRepository;
        public PlansController(IPlanService planRepository)
        {
            _planRepository = planRepository;
        }
        //index action
        public async Task<IActionResult> Index(CancellationToken ct) => View(await _planRepository.GetAllPlansAsync(ct));
        // details action
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var Plan = await _planRepository.GetPlanByIdAsync(id, ct);
            if (Plan is null)
            {
                TempData["ErrorMassage"] = "Plan not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(Plan);

        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var Plan = await _planRepository.GetPlanTOUpdateAsync(id, ct);
            if (Plan is null)
            {
                TempData["ErrorMassage"] = "Plan cannot be edited(not found,inactive,or has active Memberships)";
                return RedirectToAction(nameof(Index));
            }
            return View(Plan);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdatePlanViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _planRepository.UpdatePlanAsync(id, model, ct);
            if (result)
            {
                TempData["SuccessMassage"] = "Plan updated successfully.";
            }
            else
                TempData["ErrorMassage"] = "Failed to update the plan.";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            var result = await _planRepository.ToggleActivationAsync(id, ct);
            if (result)
            {
                TempData["SuccessMassage"] = "Plan activation status toggled successfully.";
            }
            else
            {
                TempData["ErrorMassage"] = "Failed to toggle the plan activation status.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
