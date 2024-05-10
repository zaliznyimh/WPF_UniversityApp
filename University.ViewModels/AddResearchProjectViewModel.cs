using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public class AddResearchProjectViewModel : ViewModelBase
    {
        private readonly UniversityContext _context;
        private readonly IDialogService _dialogService;

        public string Error
        {
            get { return string.Empty; }
        }
        /// <summary>
        /// Property to check if there is text only in the number fields for entry 
        /// </summary>
        private static readonly Regex onlyNumbers = new Regex("[^0-9]+");
        
        /// <summary>
        /// Method for cheking number fields
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static bool IsTextInNumberInput(string text)
        {
            return !onlyNumbers.IsMatch(text);
        }

        public string this[string columnName]
        {
            get
            {
                
                if (columnName == "Description")
                {
                    if (string.IsNullOrEmpty(Description))
                    {
                        return "Description is Required";
                    }
                }
                if (columnName == "Budget")
                {
                    if (!IsTextInNumberInput(Budget.ToString()))
                    {
                        return "Invalid value of budget";
                    }
                }
                return string.Empty;
            }
        }

        private string _title = string.Empty;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string _description = string.Empty;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        private string _teamMember = string.Empty;
        public string TeamMember
        {
            get
            {
                return _teamMember;
            }
            set
            {
                _teamMember = value;
                OnPropertyChanged(nameof(TeamMember));
            }
        }

        private string _supervisor = string.Empty;
        public string Supervisor
        {
            get
            {
                return _supervisor;
            }
            set
            {
                _supervisor = value;
                OnPropertyChanged(nameof(Supervisor));
            }
        }

        private DateTime? _startDate = null;
        public DateTime? StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        private DateTime? _endDate = null;
        public DateTime? EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }

        private int _budget;
        public int Budget
        {
            get
            {
                return _budget;
            }
            set
            {
                _budget = value;
                OnPropertyChanged(nameof(Budget));
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

        private ObservableCollection<FacultyMember>? _assignedFacultyMembers = null;
        public ObservableCollection<FacultyMember>? AssignedFacultyMembers
        {
            get
            {
                if (_assignedFacultyMembers is null)
                {
                    _assignedFacultyMembers = LoadFacultyMembers();
                    return _assignedFacultyMembers;
                }
                return _assignedFacultyMembers;
            }
            set
            {
                _assignedFacultyMembers = value;
                OnPropertyChanged(nameof(AssignedFacultyMembers));
            }
        }

        private ObservableCollection<Student>? _assignedStudents = null;
        public ObservableCollection<Student> AssignedStudents
        {
            get
            {
                if (_assignedStudents is null)
                {
                    _assignedStudents = LoadStudents();
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
                instance.ResearchProjectSubView = new ResearchProjectViewModel(_context, _dialogService);
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

            ResearchProject researchProject = new ResearchProject
            {
                Title = this.Title,
                Description = this.Description,
                TeamMember = AssignedStudents?.Where(s => s.IsSelected).ToList(),
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                Budget = this.Budget,
                Supervisor = AssignedFacultyMembers?.Where(s => s.IsSelected).ToList()
            };

            _context.ResearchProjects.Add(researchProject);
            _context.SaveChanges();

            Response = "Data Saved";
        }

        private bool IsValid()
        {
    
            string[] properties = { "Title", "Description", "Budget"};
            foreach (string property in properties)
            {
                if (!string.IsNullOrEmpty(this[property]))
                {
                    return false;
                }
            }

            return true;
        
        }

        private ObservableCollection<FacultyMember>? LoadFacultyMembers()
        {
            _context.Database.EnsureCreated();
            _context.FacultyMembers.Load();
            return _context.FacultyMembers.Local.ToObservableCollection();
        }

        private ObservableCollection<Student> LoadStudents()
        {
            _context.Database.EnsureCreated();
            _context.Students.Load();
            return _context.Students.Local.ToObservableCollection();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dialogService"></param>
        public AddResearchProjectViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;
        }
    }
}
