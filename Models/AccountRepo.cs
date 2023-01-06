using Microsoft.EntityFrameworkCore;
using Retail_Banking.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Retail_Banking.Models
{
    public class AccountRepo:IAccountInterface
    {
        ManagementContext managementContext;
        public AccountRepo(ManagementContext managementContext)
        {
            this.managementContext = managementContext;
        }

        public async Task AddAccounts(Account account)
        {
            account.Status = $"{account.AccountType} created on {DateTime.Now}";
            managementContext.Account.Add(account);
            await managementContext.SaveChangesAsync();
        }

        public async Task Delete(Account account)
        {
            managementContext.Account.Remove(account);
            await managementContext.SaveChangesAsync();
        }

        public async Task<Customer> Deposit(Account account, decimal amount)
        {
            account.AccountBalance += amount;
            account.Status = $"Deposited {amount} on {DateTime.Now}.";
            managementContext.Entry(account).State = EntityState.Modified;
            await managementContext.SaveChangesAsync();
            return await managementContext.Customer.Where(x => x.CustomerID == account.CustomerID).FirstAsync();
        }

        public async Task<List<Account>> GetAllCustomerAccounts(int CustomerID)
        {
            return await managementContext.Account.Where(x => x.CustomerID == CustomerID).ToListAsync();
        }

        public async Task<CustomerAccountDetail> GetCustomerAndAccountDetails(int AccountID)
        {
            return await (from account in managementContext.Account 
                          join customer in managementContext.Customer 
                          on account.CustomerID equals customer.CustomerID 
                          where account.AccountID == AccountID 
                          select new CustomerAccountDetail()
                          {
                              customer = customer,
                              account = account
                          }).FirstOrDefaultAsync();
        }

        public async Task<List<Account>> ViewAllAccounts()
        {
            return await managementContext.Account.Where(x => x.CustomerID!=0).ToListAsync();
        }

        public async Task<Customer> Withdraw(Account account,decimal amount)
        {
            account.AccountBalance -= amount;
            account.Status = $"Withdrew {amount} on {DateTime.Now}.";
            managementContext.Entry(account).State = EntityState.Modified;
            await managementContext.SaveChangesAsync();
            return await managementContext.Customer.Where(x => x.CustomerID == account.CustomerID).FirstAsync();
        }
    }
}
