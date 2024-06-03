using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.Services;

public class DatabaseService : IDatabaseService
{
    #region Properties and Ctor
    private readonly UniversityContext _context;
    private readonly IDialogService _dialogService;


    public DatabaseService(UniversityContext context, IDialogService dialogService)
    {
        _context = context;
        _dialogService = dialogService;
    }

    private bool? _dialogResult = null;
    public bool? DialogResult
    {
        get
        {
            return _dialogResult;
        }
        set
        {
            _dialogResult = value;
        }
    }

    #endregion

    #region BooksMethods
    public void RemoveBook(object obj)
    {
        long bookId = (long)obj;
        Book book = _context.Books.Find(bookId);
        if (book is not null)
        {
            DialogResult = _dialogService.Show(book.Title);
            if (DialogResult == false)
            {
                return;
            }

            _context.Books.Remove(book);
            _context.SaveChanges();
        }
    }

    public void SaveBook(Book book)
    {
        _context.Books.Add(book);
        _context.SaveChanges();
    }

    public Book FindBook(long BookID)
    {
        var findedBook = new Book();
        findedBook = _context.Books.Find(BookID);
        return findedBook;
    }

    public void UpdateBook(Book book)
    {
        _context.Entry(book).State = EntityState.Modified;
        _context.SaveChanges();
    }



    #endregion BooksMethods

}
