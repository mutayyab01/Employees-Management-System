using System.ComponentModel;

namespace EmployeesManagement.Models
{
    public class CompanyInformation
    {
        public int Id { get; set; }
        [DisplayName("Company Name")]
        public string Name { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; }
        [DisplayName("Phone Number")]
        public int PhoneNo { get; set; }
        [DisplayName("N.S.S.F Number")]
        public string NSSFNO { get; set; }
        [DisplayName("N.H.I.F Number")]
        public string NHIFNO { get; set; }
        [DisplayName("K.R.A Pin")]
        public string KRAPIN { get; set; }
        [DisplayName("Contact Person")]
        public string ContactPerson { get; set; }
        public string Logo { get; set; }
        [DisplayName("Postal Code")]
        public string PostalCode { get; set; }
        [DisplayName("City Name")]
        public int CityId { get; set; }
        public City City { get; set; }
        [DisplayName("Country Name")]
        public int CountryId { get; set; }
        public Country Country { get; set; }

    }
}
