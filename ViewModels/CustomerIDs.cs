using System.ComponentModel.DataAnnotations;

namespace Retail_Banking.ViewModel
{
    public class CustomerIDs
    {
        [Display(Name ="SSN ID")]
        [Required(ErrorMessage = "SSN ID is required.")]
        public int SSNID { get; set; }
        [Display(Name = "Customer ID")]
        [Required(ErrorMessage = "Customer ID is required.")]
        public int CustomerID { get; set; }
    }
}
