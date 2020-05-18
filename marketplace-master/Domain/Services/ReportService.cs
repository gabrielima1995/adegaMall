using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Domain.Reports
{
    public class ReportService
    {
        private readonly IAdminReportRepository _adminReportRepository;

        public ReportService(IAdminReportRepository adminReportRepository)
        {
            _adminReportRepository = adminReportRepository;
        }

        public AdminDashboardReport GetAdminDashboardReport(AdminDashboardReportRequest request)
        {
            return _adminReportRepository.GetDashboardReport(request);
        }
    }
}
