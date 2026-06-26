using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Attachment;
using GymManagement.BLL.Services.interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork unitOfWork,IMapper mapper,IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct)
        {
            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email, ct);
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone, ct);
            if (emailExists || phoneExists) return false;

          var storedPhotoName= await _attachmentService.UploadAsync(model.PhotoFile.OpenReadStream(),model.PhotoFile.FileName,"MembersPhoto");
            if (string.IsNullOrWhiteSpace(storedPhotoName)) return false;

            var member =_mapper.Map<Member>(model);
            member.Photo= storedPhotoName;
            _unitOfWork.GetRepository<Member>().Add(member);//add locally
            var reslut = await _unitOfWork.SaveChangesAsync(ct);
           if(reslut>0) return true;
           else
            {
                _attachmentService.Delete(storedPhotoName, "MembersPhoto");
                return false;
            }
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            if (!members.Any()) return [];
            var MemberViewModels = _mapper.Map<IEnumerable<Member>,IEnumerable<MemberViewModel>>(members);
            return MemberViewModels;
        }

        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct);
            if (member == null) return null;
            var memberViewModel = _mapper.Map<Member,MemberViewModel>(member); 


            var activeMembership = await _unitOfWork.GetRepository<MemberShip>().FirstOrDefaultAsync(m => m.MemberId == MemberId && m.EndDate > DateTime.Now);
            if (activeMembership is not null)
            {
                    var activeplan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMembership.PlanId, ct);
                memberViewModel.PlanName = activeplan?.Name;
                memberViewModel.MembershipStartDate= activeMembership.CreatedAt.ToString();
                    memberViewModel.MembershipEndDate = activeMembership.EndDate.ToString();
            }
            return memberViewModel;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordByIdAsync(int MemberId, CancellationToken ct = default)
        {
            var healthRecord = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(m=>m.MemberId==MemberId, ct:ct);
            if(healthRecord == null) return null;
           return _mapper.Map<HealthRecord, HealthRecordViewModel>(healthRecord);
            
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateByIdAsync(int MemberId, CancellationToken ct = default)
        {
            var member =await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct);
            if (member is null) return null;
            else
                return _mapper.Map<Member, MemberToUpdateViewModel>(member);
        }

        public async Task<bool> UpdateMemberDetailsAsync(int Id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member= await _unitOfWork.GetRepository<Member>().GetByIdAsync(Id,ct);
            if (member is null) return false;

            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email && m.Id != Id);
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != Id);
            if(emailExists || phoneExists) return false;
            _mapper.Map(model,member);
            model.UpdatedAt= DateTime.Now;


             _unitOfWork.GetRepository<Member>().Update(member);
            var result= await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;

        }
        public async Task<bool> RemoveMemberAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct);
            if (member is null) return false;

            var hasFetureBookings = await _unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId == MemberId && b.Session.StartDate > DateTime.Now,ct);
            if (hasFetureBookings) return false;

             _unitOfWork.GetRepository<Member>().Delete(member);
            var result= await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }
    }
}

