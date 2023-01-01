using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Retail_Banking.Models
{
    public class CustomerRepo:CustomerInterface
    {
        ManagementContext ManagementContext;
        public CustomerRepo(ManagementContext managementContext)
        {
            ManagementContext = managementContext;
        }

        public int CreateNewCustomer(Customer customer)
        {
            Customer customer1 = ManagementContext.Customer.Where(c => c.SSNID == customer.SSNID).FirstOrDefault();
            if (customer1 != null) return 1;
            ManagementContext.Customer.Add(customer);
            ManagementContext.SaveChanges();
            return 200;
        }

        public void DeleteCustomer(Customer customer)
        {
            ManagementContext.Customer.Remove(customer);
            ManagementContext.SaveChanges();
        }

        public Customer GetByEmail(string Email)
        {
            return ManagementContext.Customer.Where(x => x.Email == Email).FirstOrDefault();
        }

        public Customer GetCustomerByCustomerID(int ID)
        {
            return ManagementContext.Customer.Where(x => x.CustomerID == ID).FirstOrDefault();
        }

        public (Customer,int) GetCustomerBySSNIDorCustomerID(int ID, string Type)
        {
            Customer customer; 
            if (Type == "SSNID") customer = ManagementContext.Customer.Where(x => x.SSNID == ID).FirstOrDefault();  
            else customer = ManagementContext.Customer.Where(x => x.CustomerID == ID).FirstOrDefault();
            return customer == null && Type == "SSNID" ? (null, 703) : customer == null && Type == "CustomerID" ? (null,701) : (customer, 200);
        }

        public Customer UpdateCustomer(Customer customer)
        {
            ManagementContext.Entry(customer).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            ManagementContext.SaveChanges();
            return ManagementContext.Customer.Where(x => x.CustomerID == customer.CustomerID).FirstOrDefault();
        }

        public List<Customer> ViewAllCustomers()
        {
            return ManagementContext.Customer.Where(x=> x.CustomerID != 0).ToList();
        }
    }
}
