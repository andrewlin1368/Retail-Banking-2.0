using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Retail_Banking.Models
{
    public class CustomerRepo:ICustomerInterface
    {
        ManagementContext ManagementContext;
        public CustomerRepo(ManagementContext managementContext)
        {
            ManagementContext = managementContext;
        }

        public async Task<int> CreateNewCustomer(Customer customer)
        {
            if(await ManagementContext.Customer.Where(c => c.SSNID == customer.SSNID).FirstOrDefaultAsync() == default)
            {
                customer.Status = $"{customer.SSNID} created on {DateTime.Now}.";
                await ManagementContext.Customer.AddAsync(customer);
                await ManagementContext.SaveChangesAsync();
                return 200;
            }
            return 1;
        }

        public async Task DeleteCustomer(Customer customer)
        {
            ManagementContext.Customer.Remove(customer);
            await ManagementContext.SaveChangesAsync();
        }

        public async Task<Customer> GetByEmail(string Email)
        {
            return await ManagementContext.Customer.Where(x => x.Email == Email).FirstOrDefaultAsync();
        }

        public async Task<Customer> GetCustomerByCustomerID(int ID)
        {
            return await ManagementContext.Customer.Where(x => x.CustomerID == ID).FirstOrDefaultAsync();
        }

        public async Task<(Customer,int)> GetCustomerBySSNIDorCustomerID(int ID, string Type)
        {
            Customer customer; 
            if (Type == "SSNID") customer = await ManagementContext.Customer.Where(x => x.SSNID == ID).FirstOrDefaultAsync();  
            else customer = await ManagementContext.Customer.Where(x => x.CustomerID == ID).FirstOrDefaultAsync();
            return customer == default && Type == "SSNID" ? (customer, 703) : customer == default && Type == "CustomerID" ? (customer, 701) : (customer, 200);
        }

        public async Task<Customer> UpdateCustomer(Customer customer)
        {
            customer.Status = $"Data updated on {DateTime.Now}.";
            ManagementContext.Entry(customer).State = EntityState.Modified;
            await ManagementContext.SaveChangesAsync();
            return await ManagementContext.Customer.Where(x => x.CustomerID == customer.CustomerID).FirstAsync();
        }

        public async Task<List<Customer>> ViewAllCustomers()
        {
            return await ManagementContext.Customer.Where(x=> x.CustomerID != 0).ToListAsync();
        }
    }
}
