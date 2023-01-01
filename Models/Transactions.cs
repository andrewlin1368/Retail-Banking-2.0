using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retail_Banking.Models
{
    public class Transactions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name ="Transaction ID")]
        public int TransactionID { get; set; }
        [Display(Name ="Date")]
        public DateTime Date { get; set; }
        [Display(Name ="Transaction Type")]
        public string Type { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be atleast 1.")]
        [Required(ErrorMessage = "Amount is required.")]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name ="Amount")]
        public decimal Amount { get; set; }
        [Display(Name ="From Account ID")]
        public int FromAccountID { get; set; }
        [Required(ErrorMessage = "Destination AccountID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Account ID not found.")]
        [Display(Name = "To Account ID")]
        public int ToAccountID { get; set; }
    }
}
