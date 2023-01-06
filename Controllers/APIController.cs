using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retail_Banking.Models;
using Retail_Banking.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Retail_Banking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIController : ControllerBase
    {
        ICustomerInterface customerInterface;
        IErrorInterface errorInterface;
        public APIController(ICustomerInterface customerInterface, IErrorInterface errorInterface)
        {
            this.customerInterface = customerInterface;
            this.errorInterface = errorInterface;
        }

        [HttpGet("GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                List<Customer> customers = await customerInterface.ViewAllCustomers();
                if (customers.Count == 0)
                {
                    return BadRequest(await errorInterface.GetErrorMessage(105));
                }
                return Ok(customers);
            }
            catch
            {
                return BadRequest(RedirectToAction("Error", "Error"));
            }
        }

        [HttpGet("GetCustomer/{CustomerID}")]
        public async Task<IActionResult> GetCustomer(int CustomerID)
        {
            try
            {
                if (CustomerID == 0) return BadRequest(await errorInterface.GetErrorMessage(704));
                (Customer,int) customerData = await customerInterface.GetCustomerBySSNIDorCustomerID(CustomerID, "CustomerID");
                if (customerData.Item1 == default) return BadRequest(await errorInterface.GetErrorMessage(customerData.Item2));
                return Ok(customerData.Item1);
            }
            catch
            {
                return BadRequest(RedirectToAction("Error", "Error"));
            }
        }

        [HttpGet("GetCustomerBySSN/{SSNID}")]
        public async Task<IActionResult> GetCustomerBySSN(int SSNID)
        {
            try
            {
                if (SSNID == 0) return BadRequest(await errorInterface.GetErrorMessage(704));
                (Customer, int) customerData = await customerInterface.GetCustomerBySSNIDorCustomerID(SSNID, "SSNID");
                if (customerData.Item1 == default) return BadRequest(await errorInterface.GetErrorMessage(customerData.Item2));
                return Ok(customerData.Item1);
            }
            catch
            {
                return BadRequest(RedirectToAction("Error", "Error"));
            }
        }

        [HttpPost("AddCustomerWihoutAuthorization")]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            try
            {
                return (await customerInterface.CreateNewCustomer(customer) == 1) ? BadRequest(await errorInterface.GetErrorMessage(1)) : Ok(200);
            }
            catch
            {
                return BadRequest(RedirectToAction("Error", "Error"));
            }
        }

        [HttpDelete("DeleteWithoutAuthorization/{CustomerID}")]
        public async Task<IActionResult> DeleteCustomer(int CustomerID)
        {
            try
            {
                await customerInterface.DeleteCustomer(await customerInterface.GetCustomerByCustomerID(CustomerID));
                return Ok(200);
            }
            catch
            {
                return BadRequest(RedirectToAction("Error", "Error"));
            }
        }

        [HttpPut("UpdateWithoutAuthorization")]
        public async Task<IActionResult> UpdateCustomer(Customer customer)
        {
            try
            {
                return Ok(await customerInterface.UpdateCustomer(customer));
            }
            catch
            {
                return BadRequest(RedirectToAction("Error", "Error"));
            }
        }
    }
}
