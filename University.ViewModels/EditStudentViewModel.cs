using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using University.Data;
using University.Extensions;
using University.Interfaces;
using University.Models;

namespace University.ViewModels;

public class EditStudentViewModel : ViewModelBase, IDataErrorInfo
{
    private readonly UniversityContext _context;
    private readonly IDialogService _dialogService;
    private Student? _student = new Student();

    public string Error
    {
        get { return string.Empty; }
    }

    public string this[string columnName]
    {
        get
        {
            if (columnName == "Name")
            {
                if (string.IsNullOrEmpty(Name))
                {
                    return "Name is Required";
                }
            }
            if (columnName == "LastName")
            {
                if (string.IsNullOrEmpty(LastName))
                {
                    return "Last Name is Required";
                }
            }
            if (columnName == "PESEL")
            {
                if (string.IsNullOrEmpty(PESEL))
                {
                    return "PESEL is Required";
                }
                if (!PESEL.IsValidPESEL())
                {
                    return "PESEL is Invalid";
                }
            }
            if (columnName == "BirthDate")
            {
                if (BirthDate is null)
                {
                    return "BirthDate is Required";
                }
            }
            return string.Empty;
        }
    }

    private string _name = string.Empty;
    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    private string _lastName = string.Empty;
    public string LastName
    {
        get
        {
            return _lastName;
        }
        set
        {
            _lastName = value;
            OnPropertyChanged(nameof(LastName));
        }
    }

    private string _pesel = string.Empty;
    public string PESEL
    {
        get
        {
            return _pesel;
        }
        set
        {
            _pesel = value;
            OnPropertyChanged(nameof(PESEL));
        }
    }

    private DateTime? birthDate = null;
    public DateTime? BirthDate
    {
        get
        {
            return birthDate;
        }
        set
        {
            birthDate = value;
            OnPropertyChanged(nameof(BirthDate));
        }
    }

    private string _response = string.Empty;
    public string Response
    {
        get
        {
            return _response;
        }
        set
        {
            _response = value;
            OnPropertyChanged(nameof(Response));
        }
    }

    private long _studentId = 0;
    public long StudentId
    {
        get
        {
            return _studentId;
        }
        set
        {
            _studentId = value;
            OnPropertyChanged(nameof(StudentId));
            LoadStudentData();
        }
    }

    private ObservableCollection<Subject>? _assignedSubjects = null;
    public ObservableCollection<Subject> AssignedSubjects
    {
        get
        {
            if (_assignedSubjects is null)
            {
                _assignedSubjects = LoadSubjects();
                return _assignedSubjects;
            }
            return _assignedSubjects;
        }
        set
        {
            _assignedSubjects = value;
            OnPropertyChanged(nameof(AssignedSubjects));
        }
    }

    private ICommand? _back = null;
    public ICommand Back
    {
        get
        {
            if (_back is null)
            {
                _back = new RelayCommand<object>(NavigateBack);
            }
            return _back;
        }
    }

    private void NavigateBack(object? obj)
    {
        var instance = MainWindowViewModel.Instance();
        if (instance is not null)
        {
            instance.StudentsSubView = new StudentsViewModel(_context, _dialogService);
        }
    }

    private ICommand? _save = null;
    public ICommand Save
    {
        get
        {
            if (_save is null)
            {
                _save = new RelayCommand<object>(SaveData);
            }
            return _save;
        }
    }

    private void SaveData(object? obj)
    {
        if (!IsValid())
        {
            Response = "Please complete all required fields";
            return;
        }

        if (_student is null)
        {
            return;
        }
        _student.Name = Name;
        _student.LastName = LastName;
        _student.PESEL = PESEL;
        _student.BirthDate = BirthDate;
        _student.Subjects = AssignedSubjects.Where(s => s.IsSelected).ToList();

        _context.Entry(_student).State = EntityState.Modified;
        _context.SaveChanges();

        Response = "Data Updated";
    }

    public EditStudentViewModel(UniversityContext context, IDialogService dialogService)
    {
        _context = context;
        _dialogService = dialogService;
    }

    private ObservableCollection<Subject> LoadSubjects()
    {
        _context.Database.EnsureCreated();
        _context.Subjects.Load();
        return _context.Subjects.Local.ToObservableCollection();
    }

    private bool IsValid()
    {
        string[] properties = { "Name", "LastName", "PESEL", "BirthDay" };
        foreach (string property in properties)
        {
            if (!string.IsNullOrEmpty(this[property]))
            {
                return false;
            }
        }
        return true;
    }

    private void LoadStudentData()
    {
        if (_context?.Students is null)
        {
            return;
        }
        _student = _context.Students.Find(StudentId);
        if (_student is null)
        {
            return;
        }
        this.Name = _student.Name;
        this.LastName = _student.LastName;
        this.PESEL = _student.PESEL;
        this.BirthDate = _student.BirthDate;
        if (_student.Subjects is null)
        {
            return;
        }
        foreach (Subject subject in _student.Subjects)
        {
            if (subject is not null && AssignedSubjects is not null)
            {
                var assignedSubject = AssignedSubjects
                    .FirstOrDefault(s => s.SubjectId == subject.SubjectId);
                if (assignedSubject is not null)
                { 
                    assignedSubject.IsSelected = true;
                }
            }
        }
    }
}
