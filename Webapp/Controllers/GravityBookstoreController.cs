using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Webapp.Models.GravityBookstore;

namespace Webapp.Controllers
{
    [Authorize(Roles = "admin,user")]
    public class GravityBookstoreController : Controller
    {
        private readonly GravityBookstoreDBContext _context;

        public GravityBookstoreController(GravityBookstoreDBContext context)
        {
            _context = context;
        }

        // GET: GravityBookstore
        public async Task<IActionResult> Index(int page = 1, int size = 20)
        {
            // First query - basic book data and author count
            var booksQuery = await _context.Books
                .Include(b => b.Authors)
                .Select(b => new BookViewModel
                { 
                    BookId = b.BookId,
                    Title = b.Title,
                    Isbn13 = b.Isbn13,
                    NumPages = b.NumPages ?? 0,
                    PublicationDate = b.PublicationDate,
                    AuthorCount = b.Authors.Count,
                    SoldCopies = 0 // Default value
                })
                .OrderBy(b => b.Title)
                .Skip((page - 1) * size) 
                .Take(size)
                .ToListAsync(); 

            //  Second query  -  count order  lines for each  book 
            foreach  (var  book in  booksQuery) 
            { 
                book.SoldCopies  = await  _context.OrderLines 
                    .CountAsync(ol  =>  ol.BookId  ==  book.BookId);
            } 

            var totalCount  =  await _context.Books.CountAsync(); 
            var  paginatedList  =    new  PaginatedList<BookViewModel>(booksQuery,  totalCount, page,   size);  
            return  View(paginatedList); 
        } 

        //   GET:  GravityBookstore/Details/5 
        public  async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Language)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: GravityBookstore/Create
        public IActionResult Create()
        {
            ViewData["LanguageId"] = new SelectList(_context.BookLanguages, "LanguageId", "LanguageId");
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherId");
            return View();
        }

        // POST: GravityBookstore/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,Title,Isbn13,LanguageId,NumPages,PublicationDate,PublisherId")] Book book)
        {
            if (ModelState.IsValid) 
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LanguageId"] = new SelectList(_context.BookLanguages, "LanguageId", "LanguageId", book.LanguageId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherId", book.PublisherId);
            return View(book);
        }

        // GET: GravityBookstore/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["LanguageId"] = new SelectList(_context.BookLanguages, "LanguageId", "LanguageId", book.LanguageId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherId", book.PublisherId);
            return View(book);
        }

        // POST: GravityBookstore/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,Title,Isbn13,LanguageId,NumPages,PublicationDate,PublisherId")] Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LanguageId"] = new SelectList(_context.BookLanguages, "LanguageId", "LanguageId", book.LanguageId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherId", book.PublisherId);
            return View(book);
        }

        // GET: GravityBookstore/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Language)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: GravityBookstore/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }

        public async Task<IActionResult> Authors(int id)
        {
            var book = await _context.Books
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            ViewBag.BookTitle = book.Title; // Add book title for reference
            return View(book.Authors.OrderBy(a => a.AuthorName).ToList());
        }
        public async Task<IActionResult> AuthorDetails(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.AuthorId == id);

            if (author == null)
            {
                return NotFound();
            }

            var viewModel = new AuthorDetailsViewModel
            {
                AuthorId = author.AuthorId,
                AuthorName = author.AuthorName,
                Books = author.Books.Select(b => new BookBasicInfo 
                { 
                    BookId = b.BookId, 
                    Title = b.Title 
                })
            };

            return View(viewModel);
        }
    }
}
