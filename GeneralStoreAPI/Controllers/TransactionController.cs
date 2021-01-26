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
    public class TransactionController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        // POST (Create)
        // api/Transaction
        [HttpPost]
        public async Task<IHttpActionResult> CreateTransaction([FromBody] Transaction transaction)
        {

            // Check if transaction is null
            if (transaction is null)
                return BadRequest("Your request body cannot be empty");

            Customer customer = await _context.Customers.FindAsync(transaction.CustomerId);
            Product product = await _context.Products.FindAsync(transaction.ProductSKU);

            // Check if customer is null
            if (customer is null)
                return BadRequest("Your customer is null");

            // Check if product is null
            if (product is null)
                return BadRequest("Your product is null");

            // Verify that the product is in stock
            if (product.IsInStock is false)
                return BadRequest("The product is not in stock");

            // Check that there is enough product to complete the Transaction
            if (product.NumberInInventory < transaction.ItemCount)
            {
                return BadRequest("There is not enough product to complete the transaction");
            }

            // Check if the model is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Add the transaction and save changes
            _context.Transactions.Add(transaction);

            // Remove the Product(s) that were bought
            product.NumberInInventory = product.NumberInInventory - transaction.ItemCount;

            // Save changes and final return
            await _context.SaveChangesAsync();
            return Ok("The transaction was completed successfully!");
        }

        // GET ALL
        // api/Transaction
        [HttpGet]
        public async Task<IHttpActionResult> GetAllTransactions()
        {
            List<Transaction> transactions = await _context.Transactions.ToListAsync();
            return Ok(transactions);
        }

        // GET BY TRANSACTION ID
        // api/Transaction/{id}
        [HttpGet]
        public async Task<IHttpActionResult> GetTransactionById([FromUri] int id)
        {
            Transaction transaction = await _context.Transactions.FindAsync(id);

            if (transaction != null)
                return Ok(transaction);

            return NotFound();
        }

        //// GET ALL TRANSACTIONS BY CUSTOMER ID
        //// api/Transaction/{id}
        //[HttpGet]
        //public async Task<IHttpActionResult> GetAllTransactionsByCustomerId([FromUri] int customerId)
        //{
        //    List<Transaction> transactions = await _context.Transactions.ToListAsync();
        //    return null;
        //    // return transactions.Where(transactions.Select.transactions.Contains(customerId))
        //    // List<Transaction> customerTransactions = transactions.
        //}

        // PUT (Update)
        [HttpPut]
        public async Task<IHttpActionResult> UpdateTransaction([FromUri] int id, [FromBody] Transaction updatedTransaction)
        {
            var product = await _context.Products.FindAsync(updatedTransaction.ProductSKU);
            var customer = await _context.Customers.FindAsync(updatedTransaction.CustomerId);

            if (id != updatedTransaction?.Id)
                return BadRequest("Ids don't match!");

            if (product is null)
                return BadRequest("The product doesn't exist");

            if (customer is null)
                return BadRequest("The customer doesn't exist");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Transaction originalTransaction = await _context.Transactions.FindAsync(id);
            Product originalProduct = await _context.Products.FindAsync(originalTransaction.ProductSKU);

            if (originalTransaction is null)
                return NotFound();

            // Verify any product changes
            if (originalTransaction.ProductSKU != updatedTransaction.ProductSKU)
            {
                // Update Product Inventory to reflect updated Transaction
                originalProduct.NumberInInventory = originalProduct.NumberInInventory + originalTransaction.ItemCount;
                product.NumberInInventory = product.NumberInInventory - updatedTransaction.ItemCount;
                originalTransaction.ProductSKU = updatedTransaction.ProductSKU;
            }

            if (originalTransaction.ProductSKU == updatedTransaction.ProductSKU)
            {
                if (originalTransaction.ItemCount < updatedTransaction.ItemCount)
                {
                    int transactionDifference;
                    transactionDifference = updatedTransaction.ItemCount - originalTransaction.ItemCount;
                    originalProduct.NumberInInventory = originalProduct.NumberInInventory - transactionDifference;
                }
                else
                {
                    int transactionDifference;
                    transactionDifference = originalTransaction.ItemCount - updatedTransaction.ItemCount;
                    originalProduct.NumberInInventory = originalProduct.NumberInInventory + transactionDifference;
                }
            }

            originalTransaction.DateOfTransaction = updatedTransaction.DateOfTransaction;
            await _context.SaveChangesAsync();
            return Ok("Your transaction has been successfully updated!");
        }

        // DELETE BY TRANSACTION ID
        // api/Transaction/{id}
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteTransaction([FromUri] int id)
        {
            Transaction transaction = await _context.Transactions.FindAsync(id);
            Product product = await _context.Products.FindAsync(transaction.ProductSKU);

            if (transaction is null)
                return NotFound();

            _context.Transactions.Remove(transaction);

            if (await _context.SaveChangesAsync() == 1)
            {
                // Update Product Inventory to reflect updated Transaction
                product.NumberInInventory = product.NumberInInventory + transaction.ItemCount;
                return Ok("The transaction was deleted");
            }

            return InternalServerError();
        }
    }
}
