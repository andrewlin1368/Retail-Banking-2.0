using Retail_Banking.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Retail_Banking.Models
{
    public class AccountRepo:AccountInterface
    {
        ManagementContext managementContext;
        public AccountRepo(ManagementContext managementContext)
        {
            this.managementContext = managementContext;
        }

        public void AddAccounts(Account account)
        {
            managementContext.Account.Add(account);
            managementContext.SaveChanges();
        }

        public void Delete(Account account)
        {
            managementContext.Account.Remove(account);
            managementContext.SaveChanges();
        }

        public Customer Deposit(Account account)
        {
            managementContext.Entry(account).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            managementContext.SaveChanges();
            return managementContext.Customer.Where(x => x.CustomerID == account.CustomerID).FirstOrDefault();
        }

        public List<Account> GetAllCustomerAccounts(int CustomerID)
        {
            return managementContext.Account.Where(x => x.CustomerID == CustomerID).ToList();
        }

        public CustomerAccountDetail GetCustomerAndAccountDetails(int AccountID)
        {
            try
            {
                var details = (from account in managementContext.Account
                               join customer in managementContext.Customer
                               on account.CustomerID equals customer.CustomerID
                               where account.AccountID == AccountID
                               select new CustomerAccountDetail()
                               {
                                   customer = customer,
                                   account = account
                               }).FirstOrDefault();
                return details;
            }
            catch
            {
                return null;
            }
        }

        public List<Account> ViewAllAccounts()
        {
            return managementContext.Account.Where(x => x.AccountID != 1 && x.AccountID != 2).ToList();
        }

        public Customer Withdraw(Account account)
        {
            managementContext.Entry(account).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            managementContext.SaveChanges();
            return managementContext.Customer.Where(x => x.CustomerID == account.CustomerID).FirstOrDefault();
        }
    }
}
