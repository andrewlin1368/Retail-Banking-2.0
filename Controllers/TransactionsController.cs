using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Retail_Banking.Models;
using Retail_Banking.ViewModels;
using System;

namespace Retail_Banking.Controllers
{
    public class TransactionsController : Controller
    {
        AccountInterface accountInterface;
        CustomerInterface customerInterface;
        ErrorInterface errorInterface;
        TransactionsInterface transactionsInterface;
        private static int value = 0;
        public TransactionsController(AccountInterface accountInterface, CustomerInterface customerInterface, ErrorInterface errorInterface,TransactionsInterface transactionsInterface)
        {
            this.accountInterface = accountInterface;
            this.customerInterface = customerInterface;
            this.errorInterface = errorInterface;
            this.transactionsInterface = transactionsInterface;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles ="User")]
        public IActionResult Transfer(Account account)
        {
            try
            {
                Transactions transactions = new Transactions();
                transactions.FromAccountID = account.AccountID;
                if (value == 1) TempData["Response"] = errorInterface.GetErrorMessage(600);
                if (value == 2) TempData["Response"] = errorInterface.GetErrorMessage(702);
                if (value == 3) TempData["Response"] = errorInterface.GetErrorMessage(705);
                if (value == 4) TempData["Response"] = errorInterface.GetErrorMessage(900);
                if (value == 5) TempData["Response"] = errorInterface.GetErrorMessage(1000);
                value = 0;
                return View(transactions);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles ="User")]
        [HttpPost]
        public IActionResult TransferMoney(Transactions transactions)
        {
            try
            {
                Account account = accountInterface.GetCustomerAndAccountDetails(transactions.FromAccountID).account;
                if (ModelState.IsValid)
                {
                    if (transactions.ToAccountID == 1 || transactions.ToAccountID == 2)
                    {
                        value = 5;
                        return RedirectToAction("Transfer", account);
                    }
                    if (transactions.ToAccountID == transactions.FromAccountID)
                    {
                        value = 3;
                        return RedirectToAction("Transfer", "Transactions", account);
                    }
                    if (account.AccountBalance < transactions.Amount)
                    {
                        value = 1;
                        return RedirectToAction("Transfer", "Transactions", account);
                    }
                    CustomerAccountDetail customerAccountDetail = accountInterface.GetCustomerAndAccountDetails(transactions.ToAccountID);
                    if (customerAccountDetail == null || customerAccountDetail.account == null)
                    {
                        value = 2;
                        return RedirectToAction("Transfer", "Transactions", account);
                    }
                    transactions.Date = DateTime.Now;
                    transactionsInterface.Transfer(transactions);
                    return RedirectToAction("FetchCustomerAndAccountInfo", "Account", new { AccountID = transactions.FromAccountID });
                }
                else
                {
                    value = 4;
                    return RedirectToAction("Transfer", "Transactions", account);
                }
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles ="Manager,Worker,User")]
        public IActionResult ViewAllTransactions(Account account)
        {
            try
            {
                CustomerAccountTransactions customerAccountTransactions = transactionsInterface.Transactions(account.AccountID);
                if (customerAccountTransactions.Transactions.Count == 0) TempData["Response"] = errorInterface.GetErrorMessage(800);
                return View(customerAccountTransactions);
            }
            catch
            {
                return RedirectToAction("Error", "Error");            }
        }
    }
}
