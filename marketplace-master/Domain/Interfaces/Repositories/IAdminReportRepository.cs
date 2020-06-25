using Domain.DTO;
using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IAdminReportRepository
    {
        AdminDashboardReport GetDashboardReport(AdminDashboardReportRequest request);
    }
}