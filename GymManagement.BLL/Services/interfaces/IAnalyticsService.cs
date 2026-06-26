using GymManagement.BLL.ViewModels.AnalyticsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.interfaces
{
    public interface IAnalyticsService
    {
        Task<AnalyticsViewModel> GetDataAsync(CancellationToken ct=default);
    }
}
