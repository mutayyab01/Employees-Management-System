using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;

namespace EmployeesManagement.Models
{
    public class LeaveAdjustmentEntry :UserActivity
    {
        public int Id { get; set; }
        [DisplayName("Leave Period")]
        public int? LeavePeriodId { get; set; }
        public LeavePeriod LeavePeriod { get; set; }
        [DisplayName("Employee Name")]

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        [DisplayName("No. Of Days")]

        public decimal NoOfDays { get; set; }
        [DisplayName("Leave Adjustment Date")]

        public DateTime LeaveAdjustmentDate { get; set; }
        [DisplayName("Leave Start Date")]

        public DateTime? LeaveStartDate { get; set; }
        [DisplayName("Leave End Date")]

        public DateTime? LeaveEndDate { get; set; }
        [DisplayName("Adjustment Description")]

        public string AdjustmentDescription { get; set; }
        [DisplayName("Adjustment Type")]

        public int AdjustmentTypeId { get; set; }
        public SystemCodeDetail AdjustmentType { get; set; }

    }
}
