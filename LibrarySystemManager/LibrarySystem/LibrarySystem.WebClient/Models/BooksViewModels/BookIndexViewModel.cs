﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LibrarySystem.WebClient.Models.BooksViewModels
{
    public class BookIndexViewModel
    {
        public BookIndexViewModel() { }

        public BookIndexViewModel(IEnumerable<BookViewModel> books)
        {
            this.Books = books;
        }

        public BookIndexViewModel
            (IEnumerable<BookViewModel> books, string searchBy, string parameters, int page)
            : this(books)
        {
            this.SearchBy = searchBy;
            this.Parameters = parameters;

            this.CurrentPage = page;
            this.PreviusPage = this.CurrentPage == 1 ? 1 : this.CurrentPage - 1;
            this.NextPage = this.CurrentPage + 1;
        }

        public IEnumerable<BookViewModel> Books { get; set; }

        public string SearchBy { get; set; }

        [Display(Name = "Search")]
        [StringLength(50, ErrorMessage = "The {0} must be at max {1} characters long.")]
        public string Parameters { get; set; }

        public int PreviusPage { get; set; }

        public int CurrentPage { get; set; }

        public int NextPage { get; set; }
    }
}