using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Retail_Banking.Models;
using Retail_Banking.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;

namespace Retail_Banking.Controllers
{
    public class AccountController : Controller
    {
        CustomerInterface customerInterface;
        AccountInterface accountInterface;
        ErrorInterface errorInterface;
        private static int value = 0;
        public AccountController(CustomerInterface customerInterface, AccountInterface accountInterface, ErrorInterface errorInterface)
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
        public IActionResult ViewAllAccounts()
        {
            try
            {
                List<Account> accounts = new List<Account>();
                accounts = accountInterface.ViewAllAccounts();
                if (accounts.Count == 0)
                {
                    TempData["Response"] = errorInterface.GetErrorMessage(103);
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
        public IActionResult ViewAccounts(Customer customer)
        {
            try
            {
                CustomerAccount customerAccount = new CustomerAccount();
                customerAccount.customer = customer;
                customerAccount.accounts = accountInterface.GetAllCustomerAccounts(customerAccount.customer.CustomerID);
                if (customerAccount.accounts.Count < 1) TempData["Response"] = errorInterface.GetErrorMessage(103);
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
                CustomerAccountDetail customerAccountDetail = new CustomerAccountDetail();
                customerAccountDetail.customer = customer;
                return View(customerAccountDetail);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult AddAccounts(CustomerAccountDetail customerAccountDetail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CustomerAccount checkaccounts = new CustomerAccount();
                    checkaccounts.accounts = accountInterface.GetAllCustomerAccounts(customerAccountDetail.account.CustomerID);
                    if (checkaccounts.accounts.Count == 2)
                    {
                        TempData["Response"] = errorInterface.GetErrorMessage(100);
                        return RedirectToAction("AddAccount", "Account", customerAccountDetail.customer);
                    }
                    else if (checkaccounts.accounts.Count == 1 && customerAccountDetail.account.AccountType == "Checking" && checkaccounts.accounts[0].AccountType == "Checking")
                    {
                        TempData["Response"] = errorInterface.GetErrorMessage(101);
                        return RedirectToAction("AddAccount", "Account", customerAccountDetail.customer);
                    }
                    else if (checkaccounts.accounts.Count == 1 && customerAccountDetail.account.AccountType == "Saving" && checkaccounts.accounts[0].AccountType == "Saving")
                    {
                        TempData["Response"] = errorInterface.GetErrorMessage(102);
                        return RedirectToAction("AddAccount", "Account", customerAccountDetail.customer);
                    }
                    else
                    {
                        customerAccountDetail.account.Status = $"{customerAccountDetail.account.AccountType} created on {DateTime.Now}";
                        accountInterface.AddAccounts(customerAccountDetail.account);
                        return RedirectToAction("ViewAccounts", "Account", customerAccountDetail.customer);
                    }
                }
                else return RedirectToAction("AddAccount", "Account", customerAccountDetail.customer);
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
                Deposit deposit = new Deposit();
                deposit.Account = account;
                return View(deposit);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "Manager,Worker")]
        [HttpPost]
        public IActionResult DepositAccount(Deposit deposit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Account account = deposit.Account;
                    account.AccountBalance += deposit.Amount;
                    account.Status = $"Deposited {deposit.Amount} on {DateTime.Now}.";
                    return RedirectToAction("ViewAccounts", "Account", accountInterface.Deposit(account));
                }
                else
                {
                    if (deposit.Amount < 1) TempData["Response"] = errorInterface.GetErrorMessage(602);
                    return RedirectToAction("Deposit", "Account", deposit.Account);
                }
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "Manager,Worker,User")]
        public IActionResult Withdraw(Account account)
        {
            try
            {
                Withdraw withdraw = new Withdraw();
                withdraw.Account = account;
                return View(withdraw);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "Manager,Worker,User")]
        [HttpPost]
        public IActionResult WithdrawAccount(Withdraw withdraw)
        {
            try
            {
                if (withdraw.Amount > withdraw.Account.AccountBalance)
                {
                    TempData["Response"] = errorInterface.GetErrorMessage(600);
                    return RedirectToAction("Withdraw", "Account", withdraw.Account);
                }
                if (ModelState.IsValid)
                {
                    Account account = new Account();
                    account = withdraw.Account;
                    account.AccountBalance -= withdraw.Amount;
                    account.Status = $"Withdrew {withdraw.Amount} on {DateTime.Now}.";
                    return RedirectToAction("ViewAccounts", "Account", accountInterface.Withdraw(account));
                }
                else
                {
                    if (withdraw.Amount < 1) TempData["Response"] = errorInterface.GetErrorMessage(602);
                    return RedirectToAction("Withdraw", "Account", withdraw.Account);
                }
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Delete(Account account)
        {
            try
            {
                if (value == 1) TempData["Response"] = errorInterface.GetErrorMessage(603);
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
        public IActionResult DeleteAccount(Account account)
        {
            try
            {
                if (account.AccountBalance != 0)
                {
                    value = 1;
                    return RedirectToAction("Delete", "Account", account);
                }
                else
                {
                    Customer customer = customerInterface.GetCustomerByCustomerID(account.CustomerID);
                    accountInterface.Delete(account);
                    value = 2;
                    return RedirectToAction("ViewAccounts", "Account", customer);
                }
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "Manager,Worker")]
        public IActionResult SearchByAccountID()
        {
            try
            {
                if (value == 100) TempData["Response"] = errorInterface.GetErrorMessage(1000);
                value = 0;
                return View();
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "Manager,Worker,User")]
        public IActionResult FetchCustomerAndAccountInfo(int AccountID)
        {
            if(AccountID == 1 || AccountID == 2)
            {
                value = 100;
                return RedirectToAction("SearchByAccountID");
            }
            try
            {
                var details = accountInterface.GetCustomerAndAccountDetails(AccountID);
                if(details == null) TempData["Response"] = errorInterface.GetErrorMessage(103);
                return View(details);
            }
            catch
            {
                return RedirectToAction("SearchByAccountID", "Account");
            }
        }
    }
}
