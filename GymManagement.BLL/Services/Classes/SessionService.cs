using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Data.Enums;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork ,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
           if(model.EndDate<=model.StartDate) return Result.Vaildation("EndDate Muste Be After StartDate");
           if(model.StartDate<=DateTime.Now) return Result.Vaildation("StartDate Muste Be In The Future");
           if(model.Capacity<1 || model.Capacity>25) return Result.Vaildation("Capacity Muste Be Between 1 And 25");

           var trainer= await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if(trainer is null) return Result.NotFound("Trainer Not Found");

            var category =await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId);
            if (category is null) return Result.NotFound("Category Not Found"); ;

            var isValid=Enum.TryParse<Specialty>(category.CategoryName,true ,out var CategorySpecialty);
            if (!isValid||trainer.Specialty != CategorySpecialty) return Result.Vaildation("Can Not Create This Session To This Trainer");
             
            var session=_mapper.Map<CreateSessionViewModel,Session>(model);
            _unitOfWork.GetRepository<Session>().Add(session);
        var reslut = await _unitOfWork.SaveChangesAsync();
            return reslut > 0 ? Result.OK() : Result.Fail("Failed To Create Session");
                
        }

        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessionRepo = _unitOfWork.SessionRepository;
         var sessions=  await sessionRepo.GetAllSessionWithTrainerAndCategory(ct);

            if (sessions == null || !sessions.Any()) return null;
            var mappedSessions = sessions.Select(s => new SessionViewModel()
            {
                Id = s.Id,
                Capacity = s.Capacity,
                CategoryName=s.Category.CategoryName,
                TrainerName=s.Trainer.Name,
                Description=s.Description,
                EndDate=s.EndDate,
                StartDate=s.StartDate,
            });

            foreach (var session in  mappedSessions)
            {
                session.AvailableSlots = session.Capacity - await sessionRepo.GetCountOfBookedSlotsAsync(session.Id,ct);
            }
            return mappedSessions;
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategorisForDropDownAsync(CancellationToken ct = default)
        {
            var reslut= await _unitOfWork.GetRepository<Category>().GetAllAsync(ct:ct);
           return _mapper.Map<IEnumerable<CategorySelectViewModel>>(reslut);
        }

        public async Task<Result<SessionViewModel>> GetSessionByIdAsync(int SessionId, CancellationToken ct = default)
        {
            var session=await _unitOfWork.SessionRepository.GetSessionByIdWithTrainerAndCategory(SessionId,ct);
            if (session is null)
                return Result<SessionViewModel>.NotFound("Session Not Found");
            else
            {
                var mappedSession= _mapper.Map<Session,SessionViewModel>(session);
                mappedSession.AvailableSlots=mappedSession.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(SessionId,ct);
                return Result<SessionViewModel>.Ok(mappedSession);
            }

        }
        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default)
        {
            var reslut = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(reslut);
        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int SessionId, CancellationToken ct = default)
        {
           var session=await _unitOfWork.SessionRepository.GetByIdAsync(SessionId,ct);
            if (session == null) return Result<UpdateSessionViewModel>.NotFound("Session Not Found");

            if (session.StartDate <= DateTime.Now)
                return Result<UpdateSessionViewModel>.Fail("Can Not Update Session That Has Already Started");
            var booingCount= await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(SessionId, ct);
            if (booingCount > 0)
                return Result<UpdateSessionViewModel>.Fail("Can Not Update Session That Has Already Bookings");

            var mappedSession= _mapper.Map<Session,UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.Ok(mappedSession);
        }
        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
           var session=await _unitOfWork.SessionRepository.GetByIdAsync(id,ct);
            if(session == null)
                if (session == null) return Result.NotFound("Session Not Found");
            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Can Not Edit Session That Has Already Started");
            if (model.EndDate <= model.StartDate)
                return Result.Vaildation("End Date Muste Be After StartDte");
            var bookedCount =await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id);
            if (bookedCount > 0)
                return Result.Fail("Can Not Update Session That Has Already Bookings");
            if (model.EndDate <= DateTime.Now)
                return Result.Vaildation("Start Date Muste Be In Future");
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainer is null) return Result.NotFound("Trainer Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId);
            //if (category is null) return Result.NotFound("Category Not Found"); ;

            var isValid = Enum.TryParse<Specialty>(category?.CategoryName, true, out var CategorySpecialty);
            if (!isValid || trainer.Specialty != CategorySpecialty) return Result.Vaildation("Can Not Create This Session To This Trainer");

            _mapper.Map(model, session);
            session.UpdatedAt=DateTime.Now;
            _unitOfWork.SessionRepository.Update(session);
            var result =await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed To Update Session");

        }

        public async Task<Result> RemoveSessionAsync(int SessionId, CancellationToken ct = default)
        {
            var session= await _unitOfWork.SessionRepository.GetByIdAsync(SessionId, ct);
            if (session is null) return Result.NotFound("Session is Not Found");
            if (session.EndDate >= DateTime.Now)
                return Result.Fail("Can Not Delete Session That Has Not Ended Yet");
           var bookedCount=await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(SessionId, ct);
            if (bookedCount > 0)
                return Result.Fail("Can Not Delete Session That Has Bookings");
            _unitOfWork.SessionRepository.Delete(session);
            var result=await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed To Delete Session");

        }
    }
}
