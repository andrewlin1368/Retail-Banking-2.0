using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Retail_Banking.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retail_Banking.ViewModels
{
    public class Deposit
    {
        public Account Account { get; set; }
        [Required(ErrorMessage = "Amount is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be atleast 1.")]
        [Display(Name = "Deposit Amount")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
    }
}
