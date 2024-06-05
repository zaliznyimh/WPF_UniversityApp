using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels;

public class FacultyMemberViewModel : ViewModelBase
{
    private readonly UniversityContext _context;
    private readonly IDialogService _dialogService;
    private readonly IDatabaseService _databaseService;

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

    private ObservableCollection<FacultyMember>? _facultyMembers = null;
    public ObservableCollection<FacultyMember> FacultyMembers
    {
        get
        {
            if (_facultyMembers is null)
            {
                _facultyMembers = new ObservableCollection<FacultyMember>();
                return _facultyMembers;
            }
            return _facultyMembers;
        }
        set
        {
            _facultyMembers = value;
            OnPropertyChanged(nameof(FacultyMembers));
        }
    }

    private ICommand? _add = null;
    public ICommand? Add
    {
        get
        {
            if (_add is null)
            {
                _add = new RelayCommand<object>(AddNewFacultyMember);
            }
            return _add;
        }
    }
    private void AddNewFacultyMember(object? obj)
    {
        var instance = MainWindowViewModel.Instance();
        if (instance is not null)
        {
            instance.FacultyMemberSubView = new AddFacultyMemberViewModel(_context, _dialogService, _databaseService);
        }
    }

    private ICommand? _edit;
    public ICommand? Edit
    {
        get
        {
            if (_edit is null)
            {
                _edit = new RelayCommand<object>(EditFacultyMember);
            }
            return _edit;
        }
    }

    private void EditFacultyMember(object? obj)
    {
        if (obj is not null)
        {
            long facultyMemberId = (long)obj;
            EditFacultyMemberViewModel editFacultyMemberViewModel = new EditFacultyMemberViewModel(_context, _dialogService, _databaseService)
            {
                FacultyMemberId = facultyMemberId
            };
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.FacultyMemberSubView = editFacultyMemberViewModel;
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
                _remove = new RelayCommand<object>(RemoveFacultyMember);
            }
            return _remove;
        }
    }

    private void RemoveFacultyMember(object? obj)
    {
        if (obj is not null)
        {
            _databaseService.RemoveFacultyMember(obj);
        }
    }

    public FacultyMemberViewModel(UniversityContext context, IDialogService dialogService, IDatabaseService databaseService)
    {
        _context = context;
        _dialogService = dialogService;
        _databaseService = databaseService;

        _context.Database.EnsureCreated();
        _context.FacultyMembers.Load();
        FacultyMembers = _context.FacultyMembers.Local.ToObservableCollection();
    }
}
