
namespace Northwind.Blazor.Services
{
    public class NorthwindServiceServerSide : INorthwindService
    {
        public Task<Customer> CreateCustomerAsync(Customer c)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCustomerAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Customer?> GetCustomerAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Customer>> GetCustomersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Customer>> GetCustomersAsync(string country)
        {
            throw new NotImplementedException();
        }

        public Task<Customer> UpdateCustomerAsync(Customer c)
        {
            throw new NotImplementedException();
        }
    }
}
