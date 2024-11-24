using Microsoft.EntityFrameworkCore;
using NewPayGenixAPI.Data;
using NewPayGenixAPI.Models;

namespace NewPayGenixAPI.Repositories
{
        public class ComplianceReportRepository : IComplianceReportRepository
        {
            private readonly PaygenixDBContext _context;

            public ComplianceReportRepository(PaygenixDBContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<ComplianceReport>> GetAllComplianceReportAsync()
            {
                return await _context.ComplianceReports.Include(cr => cr.Employee).ToListAsync();
            }

            public async Task<ComplianceReport> GetComplianceReportByIdAsync(int id)
            {
                return await _context.ComplianceReports.Include(cr => cr.Employee).FirstOrDefaultAsync(cr => cr.ReportID == id);
            }

            public async Task AddComplianceReportAsync(ComplianceReport report)
            {
                await _context.ComplianceReports.AddAsync(report);
                await _context.SaveChangesAsync();
            }

            public async Task UpdateComplianceReportAsync(ComplianceReport report)
            {
                _context.ComplianceReports.Update(report);
                await _context.SaveChangesAsync();
            }

        public async Task DeleteComplianceReportAsync(int id)
        {
            var report = await _context.ComplianceReports.FindAsync(id);
            if (report == null) throw new Exception("Compliance Report not found");

            _context.ComplianceReports.Remove(report);
            await _context.SaveChangesAsync();
        }
    }
}
