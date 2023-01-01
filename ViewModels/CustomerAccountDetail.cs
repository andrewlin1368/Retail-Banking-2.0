using Retail_Banking.Models;

namespace Retail_Banking.ViewModels
{
    public class CustomerAccountDetail
    {
        public Customer customer { get; set; }
        public Account account { get; set; }
        public enum AccountType
        {
            Checking,
            Saving
        }
    }
}
