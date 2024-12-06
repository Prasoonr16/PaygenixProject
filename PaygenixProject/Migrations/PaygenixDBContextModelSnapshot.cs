﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NewPayGenixAPI.Data;

#nullable disable

namespace PaygenixProject.Migrations
{
    [DbContext(typeof(PaygenixDBContext))]
    partial class PaygenixDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("NewPayGenixAPI.Models.Benefit", b =>
                {
                    b.Property<int>("BenefitID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BenefitID"));

                    b.Property<string>("BenefitName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EligibilityCriteria")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BenefitID");

                    b.ToTable("Benefits");
                });

            modelBuilder.Entity("NewPayGenixAPI.Models.ComplianceReport", b =>
                {
                    b.Property<int>("ReportID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReportID"));

                    b.Property<string>("Comments")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ComplianceStatus")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("EmployeeID")
                        .HasColumnType("int");

                    b.Property<string>("IssuesFound")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PayrollPeriod")
                        .HasMaxLength(50)
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ReportDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ResolvedStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ReportID");

                    b.HasIndex("EmployeeID");

                    b.ToTable("ComplianceReports");
                });

            modelBuilder.Entity("NewPayGenixAPI.Models.Employee", b =>
                {
                    b.Property<int>("EmployeeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EmployeeID"));

                    b.Property<string>("ActiveStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("HireDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("UserID")
                        .HasColumnType("int");

                    b.HasKey("EmployeeID");

                    b.HasIndex("UserID")
                        .IsUnique()
                        .HasFilter("[UserID] IS NOT NULL");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("NewPayGenixAPI.Models.EmployeeBenefit", b =>
                {
                    b.Property<int>("EmployeeBenefitID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EmployeeBenefitID"));

                    b.Property<int>("BenefitID")
                        .HasColumnType("int");

                    b.Property<int>("EmployeeID")
                        .HasColumnType("int");

                    b.Property<DateTime>("EnrolledDate")
                        .HasColumnType("datetime2");

                    b.HasKey("EmployeeBenefitID");

                    b.HasIndex("BenefitID");

                    b.HasIndex("EmployeeID");

                    b.ToTable("EmployeeBenefits");
                });

            modelBuilder.Entity("NewPayGenixAPI.Models.LeaveRequest", b =>
                {
                    b.Property<int>("LeaveRequestID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LeaveRequestID"));

                    b.Property<DateTime?>("ApprovalDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("EmployeeID")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LeaveType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("RequestDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("LeaveRequestID");

                    b.HasIndex("EmployeeID");

                    b.ToTable("LeaveRequests");
                });

            modelBuilder.Entity("NewPayGenixAPI.Models.Payroll", b =>
                {
                    b.Property<int>("PayrollID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PayrollID"));

                    b.Property<decimal>("BasicSalary")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal>("DA")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal>("Deduction")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal>("ESI")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("EmployeeID")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndPeriod")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("GeneratedDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("GrossPay")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal>("HRA")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal>("LTA")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal>("NetPay")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal>("PF")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<DateTime>("StartPeriod")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("TDS")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal>("TaxAmount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal>("TravellingAllowance")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("PayrollID");

                    b.HasIndex("EmployeeID");

                    b.ToTable("Payrolls");
                });

            modelBuilder.Entity("NewPayGenixAPI.Models.Role", b =>
                {
                    b.Property<int>("RoleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoleID"));

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RoleID");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("NewPayGenixAPI.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserID"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("datetime2");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("RoleID")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("UserID");

                    b.HasIndex("RoleID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PaygenixProject.Models.RefreshToken", b =>
                {
                    b.Property<int>("RefreshTokenID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RefreshTokenID"));

                    b.Property<DateTime>("Expires")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsRevoked")
                        .HasColumnType("bit");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("bit");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("RefreshTokenID");

                    b.HasIndex("UserID");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("NewPayGenixAPI.Models.ComplianceReport", b =>
                {
                    b.HasOne("NewPayGenixAPI.Models.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("NewPayGenixAPI.Models.Employee", b =>
                {
                    b.HasOne("NewPayGenixAPI.Models.User", "User")
                        .WithOne("Employee")
                        .HasForeignKey("NewPayGenixAPI.Models.Employee", "UserID");

                    b.Navigation("User");
                });

            modelBuilder.Entity("NewPayGenixAPI.Models.EmployeeBenefit", b =>
                {
                    b.HasOne("NewPayGenixAPI.Models.Benefit", "Benefit")
                        .WithMany("EmployeeBenefits")
                        .HasForeignKey("BenefitID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NewPayGenixAPI.Models.Employee", "Employee")
                        .WithMany("EmployeeBenefits")
                        .HasForeignKey("EmployeeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Benefit");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("NewPayGenixAPI.Models.LeaveRequest", b =>
                {
                    b.HasOne("NewPayGenixAPI.Models.Employee", "Employee")
                        .WithMany("LeaveRequests")
                        .HasForeignKey("EmployeeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("NewPayGenixAPI.Models.Payroll", b =>
                {
                    b.HasOne("NewPayGenixAPI.Models.Employee", "Employee")
                        .WithMany("Payrolls")
                        .HasForeignKey("EmployeeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("NewPayGenixAPI.Models.User", b =>
                {
                    b.HasOne("NewPayGenixAPI.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("PaygenixProject.Models.RefreshToken", b =>
                {
                    b.HasOne("NewPayGenixAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("NewPayGenixAPI.Models.Benefit", b =>
                {
                    b.Navigation("EmployeeBenefits");
                });

            modelBuilder.Entity("NewPayGenixAPI.Models.Employee", b =>
                {
                    b.Navigation("EmployeeBenefits");

                    b.Navigation("LeaveRequests");

                    b.Navigation("Payrolls");
                });

            modelBuilder.Entity("NewPayGenixAPI.Models.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("NewPayGenixAPI.Models.User", b =>
                {
                    b.Navigation("Employee")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
