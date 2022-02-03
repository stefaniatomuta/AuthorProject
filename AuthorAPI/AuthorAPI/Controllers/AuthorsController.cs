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
    public class AuthorsController :ControllerBase {

        private BookContext context;

        public AuthorsController(BookContext bookContext) {
            this.context = bookContext;
        }
        [HttpGet]
        public async Task<ActionResult<IList<Author>>> GetAllAuthors() {
            try {
                IList<Author> authors =  await context.Authors.Include(a => a.Books).ToListAsync();
                return Ok(authors);
            }
            catch (NullReferenceException e) {
                return NotFound(e.Message);
            }
            catch (Exception e) {
                return StatusCode(500, e.Message);
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<Author>> AddAuthor([FromBody] Author a) {
            try {
                if (a.FirstName == null || a.LastName == null)
                    return BadRequest("Please enter an author of the proper format");
                Author existing = await context.Set<Author>()
                    .FirstOrDefaultAsync(e => e.FirstName.Equals(a.FirstName)
                                              && e.LastName.Equals(e.LastName));
                if (existing != null)
                    throw new Exception("The author already exists");
                await context.Set<Author>().AddAsync(a);
                await context.SaveChangesAsync();
                return Created($"/{a.Id}", a);
            }
            catch (Exception e) {
                return StatusCode(500, e.Message);
            }
        }
    }
}