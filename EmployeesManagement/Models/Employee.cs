﻿using System.ComponentModel;

namespace EmployeesManagement.Models
{
    public class Employee : UserActivity
    {
        public int Id { get; set; }
        public string EmpNo { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {MiddleName} {LastName}";
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
        [DisplayName("Email Address")]
        public string EmailAddress { get; set; }
        [DisplayName("Country Name")]
        public int? CountryId { get; set; }
        public Country Country { get; set; }
        [DisplayName("Date Of Birth")]
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        [DisplayName("Department Name")]
        public int? DepartmentId { get; set; }
        public Department Department { get; set; }
        [DisplayName("Designation Name")]
        public int? DesignationId { get; set; }
        public Designation Designation { get; set; }
        [DisplayName("Gender Name")]
        public int? GenderId { get; set; }
        public SystemCodeDetail Gender { get; set; }
        [DisplayName("Employee Photo")]
        public string? Photo { get; set; }
        [DisplayName("Employment Date ")]
        public DateTime? EmploymentDate { get; set; }
        public int? StatusId { get; set; }
        public SystemCodeDetail Status { get; set; }
        public DateTime? InactiveDate { get; set; }
        public int? CauseOfInactivityId { get; set; }
        public SystemCodeDetail CauseOfInactivity { get; set; }
        public DateTime? TerminationDate { get; set; }
        public int? ReasonforTerminationId { get; set; }
        public SystemCodeDetail ReasonforTermination { get; set; }
        [DisplayName("Bank Name")]
        public int? BankId { get; set; }
        public Bank Bank { get; set; }
        [DisplayName("Bank Account Number")]
        public string? BankAccountNo { get; set; }
        [DisplayName("International Bank Account Number ")]
        public string? IBAN { get; set; }
        [DisplayName("SWIFT Code")]
        public string? SWIFTCode { get; set; }
        [DisplayName("N.S.S.F Number")]
        public string? NSSFNO { get; set; }
        [DisplayName("NHIF Number")]
        public string? NHIF { get; set; }
        [DisplayName("Company Email Address")]
        public string? CompanyEmail { get; set; }
        [DisplayName("KRA Pin")]
        public string? KRAPIN { get; set; }
        [DisplayName("Passport Number")]
        public string? PassportNo { get; set; }
        [DisplayName("Employment Terms")]
        public int? EmploymentTermsId { get; set; }
        public SystemCodeDetail EmploymentTerms { get; set; }
        [DisplayName("Allocated Leave Balance")]
        public decimal? AllocatedLeaveDays { get; set; }
        [DisplayName("Leave Balance")]
        public decimal? LeaveOutStandingBalance { get; set; }
    }

}
