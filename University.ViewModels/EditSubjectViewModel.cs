using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels;

public class EditSubjectViewModel : ViewModelBase, IDataErrorInfo
{
    private readonly UniversityContext _context;
    private readonly IDialogService _dialogService;
    private Subject? _subject = new Subject();

    public string Error => string.Empty;

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
            if (columnName == "Semester")
            {
                if (string.IsNullOrEmpty(Semester))
                {
                    return "Semester is Required";
                }
            }
            if (columnName == "Lecturer")
            {
                if (string.IsNullOrEmpty(Lecturer))
                {
                    return "Lecturer is Required";
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

    private string _semester = string.Empty;
    public string Semester
    {
        get
        {
            return _semester;
        }
        set
        {
            _semester = value;
            OnPropertyChanged(nameof(Semester));
        }
    }

    private string _lecturer = string.Empty;
    public string Lecturer
    {
        get
        {
            return _lecturer;
        }
        set
        {
            _lecturer = value;
            OnPropertyChanged(nameof(Lecturer));
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

    private long _subjectId = 0;
    public long SubjectId
    {
        get
        {
            return _subjectId;
        }
        set
        {
            _subjectId = value;
            OnPropertyChanged(nameof(SubjectId));
            LoadSubjectData();
        }
    }

    private ObservableCollection<Student>? _availableStudents = null;
    public ObservableCollection<Student> AvailableStudents
    {
        get
        {
            if (_availableStudents is null)
            {
                _availableStudents = LoadStudents();
                return _availableStudents;
            }
            return _availableStudents;
        }
        set
        {
            _availableStudents = value;
            OnPropertyChanged(nameof(AvailableStudents));
        }
    }

    private ObservableCollection<Student>? _assignedStudents = null;
    public ObservableCollection<Student>? AssignedStudents
    {
        get
        {
            if (_assignedStudents is null)
            {
                _assignedStudents = new ObservableCollection<Student>();
                return _assignedStudents;
            }
            return _assignedStudents;
        }
        set
        {
            _assignedStudents = value;
            OnPropertyChanged(nameof(AssignedStudents));
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
            instance.SubjectsSubView = new SubjectsViewModel(_context, _dialogService);
        }
    }

    private ICommand? _add;
    public ICommand Add
    {
        get
        {
            if (_add is null)
            {
                _add = new RelayCommand<object>(AddStudent);
            }
            return _add;
        }
    }

    private void AddStudent(object? obj)
    {
        if (obj is Student student)
        {
            if (AssignedStudents is not null && !AssignedStudents.Contains(student))
            {
                AssignedStudents.Add(student);
            }
        }
    }

    private ICommand? _remove = null;
    public ICommand? Remove
    {
        get
        {
            if (_remove is null)
            {
                _remove = new RelayCommand<object>(RemoveStudent);
            }
            return _remove;
        }
    }

    private void RemoveStudent(object? obj)
    {
        if (obj is Student student)
        {
            if (AssignedStudents is not null)
            {
                AssignedStudents.Remove(student);
            }
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

        if (_subject is null)
        {
            return;
        }

        _subject.Name = Name;
        _subject.Semester = Semester;
        _subject.Lecturer = Lecturer;
        _subject.Students = AssignedStudents;

        _context.Entry(_subject).State = EntityState.Modified;
        _context.SaveChanges();

        Response = "Data Saved";
    }

    public EditSubjectViewModel(UniversityContext context, IDialogService dialogService)
    {
        _context = context;
        _dialogService = dialogService;
    }

    private ObservableCollection<Student> LoadStudents()
    {
        _context.Database.EnsureCreated();
        _context.Students.Load();
        return _context.Students.Local.ToObservableCollection();
    }

    private bool IsValid()
    {
        string[] properties = { "Name", "Semester", "Lecturer" };
        foreach (string property in properties)
        {
            if (!string.IsNullOrEmpty(this[property]))
            {
                return false;
            }
        }
        return true;
    }

    private void LoadSubjectData()
    {
        var subjects = _context.Subjects;
        if (subjects is not null)
        {
            _subject = subjects.Find(SubjectId);
            if (_subject is null)
            {
                return;
            }
            this.Name = _subject.Name;
            this.Semester = _subject.Semester;
            this.Lecturer = _subject.Lecturer;
            if (_subject.Students is not null)
            {
                this.AssignedStudents =
                    new ObservableCollection<Student>(_subject.Students);
            }
        }
    }
}
