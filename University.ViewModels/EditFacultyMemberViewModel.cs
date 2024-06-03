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

public class EditFacultyMemberViewModel : ViewModelBase
{
    private readonly UniversityContext _context;
    private readonly IDialogService _dialogService;
    private readonly IDatabaseService _databaseService;

    private FacultyMember? _facultyMember = new FacultyMember();

    public EditFacultyMemberViewModel(UniversityContext context, IDialogService dialogService, IDatabaseService databaseService)
    {
        _context = context;
        _dialogService = dialogService;
        _databaseService = databaseService;
    }

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
            if (columnName == "Gender")
            {
                if (string.IsNullOrEmpty(Gender))
                {
                    return "Gender is Required";
                }
            }
            if (columnName == "Position")
            {
                if (Position is null)
                {
                    return "Position is Required";
                }
            }
            if (columnName == "Email")
            {
                if (Email is null)
                {
                    return "Email is Required";
                }
            }
            if (columnName == "OfficeRoomNumber")
            {
                if (OfficeRoomNumber is null)
                {
                    return "Office room number is Required";
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

    private int _age = 0;
    public int Age
    {
        get
        {
            return _age;
        }
        set
        {
            _age = value;
            OnPropertyChanged(nameof(Age));
        }
    }

    private string _gender = string.Empty;
    public string Gender
    {
        get
        {
            return _gender;
        }
        set
        {
            _gender = value;
            OnPropertyChanged(nameof(Gender));
        }
    }

    private string _department = string.Empty;
    public string Department
    {
        get
        {
            return _department;
        }
        set
        {
            _department = value;
            OnPropertyChanged(nameof(Department));
        }
    }

    private string _position = string.Empty;
    public string Position
    {
        get
        {
            return _position;
        }
        set
        {
            _position = value;
            OnPropertyChanged(nameof(Position));
        }
    }

    private string _email = string.Empty;
    public string Email
    {
        get
        {
            return _email;
        }
        set
        {
            _email = value;
            OnPropertyChanged(nameof(Email));
        }
    }

    private string _officeRoomNumber = string.Empty;
    public string OfficeRoomNumber
    {
        get
        {
            return _officeRoomNumber;
        }
        set
        {
            _officeRoomNumber = value;
            OnPropertyChanged(nameof(OfficeRoomNumber));
        }
    }

    private long _facultyMemberId = 0;
    public long FacultyMemberId
    {
        get
        {
            return _facultyMemberId;
        }
        set
        {
            _facultyMemberId = value;
            OnPropertyChanged(nameof(FacultyMemberId));
            LoadFacultyMemberData();
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
            instance.FacultyMemberSubView = new FacultyMemberViewModel(_context, _dialogService, _databaseService);
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

        if (_facultyMember is null)
        {
            return;
        }

        _facultyMember.Name = Name;
        _facultyMember.Age = Age;
        _facultyMember.Gender = Gender;
        _facultyMember.Department = Department;
        _facultyMember.Position = Position;
        _facultyMember.Email = Email;
        _facultyMember.OfficeRoomNumber = OfficeRoomNumber;

        _databaseService.UpdateFacultyMember(_facultyMember);

        Response = "Data Saved";
    }

    private void LoadFacultyMemberData()
    {
        var facultyMembers = _context.FacultyMembers;
        if (facultyMembers is not null)
        {
            _facultyMember = _databaseService.FindFacultyMember(FacultyMemberId);

            if (_facultyMember is null)
            {
                return;
            }
            this.Name = _facultyMember.Name;
            this.Age = _facultyMember.Age;
            this.Gender = _facultyMember.Gender;
            this.Department = _facultyMember.Department;
            this.Position = _facultyMember.Position;
            this.Email = _facultyMember.Email;
            this.OfficeRoomNumber = _facultyMember.OfficeRoomNumber;

        }
    }

    private bool IsValid()
    {
        string[] properties = { "Name", "Gender", "Department", "Position", "Email", "OfficeRoomNumber" };
        foreach (string property in properties)
        {
            if (!string.IsNullOrEmpty(this[property]))
            {
                return false;
            }
        }
        return true;
    }


}
