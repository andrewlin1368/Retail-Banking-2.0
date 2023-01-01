using Retail_Banking.Models;
using System.Collections.Generic;

namespace Retail_Banking.ViewModels
{
    public class CustomerAccountTransactions
    {
        public Customer Customer { get; set; }
        public Account Account { get; set; }
        public List<Transactions> Transactions { get; set; }
    }
}
