using NewPayGenixAPI.Models;

namespace NewPayGenixAPI.Repositories
{
    public interface IComplianceReportRepository
    {
        Task<IEnumerable<ComplianceReport>> GetAllComplianceReportAsync();

        Task<ComplianceReport> GetComplianceReportByIdAsync(int id);

        Task AddComplianceReportAsync(ComplianceReport report);

        Task UpdateComplianceReportAsync(ComplianceReport report);

        Task DeleteComplianceReportAsync(int id);
    }
}
