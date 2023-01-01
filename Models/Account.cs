using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retail_Banking.Models
{
    public class Account
    {
        [Key]
        [Required(ErrorMessage ="Customer ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ID must be greater than 0.")]
        [Display(Name = "Customer ID")]
        public int CustomerID { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Account ID")]
        public int AccountID { get; set; }
        [Required(ErrorMessage ="Account Type is required.")]
        [RegularExpression("Checking|Saving",ErrorMessage =("Must be Checking or Saving."))]
        [Display(Name ="Account Type")]
        public string AccountType { get; set; }
        [Required(ErrorMessage = "Default balance is required.")]
        [Range(0,int.MaxValue,ErrorMessage ="Balance must be atleast 0.")]
        [Display(Name ="Account Balance")]
        [Column(TypeName ="decimal(18, 2)")]
        public decimal AccountBalance { get; set; }
        public string Status { get; set; }
    }
}
