using Retail_Banking.Controllers;
using Retail_Banking.ViewModels;
using System;
using System.Linq;

namespace Retail_Banking.Models
{
    public class TransactionsRepo:TransactionsInterface
    {
        ManagementContext managementContext;
        public TransactionsRepo(ManagementContext managementContext)
        {
            this.managementContext = managementContext;
        }

        public (Account, Account) FromAccountAndToAccountData(int FromAccountID, int ToAccountID)
        {
            return (managementContext.Account.Where(x => x.AccountID == FromAccountID).FirstOrDefault(), managementContext.Account.Where(x => x.AccountID == ToAccountID).FirstOrDefault());
        }

        public CustomerAccountTransactions Transactions(int AccountID)
        {
            var details = (from customer in managementContext.Customer
                           join account in managementContext.Account
                           on customer.CustomerID equals account.CustomerID
                           where account.AccountID == AccountID
                           select new CustomerAccountTransactions
                           {
                               Customer = customer,
                               Account = account
                           }).FirstOrDefault();
            details.Transactions = managementContext.Transactions.Where(x => x.FromAccountID == AccountID).ToList();
            return details;
        }

        public void Transfer(Transactions transactions)
        {
            (Account, Account) datas = FromAccountAndToAccountData(transactions.FromAccountID, transactions.ToAccountID);
            datas.Item1.AccountBalance -= transactions.Amount;
            datas.Item1.Status = $"Transfer {transactions.Amount} to Account ID {transactions.ToAccountID} on {transactions.Date}.";
            managementContext.Entry(datas.Item1).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            managementContext.SaveChanges();
            datas.Item2.AccountBalance += transactions.Amount;
            datas.Item2.Status = $"Received {transactions.Amount} from Account ID {transactions.FromAccountID} on {transactions.Date}.";
            managementContext.Entry(datas.Item2).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            managementContext.SaveChanges();
            transactions.Type = "From Transfer To";
            managementContext.Transactions.Add(transactions);
            managementContext.SaveChanges();
            Transactions nextTransaction = new Transactions();
            nextTransaction.ToAccountID = transactions.FromAccountID;
            nextTransaction.FromAccountID = transactions.ToAccountID;
            nextTransaction.Type = "To Received From";
            nextTransaction.Date = transactions.Date;
            nextTransaction.Amount = transactions.Amount;
            managementContext.Transactions.Add(nextTransaction);
            managementContext.SaveChanges();
        }
    }
}
