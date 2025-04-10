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

        //GET api/customers/{id}
        [HttpGet("{id}", Name =nameof(GetCustomers))] // route med namn
        [ProducesResponseType(200, Type = typeof(Customer))]
        [ProducesResponseType(404)] // 404 response - Resource not found
        public async Task<IActionResult> GetCustomer(string id)
        {
            Customer? c = await _repo.RetrieveAsync(id);
            if(c is null)
            {
                return NotFound(); // 404 response - Resource not found
            }
            return Ok(c); // 200 response - Resource found
        }

        //POST api/customers
        //BODY: Customer(JSON,XML)
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Customer))]
        [ProducesResponseType(400)] // 400 response - Bad request
        public async Task<IActionResult> Create([FromBody] Customer c)
        {
            if (c is null)
                return BadRequest(); // 400 response - Bad request

            Customer? addedcustomer = await _repo.CreateAsync(c);
            if (addedcustomer is null)
            {
                return BadRequest("Repository failed to create the customer"); // 400 response - Bad request
            }
            else
            {
                return CreatedAtRoute( //201 response - Created
                    routeName: nameof(GetCustomers),
                    routeValues: new { id = addedcustomer.CustomerId },
                    value: addedcustomer); // response body

            }
        }

        //PUT api/customers/{id}
        //BODY: Customer(JSON,XML)
        [HttpPut("{id}")]
        [ProducesResponseType(204)] // 204 response - No content
        [ProducesResponseType(400)] // 400 response - Bad request
        [ProducesResponseType(404)] // 404 response - Resource not found
        public async Task<IActionResult> Update(string id, [FromBody] Customer c)
        {
            id = id.ToUpper();
            c.CustomerId = c.CustomerId.ToUpper();
            if(c == null || id != c.CustomerId)
                return BadRequest(); // 400 response - Bad request

            Customer? existing = await _repo.RetrieveAsync(id);
            if(existing is null)
            {
                return NotFound(); // 404 response - Resource not found
            }
            await _repo.UpdateAsync(c);

            return new NoContentResult(); // 204 response - No content

        }

        //DELETE api/customers/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // 204 response - No content
        [ProducesResponseType(400)] // 400 response - Bad request
        [ProducesResponseType(404)] // 404 response - Resource not found
        public async Task<IActionResult> Delete(string id)
        {
            if (id == "bad")
            {
                ProblemDetails problemDetails = new()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://localhost:5151/customers/failed-to-delete",
                    Title = $"Customer ID {id} found but failed to delete.",
                    Detail = "More details like Company Name, Country and so on.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails); // 400 Bad Request
            }

            Customer? existing = await _repo.RetrieveAsync(id);
            if (existing is null)
            {
                return NotFound(); // 404 response - Resource not found
            }

            bool? deleted = await _repo.DeleteAsync(id);
            if (deleted.HasValue && deleted.Value)
            {
                return new NoContentResult(); //204 No content
            }
            else
            {
                return BadRequest(
                    $"Customer {id} was found but failed to delete."); // 400 response - Bad request
            }
        
        }

    }
}

// Accepted - 202 - används med POST - för att skapa ett resurs som kan skapas snabbt. 
