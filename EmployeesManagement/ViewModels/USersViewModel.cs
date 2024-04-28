using System.ComponentModel.DataAnnotations;

namespace EmployeesManagement.ViewModels
{
    public class UsersViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Middle Name")]

        public string MiddleName { get; set; }
        [Display(Name = "Last Name")]

        public string LastName { get; set; }
        [Display(Name = "Phone Number")]

        public string PhoneNumber { get; set; }
        [Display(Name = "Password")]

        public string Password { get; set; }
        [Display(Name = "Address")]

        public string Address { get; set; }
        [Display(Name = "User Name")]

        public string UserName { get; set; }

        [Display(Name = "National ID")]

        public string? NationalId { get; set; }
        public string FullName => $"{FirstName} {MiddleName} {LastName}";
        [Display(Name = "User Role")]

        public string? RoleId { get; set; }

    }
}
