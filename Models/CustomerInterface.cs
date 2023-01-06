using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Retail_Banking.Models
{
    public interface ICustomerInterface
    {
        public Task<List<Customer>> ViewAllCustomers();
        public Task<int> CreateNewCustomer(Customer customer);
        public Task<(Customer,int)> GetCustomerBySSNIDorCustomerID(int ID,string Type);
        public Task<Customer> GetCustomerByCustomerID(int ID);
        public Task<Customer> UpdateCustomer(Customer customer);
        public Task DeleteCustomer(Customer customer);
        public Task<Customer> GetByEmail(string Email);
    }
}
