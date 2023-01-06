using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Retail_Banking.Models;
using Retail_Banking.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Retail_Banking.Controllers
{
    public class AccountController : Controller
    {
        ICustomerInterface customerInterface;
        IAccountInterface accountInterface;
        IErrorInterface errorInterface;
        private static int value = 0;
        public AccountController(ICustomerInterface customerInterface, IAccountInterface accountInterface, IErrorInterface errorInterface)
        {
            this.customerInterface = customerInterface; 
            this.accountInterface = accountInterface;
            this.errorInterface = errorInterface;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> ViewAllAccounts()
        {
            try
            {
                List<Account> accounts = await accountInterface.ViewAllAccounts();
                if (accounts.Count == 0)
                {
                    TempData["Response"] = await errorInterface.GetErrorMessage(103);
                    return View();
                }
                return View(accounts);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "Manager,Worker,User")]
        public async Task<ActionResult> ViewAccounts(Customer customer)
        {
            try
            {
                CustomerAccount customerAccount = new()
                {
                    customer = customer,
                    accounts = await accountInterface.GetAllCustomerAccounts(customer.CustomerID)
                };
                if (customerAccount.accounts.Count < 1) TempData["Response"] = await errorInterface.GetErrorMessage(103);
                if (value == 2) TempData["_response"] = "Account deleted successfully."; 
                value = 0;
                return View(customerAccount);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "User")]
        public IActionResult AddAccount(Customer customer)
        {
            try
            {
                CustomerAccountDetail customerAccountDetail = new()
                {
                    customer = customer
                };
                return View(customerAccountDetail);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<ActionResult> AddAccounts(CustomerAccountDetail customerAccountDetail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CustomerAccount checkaccounts = new()
                    {
                        accounts = await accountInterface.GetAllCustomerAccounts(customerAccountDetail.account.CustomerID)
                    };
                    if (checkaccounts.accounts.Count == 2)
                    {
                        TempData["Response"] = await errorInterface.GetErrorMessage(100);
                        return RedirectToAction("AddAccount", customerAccountDetail.customer);
                    }
                    else if (checkaccounts.accounts.Count == 1 && customerAccountDetail.account.AccountType == "Checking" && checkaccounts.accounts[0].AccountType == "Checking")
                    {
                        TempData["Response"] = await errorInterface.GetErrorMessage(101);
                        return RedirectToAction("AddAccount", customerAccountDetail.customer);
                    }
                    else if (checkaccounts.accounts.Count == 1 && customerAccountDetail.account.AccountType == "Saving" && checkaccounts.accounts[0].AccountType == "Saving")
                    {
                        TempData["Response"] = await errorInterface.GetErrorMessage(102);
                        return RedirectToAction("AddAccount", customerAccountDetail.customer);
                    }
                    else
                    {
                        await accountInterface.AddAccounts(customerAccountDetail.account);
                        return RedirectToAction("ViewAccounts", customerAccountDetail.customer);
                    }
                }
                else return RedirectToAction("AddAccount", customerAccountDetail.customer);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "Manager,Worker")]
        public IActionResult Deposit(Account account)
        {
            try
            {
                Deposit deposit = new()
                {
                    Account = account
                };
                return View(deposit);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "Manager,Worker")]
        [HttpPost]
        public async Task<ActionResult> DepositAccount(Deposit deposit)
        {
            try
            {
                if (ModelState.IsValid) return RedirectToAction("ViewAccounts", await accountInterface.Deposit(deposit.Account,deposit.Amount));
                else
                {
                    if (deposit.Amount < 1) TempData["Response"] = await errorInterface.GetErrorMessage(602);
                    return RedirectToAction("Deposit", deposit.Account);
                }
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "User")]
        public IActionResult Withdraw(Account account)
        {
            try
            {
                Withdraw withdraw = new()
                {
                    Account = account
                };
                return View(withdraw);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<ActionResult> WithdrawAccount(Withdraw withdraw)
        {
            try
            {
                if (withdraw.Amount > withdraw.Account.AccountBalance)
                {
                    TempData["Response"] = await errorInterface.GetErrorMessage(600);
                    return RedirectToAction("Withdraw", withdraw.Account);
                }
                if (withdraw.Amount < 1)
                {
                    TempData["Response"] = await errorInterface.GetErrorMessage(602);
                    return RedirectToAction("Withdraw", withdraw.Account);
                }
                return ModelState.IsValid ? RedirectToAction("ViewAccounts", await accountInterface.Withdraw(withdraw.Account,withdraw.Amount)) : RedirectToAction("Withdraw", withdraw.Account);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> Delete(Account account)
        {
            try
            {
                if (value == 1) TempData["Response"] = await errorInterface.GetErrorMessage(603);
                value = 0;
                return View(account);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<ActionResult> DeleteAccount(Account account)
        {
            try
            {
                if (account.AccountBalance != 0)
                {
                    value = 1;
                    return RedirectToAction("Delete", account);
                }
                else
                {
                    await accountInterface.Delete(account);
                    value = 2;
                    return RedirectToAction("ViewAccounts", await customerInterface.GetCustomerByCustomerID(account.CustomerID));
                }
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "Manager,Worker")]
        public async Task<ActionResult> SearchByAccountID()
        {
            try
            {
                if (value == 100) TempData["Response"] = await errorInterface.GetErrorMessage(1000);
                value = 0;
                return View();
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "Manager,Worker,User")]
        public async Task<ActionResult> FetchCustomerAndAccountInfo(int AccountID)
        {
            try
            {
                if (AccountID == 1 || AccountID == 2)
                {
                    value = 100;
                    return RedirectToAction("SearchByAccountID");
                }
                CustomerAccountDetail details = await accountInterface.GetCustomerAndAccountDetails(AccountID);
                if(details == default) TempData["Response"] = await errorInterface.GetErrorMessage(103);
                return View(details);
            }
            catch
            {
                return RedirectToAction("SearchByAccountID", "Account");
            }
        }
    }
}
