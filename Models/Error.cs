using System.ComponentModel.DataAnnotations;

namespace Retail_Banking.Models
{
    public class Error
    {
        [Key]
        [Display(Name = "Error ID")]
        public int ErrorID { get; set; }
        [Display(Name ="Error Message")]
        public string ErrorMessage { get; set; }
    }
}
