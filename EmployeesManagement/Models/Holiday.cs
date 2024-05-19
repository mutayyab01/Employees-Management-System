using System.ComponentModel.DataAnnotations;

namespace EmployeesManagement.Models
{
    public class Holiday:UserActivity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]

        public DateTime StartDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]

        public DateTime EndDate { get; set; }

        public string Description { get; set; }



    }
}
