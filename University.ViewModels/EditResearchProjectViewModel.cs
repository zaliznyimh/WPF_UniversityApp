using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public class EditResearchProjectViewModel : ViewModelBase
    {
        private readonly UniversityContext _context;
        private readonly IDialogService _dialogService;
        private ResearchProject? _researchProject = new ResearchProject();

        public EditResearchProjectViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;
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

        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "Title")
                {
                    if (string.IsNullOrEmpty(Title))
                    {
                        return "Title is Required";
                    }
                }
                if (columnName == "Budget")
                {
                    if (!IsTextInNumberInput(Budget.ToString()))
                    {
                        return "Invalid value of budget";
                    }
                }
                if (columnName == "Description")
                {
                    if (string.IsNullOrEmpty(Description))
                    {
                        return "Description is Required";
                    }
                }
                if (columnName == "TeamMember")
                {
                    if (string.IsNullOrEmpty(Description))
                    {
                        return "TeamMembers is Required";
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

        private DateTime? _startDate;
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

        private DateTime? _endDate;
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

        private long _projectId = 0;
        public long ProjectId
        {
            get
            {
                return _projectId;
            }
            set
            {
                _projectId = value;
                OnPropertyChanged(nameof(ProjectId));
                LoadResearchProjectData();
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
        public ObservableCollection<FacultyMember> AssignedFacultyMembers
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

        private ObservableCollection<FacultyMember> LoadFacultyMembers()
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

        private void SaveData(object? obj)
        {
            if (!IsValid())
            {
                Response = "Please complete all required fields";
                return;
            }

            if (_researchProject is null)
            {
                return;
            }

            _researchProject.Title = Title;
            _researchProject.Description = Description;
            _researchProject.TeamMember = AssignedStudents.Where(s => s.IsSelected).ToList();
            _researchProject.Supervisor = AssignedFacultyMembers.Where(s => s.IsSelected).ToList();
            _researchProject.StartDate = StartDate;
            _researchProject.EndDate = EndDate;
            _researchProject.Budget = Budget;

            _context.Entry(_researchProject).State = EntityState.Modified;
            _context.SaveChanges();

            Response = "Data Saved";
        }

        private void LoadResearchProjectData()
        {
            if (_context?.ResearchProjects is null)
            {
                return;
            }
            _researchProject = _context.ResearchProjects.Find(ProjectId);
            if (_researchProject is null)
            {
                return;
            }
            this.Title = _researchProject.Title;
            this.Description = _researchProject.Description;
            this.StartDate = _researchProject.StartDate;
            this.EndDate = _researchProject.EndDate;
            this.Budget = _researchProject.Budget;

            if (_researchProject.Supervisor is null)
            {
                return;
            }
            foreach (FacultyMember facultyMember in _researchProject.Supervisor)
            {
                if (facultyMember is not null && AssignedFacultyMembers is not null)
                {
                    var assignedSupervisors = AssignedFacultyMembers
                        .FirstOrDefault(s => s.FacultyMemberId == facultyMember.FacultyMemberId);
                    if (assignedSupervisors is not null)
                    {
                        assignedSupervisors.IsSelected = true;
                    }
                }
            }

            if (_researchProject.TeamMember is null)
            {
                return;
            }
            foreach (Student student in _researchProject.TeamMember)
            {
                if (student is not null && AssignedStudents is not null)
                {
                    var assignedStudents = AssignedStudents
                        .FirstOrDefault(s => s.StudentId == student.StudentId);
                    if (assignedStudents is not null)
                    {
                        assignedStudents.IsSelected = true;
                    }
                }
            }

        }

        private bool IsValid()
        {
            string[] properties = { "Title", "Budget", "Description"};
            foreach (string property in properties)
            {
                if (!string.IsNullOrEmpty(this[property]))
                {
                    return false;
                }
            }

            if (this.Budget <= 0)
            {
                return false;
            }

            return true;
        }
    }
}
