using System;
using System.Collections.Generic;

namespace Retail_Banking.Models
{
    public interface CustomerInterface
    {
        public List<Customer> ViewAllCustomers();
        public int CreateNewCustomer(Customer customer);
        public (Customer,int) GetCustomerBySSNIDorCustomerID(int ID,string Type);
        public Customer GetCustomerByCustomerID(int ID);
        public Customer UpdateCustomer(Customer customer);
        public void DeleteCustomer(Customer customer);
        public Customer GetByEmail(string Email);
    }
}
