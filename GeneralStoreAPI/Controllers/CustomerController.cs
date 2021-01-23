using GeneralStoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GeneralStoreAPI.Controllers
{
    public class CustomerController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        // POST (Create)
        // api/Customer
        [HttpPost]
        public async Task<IHttpActionResult> CreateCustomer([FromBody] Customer model)
        {
            if (model is null)
            {
                return BadRequest("The request body cannot be empty");
            }

            if (ModelState.IsValid)
            {
                _context.Customers.Add(model);
                int changeCount = await _context.SaveChangesAsync();
                return Ok("The customer was successfully created");
            }

            return BadRequest(ModelState);
        }

        // GET ALL
        // api/Customer
        [HttpGet]
        public async Task<IHttpActionResult> GetAllCustomers()
        {
            List<Customer> customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }

        // GET BY ID
        // api/Restaurant/{id}
        [HttpGet]
        public async Task<IHttpActionResult> GetCustomerById([FromUri] int id)
        {
            Customer customer = await _context.Customers.FindAsync(id);

            if(customer != null)
            {
                return Ok(customer);
            }

            return NotFound();
        }

        // PUT (update)
        // api/Customer/{id}
        [HttpPut]
        public async Task<IHttpActionResult> UpdateCustomer([FromUri] int id, [FromBody] Customer updatedCustomer)
        {
            // Check if ids match
            if (id != updatedCustomer?.Id)
            {
                return BadRequest("Ids do not match");
            }

            // Check the ModelState
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Find customer in database
            Customer customer = await _context.Customers.FindAsync(id);

            // If customer doesn't exist, do something
            if (customer is null)
                return NotFound();

            // Update the properties
            customer.FirstName = updatedCustomer.FirstName;
            customer.LastName = updatedCustomer.LastName;

            // Save the changes
            await _context.SaveChangesAsync();
            return Ok("The customer was updated!");
        }

        // DELETE (delete)
        // api/Customer/{id}
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteCustomer([FromUri] int id)
        {
            Customer customer = await _context.Customers.FindAsync(id);

            if (customer is null)
                return NotFound();

            _context.Customers.Remove(customer);

            if (await _context.SaveChangesAsync() == 1)
            {
                return Ok("The customer was deleted");
            }

            return InternalServerError();
        }
    }
}
