using GymManagement.BLL.Services.Attachment;
using GymManagement.BLL.Services.interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Threading.Tasks;

namespace GymCore_Project.Controllers
{
    [Authorize(Roles ="SuperAdmin")]
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IAttachmentService _attachmentService;

        public MembersController(IMemberService memberService,IAttachmentService attachmentService)
        {
            _memberService = memberService;
            _attachmentService = attachmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var member=await _memberService.GetMemberDetailsByIdAsync(id);
            if(member == null || string.IsNullOrWhiteSpace(member.Photo))
                return NotFound();
           var result= _attachmentService.GetFile(member.Photo, "MembersPhoto");
            if(result == null) return NotFound();
            return File(result.Value.stream, result.Value.contentType);
            
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberService.GetAllMembersAsync(ct);
            return View(members);
        }
        [HttpGet]
        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Create), model);
            }
            var result = await _memberService.CreateMemberAsync(model, ct);
            if (result)
                TempData["SuccessMessage"] = "Member created successfully!";
            else
                TempData["ErrorMessage"] = "Failed to create member";
            return RedirectToAction(nameof(Index));


        }
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsByIdAsync(id, ct);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);

        }
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var HealthRecord = await _memberService.GetMemberHealthRecordByIdAsync(id, ct);
            if (HealthRecord is null)
            {
                TempData["ErrorMessage"] = "Health Record not found";
                return RedirectToAction(nameof(Index));
            }
            return View(HealthRecord);
        }
        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberToUpdateByIdAsync(id, ct);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }
        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute]int id ,MemberToUpdateViewModel model,CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _memberService.UpdateMemberDetailsAsync(id, model, ct);
            if (result)
            
                TempData["SuccessMessage"] = "Member updated successfully!";
            else
                TempData["ErrorMessage"] = "Failed to update member";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id,CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsByIdAsync(id, ct);
            if (member == null)
            {
                TempData["ErrorMessage"] = "Member not found";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute]int id ,CancellationToken ct)
        {
            var result = await _memberService.RemoveMemberAsync(id, ct);
            if (result)
                TempData["SuccessMessage"] = "Member deleted successfully!";
            else
                TempData["ErrorMessage"] = "Failed to delete member";
            return RedirectToAction(nameof(Index));
        }

    }
     
}
