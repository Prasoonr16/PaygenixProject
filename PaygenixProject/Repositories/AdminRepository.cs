﻿using Microsoft.EntityFrameworkCore;
using NewPayGenixAPI.Data;
using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;
using PaygenixProject.Models;

namespace NewPayGenixAPI.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly PaygenixDBContext _context;


        public AdminRepository(PaygenixDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeID == id);
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) throw new Exception("Employee not found");

            employee.ActiveStatus = "Offboarded";
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllUserAsync()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserID == id);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Payroll>> GetAllPayrollsAsync()
        {
            return await _context.Payrolls.ToListAsync();
        }

        public async Task AddPayrollAsync(Payroll payroll)
        {
            var newpayroll = new Payroll
            {
                EmployeeID = payroll.EmployeeID,
                BasicSalary = payroll.BasicSalary,
                HRA = payroll.HRA,
                LTA = payroll.LTA,
                TravellingAllowance = payroll.TravellingAllowance,
                DA = payroll.DA,
                GrossPay = payroll.GrossPay,
                PF = payroll.PF,
                TDS = payroll.TDS,
                ESI = payroll.ESI,
                Deduction = payroll.Deduction,
                NetPay = payroll.NetPay,
                StartPeriod = payroll.StartPeriod,
                EndPeriod = payroll.EndPeriod,
                GeneratedDate = payroll.GeneratedDate
            };
            await _context.Payrolls.AddAsync(newpayroll);
            await _context.SaveChangesAsync();
        }
        public async Task AddUserAsync(User user)
        {
            try
            {
                _context.Users.Add(user); // Add user to the DbSet
                await _context.SaveChangesAsync(); // Commit changes to the database
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not add user: {ex.Message}");
            }
        }


        public async Task<Payroll> GetPayrollByEmployeeIdAsync(int id)
        {
            return await _context.Payrolls.FirstOrDefaultAsync(p => p.EmployeeID == id);
        }

        public async Task UpdatePayrollAsync(Payroll payroll)
        {
            _context.Payrolls.Update(payroll);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<ComplianceReport>> GetAllComplianceReportAsync()
        {
            return await _context.ComplianceReports.ToListAsync();
        }

        public async Task<bool> UpdateComplianceReportAsync(int reportId, ComplianceReportDTO reportDTO)
        {
            // Find the compliance report for the given EmployeeID
            var report = await _context.ComplianceReports.FirstOrDefaultAsync(r => r.ReportID == reportId);

            if (report == null)
            {
                return false; // Compliance report not found
            }

            // Update the fields based on the DTO
            
            report.ComplianceStatus = reportDTO.ComplianceStatus;
            report.ResolvedStatus = reportDTO.ResolvedStatus;


            // Save changes to the database

            _context.ComplianceReports.Update(report);

            await _context.SaveChangesAsync();

            return true; // Update successful
        }


        public async Task<IEnumerable<AuditTrail>> GetAllAuditTrailsAsync()
        {
            return await _context.AuditTrails.OrderByDescending(a => a.Timestamp).ToListAsync();
        }

        public async Task<IEnumerable<AuditTrail>> SearchAuditTrailsAsync(string searchTerm, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.AuditTrails.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(a => a.Action.Contains(searchTerm) || a.PerformedBy.Contains(searchTerm));
            }

            if (startDate.HasValue)
            {
                query = query.Where(a => a.Timestamp >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(a => a.Timestamp <= endDate.Value);
            }

            return await query.OrderByDescending(a => a.Timestamp).ToListAsync();
        }

        public async Task LogAuditTrailAsync(AuditTrail auditTrail)
        {
            await _context.AuditTrails.AddAsync(auditTrail);
            await _context.SaveChangesAsync();
        }
    }
}