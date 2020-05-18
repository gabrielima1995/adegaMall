using Domain.DTO;
using Domain.Entities;

namespace Application.Reports
{
    public interface IReportAppService
    {
        AdminDashboardReport GetAdminDashboardReport(AdminDashboardReportRequest request);
    }
}