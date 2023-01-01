using Retail_Banking.Models;
using System.Collections.Generic;

namespace Retail_Banking.ViewModels
{
    public class CustomerAccount
    {
        public Customer customer { get; set; }
        public Account account { get; set; }
        public List<Account> accounts { get; set; }
        public enum AccountType 
        {
            Checking,
            Saving
        }
    }
}
