using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthorAPI.Models;
using AuthorAPI.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorAPI.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase {

        private BookContext context;

        public BooksController(BookContext bookContext) {
            context = bookContext;
        }
        [HttpGet]
        public async Task<ActionResult<IList<Book>>> GetAllBooks() {
            try {
                IList<Book> books = await context.Books.ToListAsync();
                return Ok(books);
            }
            catch (NullReferenceException e) {
                return NotFound(e.Message);
            }
            catch (Exception e) {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        [Route("{id:int}")]
        public async Task<ActionResult<Book>> AddBook([FromBody] Book a, [FromRoute] int id) {
            try {
                if (!ModelState.IsValid)
                    return BadRequest("Please specify a book of proper format");
                Book book = await context.Books.FirstOrDefaultAsync(i => i.ISBN == a.ISBN);
                if (book != null)
                    throw new Exception("The book with the inputed isbn already exists");
                Author existing = await context.Set<Author>()
                    .FirstOrDefaultAsync(e => e.Id == id);
                if (existing == null)
                    throw new Exception("The author does not exist");
                
                //add book to the database
                IList<Book> books = new List<Book>();
                books.Add(a);
                existing.Books = books;
                await context.Set<Book>().AddAsync(a);
                //add the book to the author
                await context.SaveChangesAsync();
                return Created($"/{a.ISBN}", a);
            }
            catch (NullReferenceException e) {
                return NotFound(e.Message);
            }
            catch (Exception e) {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete]
        [Route("{isbn:int}")]
        public async Task<ActionResult> DeleteBook([FromRoute] int isbn) {
            try {
                Book book = await context.Set<Book>().FirstOrDefaultAsync(b => b.ISBN == isbn);
                if (book == null)
                    throw new Exception("The book does not exist");
                context.Books.Remove(book);
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (NullReferenceException e) {
                return NotFound(e.Message);
            }
            catch (Exception e) {
                return StatusCode(500, e.Message);
            }
        }
    }
}