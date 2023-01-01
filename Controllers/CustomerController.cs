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
        CustomerInterface customerInterface;
        ErrorInterface errorInterface;
        AccountInterface accountInterface;
        private readonly UserManager<IdentityUser> userManager;
        public CustomerController(CustomerInterface customerInterface, ErrorInterface errorInterface, AccountInterface accountInterface, UserManager<IdentityUser> userManager)
        {
            this.customerInterface = customerInterface;
            this.errorInterface = errorInterface;
            this.accountInterface = accountInterface;
            this.userManager = userManager;
        }

        [Authorize(Roles = "Manager")]
        public IActionResult ViewAllCustomers()
        {
            try
            {
                List<Customer> customers = new List<Customer>();
                customers = customerInterface.ViewAllCustomers();
                if (customers.Count == 0)
                {
                    TempData["Response"] = errorInterface.GetErrorMessage(105);
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
                Customer customer = new Customer();
                var user = await userManager.GetUserAsync(HttpContext.User);
                customer.Email = user.Email;
                if (customerInterface.GetByEmail(user.Email) != null) TempData["Response"] = errorInterface.GetErrorMessage(1);
                return View(customer);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles ="User")]
        [HttpPost]
        public IActionResult CreateNewCustomer(Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    customer.Status = $"{customer.SSNID} created on {DateTime.Now}.";
                    int ErrorID = customerInterface.CreateNewCustomer(customer);
                    if (ErrorID == 1)
                    {
                        TempData["Response"] = errorInterface.GetErrorMessage(ErrorID);
                        return View(customer);
                    }
                    else
                    {
                        CustomerIDs customerIDs = new CustomerIDs();
                        customerIDs.SSNID = customer.SSNID;
                        customerIDs.CustomerID = 0;
                        return RedirectToAction("GetCustomer", "Customer", customerIDs);
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
            return View();
        }

        [Authorize(Roles ="Manager,Worker,User")]
        public IActionResult GetCustomer(CustomerIDs ids)
        {
            if (ids.CustomerID == 0 && ids.SSNID == 0)
            {
                TempData["Response"] = errorInterface.GetErrorMessage(704);
                return RedirectToAction("GetCustomerBySSNIDorCustomerID", ids);
            }
            try
            {
                if (ModelState.IsValid)
                {
                    (Customer findcustomer, int ErrorID) data;
                    
                    if (ids.CustomerID == 0) data = customerInterface.GetCustomerBySSNIDorCustomerID(ids.SSNID, "SSNID");
                    else data = customerInterface.GetCustomerBySSNIDorCustomerID(ids.CustomerID, "CustomerID");

                    if (data.findcustomer == null)
                    {
                        TempData["Response"] = errorInterface.GetErrorMessage(data.ErrorID);
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
                Customer customer = customerInterface.GetByEmail(email);
                if (customer == null) TempData["Response"] = errorInterface.GetErrorMessage(2);
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
        public IActionResult UpdateCustomer(Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    customer.Status = $"Data updated on {DateTime.Now}.";
                    Customer updatedcustomer = customerInterface.UpdateCustomer(customer);
                    return RedirectToAction("ViewCustomer", updatedcustomer);
                }
                else return RedirectToAction("Update", customer);
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Delete(Customer customer)
        {
            try
            {
                List<Account> accounts = accountInterface.GetAllCustomerAccounts(customer.CustomerID);
                if (accounts.Count > 0) TempData["Response"] = errorInterface.GetErrorMessage(104);
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
        public IActionResult DeleteCustomer(Customer customer)
        {
            try
            {
                customerInterface.DeleteCustomer(customer);
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
