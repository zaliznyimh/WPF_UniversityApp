using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Xml.Linq;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels;

public class AddFacultyMemberViewModel : ViewModelBase
{
    private readonly UniversityContext _context;
    private readonly IDialogService _dialogService;
    private readonly IDatabaseService _databaseService;

    public string Error
    {
        get { return string.Empty; }
    }

    private static readonly Regex onlyNumbers = new Regex("[^0-9.-]+"); 

    private static bool IsTextAllowed(string text)
    {
        return !onlyNumbers.IsMatch(text);
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
            if (columnName == "Age")
            {
                if (!IsTextAllowed(Age.ToString()))
                {
                    return "Invalid value of Age";
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
                if (string.IsNullOrEmpty(Position))
                {
                    return "Position is Required";
                }
            }
            if (columnName == "Email")
            {
                if (string.IsNullOrEmpty(Email))
                {
                    return "Email is Required";
                }
            }
            if (columnName == "OfficeRoomNumber")
            {
                if (string.IsNullOrEmpty(OfficeRoomNumber))
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

    private int _age;
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
    public ICommand? Save
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

        FacultyMember facultyMember = new FacultyMember
        {
            Name = this.Name,
            Age = this.Age,
            Gender = this.Gender,
            Department = this.Department,
            Position = this.Position,
            Email = this.Email,
            OfficeRoomNumber = this.OfficeRoomNumber
        };

        _databaseService.SaveFacultyMember(facultyMember);

        Response = "Data Saved";
    }

    private bool IsValid()
    {
        string[] properties = { "Name", "Age", "Gender", "Department", "Position", "Email", "OfficeRoomNumber" };
        foreach (string property in properties)
        {
            if (!string.IsNullOrEmpty(this[property]))
            {
                return false;
            }
        }

        if (this.Age <= 0)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="context"></param>
    /// <param name="dialogService"></param>
    public AddFacultyMemberViewModel(UniversityContext context, IDialogService dialogService, IDatabaseService databaseService)
    {
        _context = context;
        _dialogService = dialogService;
        _databaseService = databaseService;
    }
}
