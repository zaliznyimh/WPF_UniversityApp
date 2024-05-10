using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels;

public class StudentsViewModel : ViewModelBase
{
    private readonly UniversityContext _context;
    private readonly IDialogService _dialogService;

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

    private ObservableCollection<Student>? _students = null;
    public ObservableCollection<Student>? Students
    {
        get
        {
            if (_students is null)
            {
                _students = new ObservableCollection<Student>();
                return _students;
            }
            return _students;
        }
        set
        {
            _students = value;
            OnPropertyChanged(nameof(Students));
        }
    }

    private ICommand? _add = null;
    public ICommand? Add
    {
        get
        {
            if (_add is null)
            {
                _add = new RelayCommand<object>(AddNewStudent);
            }
            return _add;
        }
    }

    private void AddNewStudent(object? obj)
    {
        var instance = MainWindowViewModel.Instance();
        if (instance is not null)
        {
            instance.StudentsSubView = new AddStudentViewModel(_context, _dialogService);

        }
    }

    private ICommand? _edit = null;
    public ICommand? Edit
    {
        get
        {
            if (_edit is null)
            {
                _edit = new RelayCommand<object>(EditStudent);
            }
            return _edit;
        }
    }

    private void EditStudent(object? obj)
    {
        if (obj is not null)
        {
            long studentId = (long)obj;
            EditStudentViewModel editStudentViewModel = new EditStudentViewModel(_context, _dialogService)
            {
                StudentId = studentId
            };
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.StudentsSubView = editStudentViewModel;
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
        if (obj is not null)
        {
            long studentId = (long)obj;
            Student? student = _context.Students.Find(studentId);
            if (student is not null)
            {
                DialogResult = _dialogService.Show(student.Name + " " + student.LastName);
                if (DialogResult == false)
                {
                    return;
                }

                _context.Students.Remove(student);
                _context.SaveChanges();
            }
        }
    }

    public StudentsViewModel(UniversityContext context, IDialogService dialogService)
    {
        _context = context;
        _dialogService = dialogService;

        _context.Database.EnsureCreated();
        _context.Students.Load();
        Students = _context.Students.Local.ToObservableCollection();
    }
}
