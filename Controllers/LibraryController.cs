using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibraryWebProject.Data;
using LibraryWebProject.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryWebProject.Controllers
{
    public class LibraryController : Controller
    {
        private readonly ILogger<LibraryController> _logger;
        private readonly LibraryContext _context;
        string _filePath;
        private IWebHostEnvironment _hostEnvironment;
        public LibraryController(ILogger<LibraryController> logger, LibraryContext context, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _context = context;
            _filePath = Path.Combine(_hostEnvironment.WebRootPath, "BookPhotoFile");
            if (!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(_filePath);
            }
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Authors.Include(x => x.AuthorBooks).ThenInclude(x => x.BookPhotos).ToListAsync());
        }

        [HttpGet]
        public IActionResult CreateAuthor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthor(AuthorModel Author)
        {
            if (ModelState.IsValid)
            {
                _context.Authors.Add(Author);
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "Author is successfully added";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Mesaj"] = "Author is not added (Fill all informations)";
                return RedirectToAction("CreateAuthor");
            }
            return View();
        }
        [HttpGet]
        public IActionResult CreateBook(int? AuthorId)
        {
            if (_context.Authors.FirstOrDefault().AuthorID == null)
            {
                TempData["Mesaj"] = "First Create Author";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook(BookModel Book)
        {
            if (ModelState.IsValid)
            {
                foreach (var bookPhoto in Book.Author.BookPhotoFiles)
                {
                    var fullFilePath = Path.Combine(_filePath, bookPhoto.FileName);
                    using (var stream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        await bookPhoto.CopyToAsync(stream);
                    }   
                    Book.BookPhotos.Add(new BookPhotoModel { PhotoName = bookPhoto.FileName, createdTime = DateTime.Now, createdBy = "Admin", updatedBy = "Admin", updatedTime = DateTime.Now, isDeleted = false });
                }
                var AuthorModel = _context.Authors.Where(x => x.AuthorID == Book.AuthorID).FirstOrDefault();
                AuthorModel.AuthorBooks.Add(Book);
                _context.Update(AuthorModel);

                _context.Books.Where(x => x.AuthorID == Book.AuthorID).Append(Book);

                await _context.SaveChangesAsync();

                TempData["Mesaj"] = "Book is successfully added";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Mesaj"] = "Book is not added (Fill all informations)";
                return RedirectToAction("CreateBook");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteBook(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = _context.Books.Include(x => x.BookPhotos).Where(x => x.AuthorID == id).ToList();
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteBookConfirmed(int id)
        {
            var books = _context.Books.Where(x => x.BookID == id).FirstOrDefault();
            books.isDeleted = true;
            _context.Books.Update(books);
            await _context.SaveChangesAsync();

            return RedirectToAction("DeleteBook", new { id = books.BookID });
        }

        [HttpGet]
        public async Task<IActionResult> AuthorDetail(int? id)      
        {
            if (id == null)
            {
                return NotFound();
            }
            var author = _context.Authors.Include(x => x.AuthorBooks).Where(x => x.AuthorID == id).FirstOrDefault();

            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        [HttpGet]

        public async Task<IActionResult> AuthorEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = _context.Authors.Where(x => x.AuthorID == id).FirstOrDefault();

            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        [HttpPost]
        public async Task<IActionResult> AuthorEdit(int id, AuthorModel author)
        {
            if (id != author.AuthorID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Authors.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (System.Exception)
                {

                    throw;
                }
                return RedirectToAction(nameof(AuthorDetail), new { id = author.AuthorID });
            }

            return View(author);
        }

        public async Task<IActionResult> BookDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var books = _context.Books.Include(x => x.BookPhotos).Where(x => x.AuthorID == id).ToList();

            if (books == null)
            {
                return NotFound();
            }
            return View(books);
        }

        [HttpGet]
        public IActionResult EditBook(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = _context.Books.Include(x => x.BookPhotos).Where(x => x.BookID == id).FirstOrDefault();
            var books = _context.Authors.Include(x => x.AuthorBooks).ThenInclude(x => x.BookPhotos).Where(x => x.AuthorBooks.Where(x => x.BookID == id).FirstOrDefault() == book).FirstOrDefault();

            if (books == null)
            {
                return NotFound();
            }

            return View(books);
        }
        [HttpPost]
        public async Task<IActionResult> EditBook(int id, BookModel book)
        {
            if (id != book.BookID)
            {
                return NotFound();
            }
            
            var authors = _context.Authors.Where(x => x.AuthorBooks.Where(x => x.BookID == id).FirstOrDefault().BookID == book.BookID).FirstOrDefault();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(authors);
                    await _context.SaveChangesAsync();
                }
                catch (System.Exception)
                {

                    throw;
                }
                return RedirectToAction("BookDetail", new { id = book.BookID });
            }
            return View(book);
        }

        public async Task<IActionResult> PhotoAdd(BookModel book)
        {
            var AuthorModel = _context.Authors.Where(x => x.AuthorID == book.AuthorID).FirstOrDefault();

            try
            {
                foreach (var bookPhoto in book.Author.BookPhotoFiles)
                {
                    var fullFilePath = Path.Combine(_filePath, bookPhoto.FileName);
                    using (var stream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        await bookPhoto.CopyToAsync(stream);
                    }
                    book.BookPhotos.Add(new BookPhotoModel { PhotoName = bookPhoto.FileName, createdTime = DateTime.Now, createdBy = "Admin", updatedBy = "Admin", updatedTime = DateTime.Now, isDeleted = false });
                }

                AuthorModel.AuthorBooks.Add(book);
                _context.Update(AuthorModel);

                _context.Books.Where(x => x.AuthorID == book.AuthorID).Append(book);

                await _context.SaveChangesAsync();
            }
            catch (System.Exception)
            {

                throw;
            }
            return RedirectToAction("BookDetail", new { id = AuthorModel.AuthorID });
        }

        public async Task<IActionResult> PhotoDelete(int PhotoId)
        {
            var bookPhoto = _context.BookPhotos.Where(x => x.PhotoID == PhotoId).FirstOrDefault();
            bookPhoto.isDeleted = false;
            _context.Update(bookPhoto);
            await _context.SaveChangesAsync();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AuthorOption(int? id)
        {
            {
                if (_context.Authors.FirstOrDefault().AuthorID == null)
                {
                    TempData["Mesaj"] = "First Create Author";
                    return RedirectToAction("Index");
                }
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> AuthorDelete(int? id)
        {
            if(id == null){
                return NotFound();
            }
            var author = _context.Authors.Where(x => x.AuthorID == id).FirstOrDefault();

            if(author == null){
                return NotFound();
            }
            return View(author);
        }
        
        [HttpPost]
        public async Task<IActionResult> AuthorDelete(int id)
        {
            var author = _context.Authors.Where(x => x.AuthorID == id).FirstOrDefault();
            author.isDeleted = true;
            _context.Update(author);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        
            public IActionResult Privacy()
            {
                return View();
            }

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            public IActionResult Error()
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }