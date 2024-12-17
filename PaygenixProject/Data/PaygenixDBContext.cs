using Microsoft.EntityFrameworkCore;
using NewPayGenixAPI.Models;
using PaygenixProject.Models;

namespace NewPayGenixAPI.Data
{
    public class PaygenixDBContext : DbContext
    {

        public PaygenixDBContext(DbContextOptions<PaygenixDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Role and User (One-to-Many)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleID)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee and User (One-to-One)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserID);

            //Employee and Payroll (One-to-Many)
            modelBuilder.Entity<Payroll>()
                .HasOne(p => p.Employee)
                .WithMany(e => e.Payrolls)
                .HasForeignKey(p => p.EmployeeID);

            //Employee and LeaveRequest (One-to-Many)
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(l => l.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(l => l.EmployeeID);


            //ComplainceReport and Employee (One-to-Many)
            modelBuilder.Entity<ComplianceReport>()
                .HasOne(cr => cr.Employee)
                .WithMany()
                .HasForeignKey(cr => cr.EmployeeID)
                .OnDelete(DeleteBehavior.Restrict);
        }

      

            public DbSet<Role> Roles { get; set; }
            public DbSet<User> Users { get; set; }
            public DbSet<Employee> Employees { get; set; }
            public DbSet<Payroll> Payrolls { get; set; }
            public DbSet<LeaveRequest> LeaveRequests { get; set; }
            
            public DbSet<ComplianceReport> ComplianceReports { get; set; }
            public DbSet<RefreshToken> RefreshTokens { get; set; }

            public DbSet<AuditTrail> AuditTrails { get; set; }

        }
}
