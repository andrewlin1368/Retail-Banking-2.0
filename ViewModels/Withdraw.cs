using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Retail_Banking.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Retail_Banking.ViewModels
{
    public class Withdraw
    {
        public Account Account { get; set; }
        [Required(ErrorMessage = "Amount is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be atleast 1.")]
        [Display(Name = "Withdrawal Amount")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
    }
}
