using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Retail_Banking.Models;
using Retail_Banking.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Retail_Banking.Controllers
{
    public class CustomerController : Controller
    {
        ICustomerInterface customerInterface;
        IErrorInterface errorInterface;
        IAccountInterface accountInterface;
        private readonly UserManager<IdentityUser> userManager;
        public CustomerController(ICustomerInterface customerInterface, IErrorInterface errorInterface, IAccountInterface accountInterface, UserManager<IdentityUser> userManager)
        {
            this.customerInterface = customerInterface;
            this.errorInterface = errorInterface;
            this.accountInterface = accountInterface;
            this.userManager = userManager;
        }

        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> ViewAllCustomers()
        {
            try
            {
                List<Customer> customers = await customerInterface.ViewAllCustomers();
                if (customers.Count == 0)
                {
                    TempData["Response"] = await errorInterface.GetErrorMessage(105);
                    return View();
                }
                return View(customers);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles ="User")]
        public async Task<ActionResult> CreateNewCustomer()
        {
            try
            {
                Customer customer = new();
                var user = await userManager.GetUserAsync(HttpContext.User);
                customer.Email = user.Email;
                if (await customerInterface.GetByEmail(user.Email) != default) TempData["Response"] = await errorInterface.GetErrorMessage(1);
                return View(customer);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles ="User")]
        [HttpPost]
        public async Task<ActionResult> CreateNewCustomer(Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await customerInterface.CreateNewCustomer(customer) == 1)
                    {
                        TempData["Response"] = await errorInterface.GetErrorMessage(1);
                        return View(customer);
                    }
                    else
                    {
                        CustomerIDs customerIDs = new()
                        {
                            SSNID = customer.SSNID,
                            CustomerID = 0
                        };
                        return RedirectToAction("GetCustomer", customerIDs);
                    }
                }
                else return View(customer);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles ="Manager,Worker")]
        public IActionResult GetCustomerBySSNIDorCustomerID()
        {
            try
            {
                return View();
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles ="Manager,Worker,User")]
        public async Task<ActionResult> GetCustomer(CustomerIDs ids)
        {
            try
            {
                if (ids.CustomerID == 0 && ids.SSNID == 0)
                {
                    TempData["Response"] = await errorInterface.GetErrorMessage(704);
                    return RedirectToAction("GetCustomerBySSNIDorCustomerID", ids);
                }
                if (ModelState.IsValid)
                {
                    (Customer findcustomer, int ErrorID) data;
                    
                    if (ids.CustomerID == 0) data = await customerInterface.GetCustomerBySSNIDorCustomerID(ids.SSNID, "SSNID");
                    else data = await customerInterface.GetCustomerBySSNIDorCustomerID(ids.CustomerID, "CustomerID");

                    if (data.findcustomer == default)
                    {
                        TempData["Response"] = await errorInterface.GetErrorMessage(data.ErrorID);
                        return RedirectToAction("GetCustomerBySSNIDorCustomerID", ids);
                    }
                    else return RedirectToAction("ViewCustomer", data.findcustomer);
                }
                else return RedirectToAction("GetCustomerBySSNIDorCustomerID", ids);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles ="User")]
        public async Task<ActionResult> ViewInfo()
        {
            try
            {
                var user = await userManager.GetUserAsync(HttpContext.User);
                var email = user.Email;
                Customer customer = await customerInterface.GetByEmail(email);
                if (customer == default) TempData["Response"] = await errorInterface.GetErrorMessage(2);
                return RedirectToAction("ViewCustomer", customer);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "Manager,Worker,User")]
        public IActionResult ViewCustomer(Customer customer)
        {
            try
            {
                return View(customer);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles ="Manager,Worker,User")]
        public IActionResult Update(Customer customer)
        {
            try
            {
                return View(customer);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }   
        }

        [Authorize(Roles = "Manager,Worker,User")]
        [HttpPost]
        public async Task<ActionResult> UpdateCustomer(Customer customer)
        {
            try
            {
                return ModelState.IsValid ? RedirectToAction("ViewCustomer", await customerInterface.UpdateCustomer(customer)) : RedirectToAction("Update", customer);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> Delete(Customer customer)
        {
            try
            {
                List<Account> accounts = await accountInterface.GetAllCustomerAccounts(customer.CustomerID);
                if (accounts.Count > 0) TempData["Response"] = await errorInterface.GetErrorMessage(104);
                return View(customer);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "Manager")]
        public IActionResult ConfirmDelete(Customer customer)
        {
            try
            {
                return View(customer);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<ActionResult> DeleteCustomer(Customer customer)
        {
            try
            {
                await customerInterface.DeleteCustomer(customer);
                TempData["Response"] = "Customer Deleted Successfully.";
                return View();
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }
    }
}
