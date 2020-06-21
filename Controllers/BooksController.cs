using BooksAPI.DTOs;
using BooksAPI.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace BooksAPI.Controllers
{
    [RoutePrefix("api/books")]
    public class BooksController : ApiController
    {
        private BooksAPIContext db = new BooksAPIContext();

        // Typed lambda expression for Select() method. 
        private static readonly Expression<Func<Book, BookDto>> AsBookDto =
            x => new BookDto
            {
                Title = x.Title,
                Author = x.Author.Name,
                Genre = x.Genre
            };

        [Route("")]
        // GET: api/Books
        public IQueryable<BookDto> GetBooks()
        {
            return db.Books.Include(b => b.Author).Select(AsBookDto);
        }

        [Route("{id:int}")]
        // GET: api/Books/5
        [ResponseType(typeof(BookDto))]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            var book = await (from b in db.Books.Include(b => b.Author)
                              where b.BookId == id
                              select new BookDetailDto
                              {
                                  Title = b.Title,
                                  Genre = b.Genre,
                                  PublishDate = b.PublishDate,
                                  Price = b.Price,
                                  Description = b.Description,
                                  Author = b.Author.Name
                              }).FirstOrDefaultAsync();
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpGet]
        [Route("{genre}")]
        public IQueryable<BookDto> listBooksByGenre(string genre)
        {
            return db.Books.Include(b => b.Author)
                .Where(b => b.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase))
                .Select(AsBookDto);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}