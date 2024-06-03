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

    #region FacultyMember Methods
    public void RemoveFacultyMember(object obj)
    {
        long facultyMemberId = (long)obj;
        FacultyMember facultyMember = _context.FacultyMembers.Find(facultyMemberId);
        if (facultyMember is not null)
        {
            DialogResult = _dialogService.Show(facultyMember.Name);
            if (DialogResult == false)
            {
                return;
            }

            _context.FacultyMembers.Remove(facultyMember);
            _context.SaveChanges();
        }
    }

    public void SaveFacultyMember(FacultyMember facultyMember)
    {
        _context.FacultyMembers.Add(facultyMember);
        _context.SaveChanges();
    }

    public FacultyMember FindFacultyMember(long FacultyMemberId)
    {
        var findedFacultyMember = new FacultyMember();
        findedFacultyMember = _context.FacultyMembers.Find(FacultyMemberId);
        return findedFacultyMember;
    }

    public void UpdateFacultyMember(FacultyMember facultyMember)
    {

        _context.Entry(facultyMember).State = EntityState.Modified;
        _context.SaveChanges();
    }

    #endregion //FacultyMember Methods

    #region ResearchProject Methods
    public void RemoveResearchProject(object obj)
    {
        long projectId = (long)obj;
        ResearchProject researchProject = _context.ResearchProjects.Find(projectId);
        if (researchProject is not null)
        {
            DialogResult = _dialogService.Show(researchProject.Title);
            if (DialogResult == false)
            {
                return;
            }

            _context.ResearchProjects.Remove(researchProject);
            _context.SaveChanges();
        }
    }

    public void SaveResearchProject(ResearchProject researchProject)
    {
        _context.ResearchProjects.Add(researchProject);
        _context.SaveChanges();
    }

    public ResearchProject FindResearchProject(long researchProjectID)
    {
        var findedResearchProject = new ResearchProject();
        findedResearchProject = _context.ResearchProjects.Find(researchProjectID);
        return findedResearchProject;
    }

    public void UpdateResearchProject(ResearchProject researchProject)
    {
        _context.Entry(researchProject).State = EntityState.Modified;
        _context.SaveChanges();
    }

    #endregion // ResearchProject Methods

}
