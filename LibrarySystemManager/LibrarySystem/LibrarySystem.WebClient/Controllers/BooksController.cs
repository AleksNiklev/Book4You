﻿using System;
using System.Linq;
using LibrarySystem.Data.Models;
using LibrarySystem.Services;
using LibrarySystem.WebClient.Models.BooksViewModels;
using LibrarySystem.WebClient.WebClientGlobalConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LibrarySystem.WebClient.Controllers
{
    public class BooksController : Controller
    {


        private readonly IBooksServices _booksServices;
        private readonly UserManager<User> _userManager;
        private readonly IUsersServices _usersServices;
        private readonly IMemoryCache _memoryCache;

        public BooksController(
            IBooksServices booksServices,
            UserManager<User> userManager,
            IUsersServices usersServices,
            IMemoryCache memoryCache)
        {
            _booksServices = booksServices;
            _userManager = userManager;
            _usersServices = usersServices;
            _memoryCache = memoryCache;
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult Index()
        {
            var user = GetUser();

            var booksOfTheDay = this._memoryCache.GetOrCreate("BooksOfTheDay", e =>
            {
                e.AbsoluteExpiration = DateTime.UtcNow.AddDays(1);

                // TODO Take 5 random books.
                return this._booksServices.ListBooks().Take(5);
            }
            );

            var bookModel = booksOfTheDay.Select(b => new BookViewModel(b, user));

            var model = new BookIndexViewModel(bookModel);

            return View(model);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBook(Guid bookId)
        {
            var userId = this._userManager.GetUserId(HttpContext.User);

            this._usersServices.BorrowBook(userId, bookId);

            return RedirectToAction("Index", "Books");
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult Details(Guid bookId)
        {
            var book = _booksServices.GetBookById(bookId);

            var model = new BookViewModel(book);

            return View("Details", model);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public IActionResult ListBooks(string searchBy, string parameters, int page)
        {
            var books = this._booksServices
                .ListBooks(searchBy, parameters, WebConstants.numOfElementInPage, page);

            if (books.Count() == 0)
            {
                return RedirectToAction("ListBooks", new
                {
                    searchBy,
                    parameters,
                    page = page - 1
                });
            }

            var user = GetUser();

            var bookModel = books.Select(b => new BookViewModel(b, user));

            var model = new BookIndexViewModel(bookModel, searchBy, parameters, page);

            return View(model);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ListBooks(BookIndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("ListBooks", new
            {
                searchBy = model.SearchBy,
                parameters = model.Parameters,
                page = model.CurrentPage
            });
        }

        private User GetUser()
        {
            var userId = this._userManager.GetUserId(HttpContext.User);

            var user = this._usersServices.GetUserById(userId);

            return user;
        }
    }
}