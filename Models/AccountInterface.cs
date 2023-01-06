using Retail_Banking.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Retail_Banking.Models
{
    public interface IAccountInterface
    {
        public Task<List<Account>> GetAllCustomerAccounts(int CustomerID);
        public Task AddAccounts(Account account);
        public Task<Customer> Deposit(Account account, decimal amount);
        public Task<Customer> Withdraw(Account account, decimal amount);
        public Task Delete(Account account);
        public Task<CustomerAccountDetail> GetCustomerAndAccountDetails(int AccountID);
        public Task<List<Account>> ViewAllAccounts();
    }
}
