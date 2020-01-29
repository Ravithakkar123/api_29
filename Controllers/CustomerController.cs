using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webapi.Models;

namespace Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly RestaurantDataContext _context;

        public CustomerController(RestaurantDataContext context)
        {
            _context = context;
        }
        // GET: api/Customer
        [HttpGet]
        public IActionResult GetCustomer()
        {
            try
            {
                var customers = from o in _context.Customers
                             select o;

                var customer = _context.Customers.Include(o => o.Addresses).ToList();
                return Ok(customer);
          
            
            }
            catch
            {
                return BadRequest();
            }
        }

        // GET: api/Customer/5
        [HttpGet("{id}")]

        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }



        // PUT: api/Customer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }


            return Ok();
        }

        // POST: api/Customer
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
        }

      
        // DELETE: api/Customer/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Customer>> DeleteCustomer(int id)
        {
            var customer = _context.Customers.Where(p => p.CustomerId == id).Include(p => p.Addresses).SingleOrDefault();

            if (customer == null)
            {
                return NotFound();
            }

            foreach (var child in customer.Addresses.ToList())
            {
                if (customer.Addresses.Any(c => c.AddressId == child.AddressId))
                    _context.addresses.Remove(child);
            }


            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return customer;
        }


        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
