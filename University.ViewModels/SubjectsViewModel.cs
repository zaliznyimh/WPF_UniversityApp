using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels;

public class SubjectsViewModel : ViewModelBase
{
    private readonly UniversityContext _context;
    private readonly IDialogService _dialogService;

    private bool? dialogResult;
    public bool? DialogResult
    {
        get
        {
            return dialogResult;
        }
        set
        {
            dialogResult = value;
        }
    }

    private ObservableCollection<Subject>? subjects = null;
    public ObservableCollection<Subject>? Subjects
    {
        get
        {
            if (subjects is null)
            {
                subjects = new ObservableCollection<Subject>();
                return subjects;
            }
            return subjects;
        }
        set
        {
            subjects = value;
            OnPropertyChanged(nameof(Subjects));
        }
    }

    private ICommand? _add = null;
    public ICommand? Add
    {
        get
        {
            if (_add is null)
            {
                _add = new RelayCommand<object>(AddNewSubject);
            }
            return _add;
        }
    }

    private void AddNewSubject(object? obj)
    {
        var instance = MainWindowViewModel.Instance();
        if (instance is not null)
        {
            instance.SubjectsSubView = new AddSubjectViewModel(_context, _dialogService);
        }
    }

    private ICommand? _edit;
    public ICommand? Edit
    {
        get
        {
            if (_edit is null)
            {
                _edit = new RelayCommand<object>(EditSubject);
            }
            return _edit;
        }
    }

    private void EditSubject(object? obj)
    {
        if (obj is not null)
        {
            long subjectId = (long)obj;
            EditSubjectViewModel editSubjectViewModel = new EditSubjectViewModel(_context, _dialogService)
            {
                SubjectId = subjectId
            };
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.SubjectsSubView = editSubjectViewModel;
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
                _remove = new RelayCommand<object>(RemoveSubject);
            }
            return _remove;
        }
    }

    private void RemoveSubject(object? obj)
    {
        if (obj is not null)
        {
            long subjectId = (long)obj;
            Subject? subject = _context.Subjects.Find(subjectId);
            if (subject is not null)
            {
                DialogResult = _dialogService.Show(subject.Name);
                if (DialogResult == false)
                {
                    return;
                }

                _context.Subjects.Remove(subject);
                _context.SaveChanges();
            }
        }
    }

    public SubjectsViewModel(UniversityContext context, IDialogService dialogService)
    {
        _context = context;
        _dialogService = dialogService;

        _context.Database.EnsureCreated();
        _context.Subjects.Load();
        Subjects = _context.Subjects.Local.ToObservableCollection();
    }
}