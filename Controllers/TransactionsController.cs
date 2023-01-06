using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Retail_Banking.Models;
using Retail_Banking.ViewModels;
using System;
using System.Threading.Tasks;

namespace Retail_Banking.Controllers
{
    public class TransactionsController : Controller
    {
        IAccountInterface accountInterface;
        IErrorInterface errorInterface;
        ITransactionsInterface transactionsInterface;
        private static int value = 0;
        public TransactionsController(IAccountInterface accountInterface, IErrorInterface errorInterface,ITransactionsInterface transactionsInterface)
        {
            this.accountInterface = accountInterface;
            this.errorInterface = errorInterface;
            this.transactionsInterface = transactionsInterface;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles ="User")]
        public async Task<ActionResult> Transfer(Account account)
        {
            try
            {
                Transactions transactions = new()
                {
                    FromAccountID = account.AccountID
                };

                if (value == 1) TempData["Response"] = await errorInterface.GetErrorMessage(600);
                if (value == 2) TempData["Response"] = await errorInterface.GetErrorMessage(702);
                if (value == 3) TempData["Response"] = await errorInterface.GetErrorMessage(705);
                if (value == 4) TempData["Response"] = await errorInterface.GetErrorMessage(900);
                if (value == 5) TempData["Response"] = await errorInterface.GetErrorMessage(1000);
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
        public async Task<ActionResult> TransferMoney(Transactions transactions)
        {
            try
            {
                CustomerAccountDetail customerAccountDetail = await accountInterface.GetCustomerAndAccountDetails(transactions.FromAccountID);
                if (ModelState.IsValid)
                {
                    if (transactions.ToAccountID == 1 || transactions.ToAccountID == 2)
                    {
                        value = 5;
                        return RedirectToAction("Transfer", customerAccountDetail.account);
                    }
                    if (transactions.ToAccountID == transactions.FromAccountID)
                    {
                        value = 3;
                        return RedirectToAction("Transfer", customerAccountDetail.account);
                    }
                    if (customerAccountDetail.account.AccountBalance < transactions.Amount)
                    {
                        value = 1;
                        return RedirectToAction("Transfer", customerAccountDetail.account);
                    }
                    CustomerAccountDetail toCustomerAccountDetail = await accountInterface.GetCustomerAndAccountDetails(transactions.ToAccountID);
                    if (toCustomerAccountDetail == default || toCustomerAccountDetail.account == default)
                    {
                        value = 2;
                        return RedirectToAction("Transfer", customerAccountDetail.account);
                    }
                    
                    await transactionsInterface.Transfer(transactions);
                    return RedirectToAction("FetchCustomerAndAccountInfo", "Account", new { AccountID = transactions.FromAccountID });
                }
                else
                {
                    value = 4;
                    return RedirectToAction("Transfer", customerAccountDetail.account);
                }
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles ="Manager,Worker,User")]
        public async Task<ActionResult> ViewAllTransactions(Account account)
        {
            try
            {
                CustomerAccountTransactions customerAccountTransactions = await transactionsInterface.Transactions(account.AccountID);
                if (customerAccountTransactions.Transactions.Count == 0) TempData["Response"] = await errorInterface.GetErrorMessage(800);
                return View(customerAccountTransactions);
            }
            catch
            {
                return RedirectToAction("Error", "Error");            
            }
        }
    }
}
