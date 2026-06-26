using GymManagement.BLL.Services.interfaces;
using GymManagement.BLL.ViewModels.AnalyticsViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<AnalyticsViewModel> GetDataAsync(CancellationToken ct = default)
        {
            var now= DateTime.Now;
           var upcomingSession=await _unitOfWork.GetRepository<Session>().CountAsync(x=>x.StartDate > now);
           var omgoingSession=await _unitOfWork.GetRepository<Session>().CountAsync(x=>x.StartDate <= now && x.EndDate >= now);
           var completedSession=await _unitOfWork.GetRepository<Session>().CountAsync(x=>x.EndDate < now); 
            var totalMembers=await _unitOfWork.GetRepository<Member>().CountAsync(ct:ct);
            var totalTrainers=await _unitOfWork.GetRepository<Trainer>().CountAsync(ct: ct);
            var activeMembers =await _unitOfWork.GetRepository<MemberShip>().CountAsync(x => x.EndDate > now, ct);

            return new AnalyticsViewModel()
            {
                TotalMembers = totalMembers,
                TotalTrainers = totalTrainers,
                ActiveMembers = activeMembers,
                UpcomingSession = upcomingSession,
                CompletedSession = completedSession,
                OngoingSession = omgoingSession,
            };
        }
    }
}
