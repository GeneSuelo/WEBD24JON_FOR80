using Microsoft.AspNetCore.Mvc;
using Northwind.EntityModels;
using Northwind.WebApi.Repositories;

namespace Northwind.WebApi.Controllers
{
    //api/customers
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _repo;
        
        //Konstruktör lägger till repository som är registrerad i Program.cs
        public CustomersController(ICustomerRepository repo)
        {
            _repo = repo;
        }

        //GET api/customers
        //GET api/customers?country=[country]
        ///Returnerar lista med customers (men det kan vara tom/null)
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
        public async Task<IEnumerable<Customer>> GetCustomers(string? country)
        {
            if(string.IsNullOrEmpty(country))
            {
                return await _repo.RetrieveAllAsync();
            }
            else
            {
                return (await _repo.RetrieveAllAsync())
                    .Where(customer => customer.Country == country);
            }
        }

    }
}
