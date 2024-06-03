using System;
using University.Interfaces;
using University.Data;

namespace University.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly UniversityContext _context;
    private readonly IDialogService _dialogService;
    private readonly IDatabaseService _databaseService;

    private int _selectedTab;
    public int SelectedTab
    {
        get
        {
            return _selectedTab;
        }
        set
        {
            _selectedTab = value;
            OnPropertyChanged(nameof(SelectedTab));
        }
    }

    private object? _studentsSubView = null;
    public object? StudentsSubView
    {
        get
        {
            return _studentsSubView;
        }
        set
        {
            _studentsSubView = value;
            OnPropertyChanged(nameof(StudentsSubView));
        }
    }

    private object? _subjectsSubView = null;
    public object? SubjectsSubView
    {
        get
        {
            return _subjectsSubView;
        }
        set
        {
            _subjectsSubView = value;
            OnPropertyChanged(nameof(SubjectsSubView));
        }
    }

    private object? _facultyMemberSubView = null;
    public object? FacultyMemberSubView
    {
        get
        {
            return _facultyMemberSubView;
        }
        set
        {
            _facultyMemberSubView = value;
            OnPropertyChanged(nameof(FacultyMemberSubView));
        }
    }

    private object? _researchProjectSubView = null;
    public object? ResearchProjectSubView
    {
        get
        {
            return _researchProjectSubView;
        }
        set
        {
            _researchProjectSubView = value;
            OnPropertyChanged(nameof(ResearchProjectSubView));
        }
    }


    private object? _booksSubView = null;
    public object? BooksSubView 
    {
        get
        {
            return _booksSubView;
        }
        set
        {
            _booksSubView = value;
            OnPropertyChanged(nameof(BooksSubView));
        }
    }

    private object? _searchSubView = null;
    public object? SearchSubView
    {
        get
        {
            return _searchSubView;
        }
        set
        {
            _searchSubView = value;
            OnPropertyChanged(nameof(SearchSubView));
        }
    }


    private static MainWindowViewModel? _instance = null;
    public static MainWindowViewModel? Instance()
    {
        return _instance;
    }

    public MainWindowViewModel(UniversityContext context, IDialogService dialogService, IDatabaseService databaseService)
    {
        _context = context;
        _dialogService = dialogService;
        _databaseService = databaseService;

        if (_instance is null)
        {
            _instance = this;
        }

        FacultyMemberSubView = new FacultyMemberViewModel(_context, _dialogService);
        StudentsSubView = new StudentsViewModel(_context, _dialogService);
        SubjectsSubView = new SubjectsViewModel(_context, _dialogService);
        SearchSubView = new SearchViewModel(_context, _dialogService, _databaseService);
        ResearchProjectSubView = new ResearchProjectViewModel(_context, _dialogService);
        BooksSubView = new BooksViewModel(_context, _dialogService, _databaseService);
    }
}
