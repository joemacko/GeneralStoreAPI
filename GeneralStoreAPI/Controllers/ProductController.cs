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
    public class ProductController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        // POST (Create)
        // api/Product
        [HttpPost]
        public async Task<IHttpActionResult> CreateProduct([FromBody] Product model)
        {
            if (model is null)
                return BadRequest("The request body cannot be empty");

            if (ModelState.IsValid)
            {
                _context.Products.Add(model);
                int changeCount = await _context.SaveChangesAsync();
                return Ok("The product was successfully created!");
            }

            return BadRequest(ModelState);
        }

        // GET ALL
        // api/Product
        [HttpGet]
        public async Task<IHttpActionResult> GetAllProducts()
        {
            List<Product> products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        // GET BY ID
        // api/Product?sku={sku}
        [HttpGet]
        public async Task<IHttpActionResult> GetProductById([FromUri] string sku)
        {
            Product product = await _context.Products.FindAsync(sku);

            if (product != null)
                return Ok(product);

            return NotFound();
        }

        // PUT (Update)
        // api/Product?sku={sku}
        [HttpPut]
        public async Task<IHttpActionResult> UpdateProduct([FromUri] string sku, [FromBody] Product updatedProduct)
        {
            // Check if ids match
            if (sku != updatedProduct?.SKU)
                return BadRequest("Ids do not match");

            // Check the ModelState
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Find customer in database
            Product product = await _context.Products.FindAsync(sku);

            // If customer doesn't exist, do something
            if (product is null)
                return NotFound();

            // Update the properties
            // product.SKU = updatedProduct.SKU; - can't update this as of now (ids won't match then)
            product.Name = updatedProduct.Name;
            product.Cost = updatedProduct.Cost;
            product.NumberInInventory = updatedProduct.NumberInInventory;

            // Save the changes
            await _context.SaveChangesAsync();
            return Ok("The customer was updated!");
        }

        // DELETE (Delete)
        // api/Product?sku={sku}
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteProduct([FromUri] string sku)
        {
            Product product = await _context.Products.FindAsync(sku);

            if (product is null)
                return NotFound();

            _context.Products.Remove(product);

            if (await _context.SaveChangesAsync() == 1)
                return Ok("The customer was deleted");

            return InternalServerError();
        }
    }
}
