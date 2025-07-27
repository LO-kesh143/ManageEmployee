using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Web.ViewModels
{
    public class EmployeeViewModel
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "SSN is required.")]
        [StringLength(11, ErrorMessage = "SSN must be in the numeric format XXX-XX-XXXX.")]
        [RegularExpression(@"^\d{3}-\d{2}-\d{4}$", ErrorMessage = "SSN must be in the format XXX-XX-XXXX.")]
        public string SSN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DOB { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address can't exceed 200 characters.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, ErrorMessage = "City can't exceed 50 characters.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required.")]
        [StringLength(50, ErrorMessage = "State can't exceed 50 characters.")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "ZIP code is required.")]
        [StringLength(10, ErrorMessage = "ZIP can be in 5-digit or 9-digit")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "ZIP must be in 5-digit or 9-digit format (e.g., 12345 or 12345-6789).")]
        public string Zip { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(10, ErrorMessage = "Phone number must be exactly 10 digits.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be numberic 10 digits.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Join Date is required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? JoinDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ExitDate { get; set; }

        public List<EmployeeSalaryViewModel> Salaries { get; set; } = new List<EmployeeSalaryViewModel>();
    }
}
