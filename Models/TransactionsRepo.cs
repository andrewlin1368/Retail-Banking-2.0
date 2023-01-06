using Microsoft.EntityFrameworkCore;
using Retail_Banking.Controllers;
using Retail_Banking.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Retail_Banking.Models
{
    public class TransactionsRepo:ITransactionsInterface
    {
        ManagementContext managementContext;
        public TransactionsRepo(ManagementContext managementContext)
        {
            this.managementContext = managementContext;
        }

        public async Task<(Account, Account)> FromAccountAndToAccountData(int FromAccountID, int ToAccountID)
        {
            return (await managementContext.Account.Where(x => x.AccountID == FromAccountID).FirstOrDefaultAsync(), await managementContext.Account.Where(x => x.AccountID == ToAccountID).FirstOrDefaultAsync());
        }

        public async Task<CustomerAccountTransactions> Transactions(int AccountID)
        {
            CustomerAccountTransactions details = await (from customer in managementContext.Customer
                                                         join account in managementContext.Account
                                                         on customer.CustomerID equals account.CustomerID
                                                         where account.AccountID == AccountID
                                                         select new CustomerAccountTransactions
                                                         {
                                                            Customer = customer,
                                                            Account = account
                                                         }).FirstOrDefaultAsync();
            details.Transactions = await managementContext.Transactions.Where(x => (x.FromAccountID == AccountID ||  x.ToAccountID == AccountID)).ToListAsync();
            return details;
        }

        public async Task Transfer(Transactions transactions)
        {
            transactions.Date = DateTime.Now;
            (Account, Account) datas = await FromAccountAndToAccountData(transactions.FromAccountID, transactions.ToAccountID);

            datas.Item1.AccountBalance -= transactions.Amount;
            datas.Item1.Status = $"Transfer {transactions.Amount} to Account ID {transactions.ToAccountID} on {transactions.Date}.";
            managementContext.Entry(datas.Item1).State = EntityState.Modified;
            await managementContext.SaveChangesAsync();

            datas.Item2.AccountBalance += transactions.Amount;
            datas.Item2.Status = $"Received {transactions.Amount} from Account ID {transactions.FromAccountID} on {transactions.Date}.";
            managementContext.Entry(datas.Item2).State = EntityState.Modified;
            await managementContext.SaveChangesAsync();

            transactions.Type = $"Acc ID: {transactions.FromAccountID} Transfer To Acc ID: {transactions.ToAccountID}";
            managementContext.Transactions.Add(transactions);
            await managementContext.SaveChangesAsync();
        }
    }
}
