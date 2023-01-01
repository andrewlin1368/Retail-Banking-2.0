using Retail_Banking.ViewModels;
using System.Collections.Generic;

namespace Retail_Banking.Models
{
    public interface AccountInterface
    {
        public List<Account> GetAllCustomerAccounts(int CustomerID);
        public void AddAccounts(Account account);
        public Customer Deposit(Account account);
        public Customer Withdraw(Account account);
        public void Delete(Account account);
        public CustomerAccountDetail GetCustomerAndAccountDetails(int AccountID);
        public List<Account> ViewAllAccounts();
    }
}
