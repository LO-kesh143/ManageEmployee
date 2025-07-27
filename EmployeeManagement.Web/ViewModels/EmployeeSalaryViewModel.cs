using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Web.ViewModels
{
    public class EmployeeSalaryViewModel
    {
        [Key]
        public int EmployeeSalaryId { get; set; }
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Salary is required.")]
        [Range(0, 10000000, ErrorMessage = "Salary must be between 0 and 10,000,000.")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "From Date is required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ToDate { get; set; }

        //public EmployeeViewModel EmployeeViewModel { get; set; } = null!;
    }
}
