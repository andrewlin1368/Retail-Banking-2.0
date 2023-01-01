using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retail_Banking.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name ="Customer ID")]
        public int CustomerID { get; set; }
        [Required(ErrorMessage ="SSNID is required.")]
        [RegularExpression("^[0-9]{9,9}$",ErrorMessage ="SSNID must be 9 digits.")]
        [Display(Name ="SSN ID")]
        public int SSNID { get; set; }
        [Required(ErrorMessage ="Age is required.")]
        [Range(18,int.MaxValue,ErrorMessage ="Must be atleast 18 years old.")]
        [Display(Name ="Age")]
        public int Age { get; set; }
        [Required(ErrorMessage ="Name is required.")]
        [Display(Name ="Name")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Address is required.")]
        [Display(Name ="Address")]
        public string Address { get; set; }
        [Required(ErrorMessage ="Email is required")]
        [Display(Name ="Email Address")]
        public string Email { get; set; }
        public string Status { get; set; }
    }
}
