using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels;

public class SearchViewModel : ViewModelBase
{
    private readonly UniversityContext _context;
    private readonly IDialogService _dialogService;
    private readonly IDatabaseService _databaseService;

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

    private string _firstCondition = string.Empty;
    public string FirstCondition
    {
        get
        {
            return _firstCondition;
        }
        set
        {
            _firstCondition = value;
            OnPropertyChanged(nameof(FirstCondition));
        }
    }

    private string _secondCondition = string.Empty;
    public string SecondCondition
    {
        get
        {
            return _secondCondition;
        }
        set
        {
            _secondCondition = value;
            OnPropertyChanged(nameof(SecondCondition));
        }
    }

    private bool _isVisible;
    public bool IsVisible
    {
        get
        {
            return _isVisible;
        }
        set
        {
            _isVisible = value;
            OnPropertyChanged(nameof(IsVisible));
        }
    }

    private bool _areStudentsVisible;
    public bool AreStudentsVisible
    {
        get
        {
            return _areStudentsVisible;
        }
        set
        {
            _areStudentsVisible = value;
            OnPropertyChanged(nameof(AreStudentsVisible));
        }
    }

    private bool _areSubjectsVisible;
    public bool AreSubjectsVisible
    {
        get
        {
            return _areSubjectsVisible;
        }
        set
        {
            _areSubjectsVisible = value;
            OnPropertyChanged(nameof(AreSubjectsVisible));
        }
    }

    private bool _areBooksVisible;
    public bool AreBooksVisible
    {
        get
        {
            return _areBooksVisible;
        }
        set
        {
            _areBooksVisible = value;
            OnPropertyChanged(nameof(AreBooksVisible));
        }
    }


    private bool _areFacultyMembersVisible;
    public bool AreFacultyMembersVisible
    {
        get
        {
            return _areFacultyMembersVisible;
        }
        set
        {
            _areFacultyMembersVisible = value;
            OnPropertyChanged(nameof(AreFacultyMembersVisible));
        }
    }

    private bool _areResearchProjectVisible;
    public bool AreResearchProjectVisible
    {
        get
        {
            return _areResearchProjectVisible;
        }
        set
        {
            _areResearchProjectVisible = value;
            OnPropertyChanged(nameof(AreResearchProjectVisible));
        }
    }


    private ObservableCollection<Book>? _books = null;
    public ObservableCollection<Book>? Books
    {
        get
        {
            if (_books is null)
            {
                _books = new ObservableCollection<Book>();
                return _books;
            }
            return _books;
        }
        set
        {
            _books = value;
            OnPropertyChanged(nameof(Books));
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

    private ObservableCollection<Subject>? _subjects = null;
    public ObservableCollection<Subject>? Subjects
    {
        get
        {
            if (_subjects is null)
            {
                _subjects = new ObservableCollection<Subject>();
                return _subjects;
            }
            return _subjects;
        }
        set
        {
            _subjects = value;
            OnPropertyChanged(nameof(Subjects));
        }
    }

    private ObservableCollection<FacultyMember>? _facultyMembers = null;
    public ObservableCollection<FacultyMember>? FacultyMembers
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

    private ObservableCollection<ResearchProject>? _researchProjects = null;
    public ObservableCollection<ResearchProject>? ResearchProjects
    {
        get
        {
            if (_researchProjects is null)
            {
                _researchProjects = new ObservableCollection<ResearchProject>();
                return _researchProjects;
            }
            return _researchProjects;
        }
        set
        {
            _researchProjects = value;
            OnPropertyChanged(nameof(ResearchProjects));
        }
    }

    private ICommand? _comboBoxSelectionChanged = null;
    public ICommand? ComboBoxSelectionChanged
    {
        get
        {
            if (_comboBoxSelectionChanged is null)
            {
                _comboBoxSelectionChanged = new RelayCommand<object>(UpdateCondition);
            }
            return _comboBoxSelectionChanged;
        }
    }

    private void UpdateCondition(object? obj)
    {
        if (obj is string objAsString)
        {
            IsVisible = true;
            string selectedValue = objAsString;
            SecondCondition = string.Empty;
            if (selectedValue == "Students")
            {
                FirstCondition = "who attends";
            }
            else if (selectedValue == "Subjects")
            {
                FirstCondition = "attended by Student with PESEL";
            }
            else if (selectedValue == "Books")
            {
                FirstCondition = "with ISBN";
            }
            else if (selectedValue == "Faculty Members")
            {
                FirstCondition = "attended by FacultyMember with name";
            }
            else if (selectedValue == "Research Projects")
            {
                FirstCondition = "with title";
            }
        }
    }

    private ICommand? _search = null;
    public ICommand? Search
    {
        get
        {
            if (_search is null)
            {
                _search = new RelayCommand<object>(SelectData);
            }
            return _search;
        }
    }

    private void SelectData(object? obj)
    {
        if (FirstCondition == "who attends")
        {
            _context.Database.EnsureCreated();
            Subject? subject = _context.Subjects.Where(s => s.Name == SecondCondition).FirstOrDefault();
            if (subject is not null)
            {
                var students = _context.Students
                    .Include(s => s.Subjects)
                    .ToList();

                var filteredStudents = students
                    .Where(s => s.Subjects != null && s.Subjects.Any(sub => sub.Name == subject.Name))
                    .ToList();

                Students = new ObservableCollection<Student>(filteredStudents);
                AreSubjectsVisible = false;
                AreStudentsVisible = true;
            }
        }
        else if (FirstCondition == "attended by Student with PESEL")
        {
            _context.Database.EnsureCreated();
            Student? student = _context.Students
                .Where(s => s.PESEL == SecondCondition)
                .FirstOrDefault();
            if (student is not null)
            {
                var subjects = _context.Subjects
                    .Include(s => s.Students)
                    .ToList();

                var filteredSubjects = subjects
                    .Where(s => s.Students != null && s.Students.Any(sub => sub.PESEL == SecondCondition))
                    .ToList();

                Subjects = new ObservableCollection<Subject>(filteredSubjects);
                AreStudentsVisible = false;
                AreSubjectsVisible = true;
            }
        }
        else if (FirstCondition == "with ISBN")
        {
            _context.Database.EnsureCreated();
            Book? book = _context.Books
                .Where(b => b.ISBN == SecondCondition)
                .FirstOrDefault();
            if (book is not null)
            {
                var books = _context.Books.ToList();

                var filteredBooks = books.Where(b => b.ISBN == SecondCondition).ToList();

                Books = new ObservableCollection<Book>(filteredBooks);
                AreBooksVisible = true;
            }
        }
        else if (FirstCondition == "attended by FacultyMember with name")
        {
            _context.Database.EnsureCreated();
            FacultyMember? facultyMember = _context.FacultyMembers
                .Where(fm => fm.Name == SecondCondition)
                .FirstOrDefault();
            if (facultyMember is not null)
            {
                var facultyMemberToShow = _context.FacultyMembers.ToList();

                var filteredFacultyMember = facultyMemberToShow.Where(b => b.Name == SecondCondition).ToList();

                FacultyMembers = new ObservableCollection<FacultyMember>(filteredFacultyMember);
                AreFacultyMembersVisible = true;
            }
        }
        else if (FirstCondition == "with title")
        {
            _context.Database.EnsureCreated();
            ResearchProject? researchProject = _context.ResearchProjects.Where(s => s.Title == SecondCondition).FirstOrDefault();
            if (researchProject is not null)
            {
                var researchProjects = _context.ResearchProjects
                    .Include(s => s.Supervisor)
                    .ToList();

                var supervisorName = researchProject.Supervisor?.Select(s => s.Name).ToList();

                if (supervisorName is not null)
                {
                    var filteredResearchProjects = researchProjects
                        .Where(rp => rp.Supervisor != null && rp.Supervisor.Any(f => supervisorName.Contains(f.Name)))
                        .ToList();

                ResearchProjects = new ObservableCollection<ResearchProject>(filteredResearchProjects);
                AreResearchProjectVisible = true;
                }
            }
        }
    }

    private ICommand? _edit = null;
    public ICommand? Edit
    {
        get
        {
            if (_edit is null)
            {
                _edit = new RelayCommand<object>(EditItem);
            }
            return _edit;
        }
    }

    private void EditItem(object? obj)
    {
        if (obj is not null)
        {
            if (FirstCondition == "who attends")
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
                    instance.SelectedTab = 0;
                }
            }
            else if (FirstCondition == "attended by Student with PESEL")
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
                    instance.SelectedTab = 1;
                }
            }
            else if (FirstCondition == "with ISBN")
            {
                long bookId = (long)obj;
                EditBookViewModel editBookViewModel = new EditBookViewModel(_context, _dialogService, _databaseService)
                {
                    BookId = bookId
                };
                var instance = MainWindowViewModel.Instance();
                if (instance is not null)
                {
                    instance.BooksSubView = editBookViewModel;
                    instance.SelectedTab = 2;
                }
            }
            else if (FirstCondition == "attended by FacultyMember with name")
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
                    instance.SelectedTab = 3;
                }
            }
            else if (FirstCondition == "with title")
            {
                long researchProjectId = (long)obj;
                EditResearchProjectViewModel editResearchProjectViewModel = new EditResearchProjectViewModel(_context, _dialogService, _databaseService)
                {
                    ProjectId = researchProjectId
                };
                var instance = MainWindowViewModel.Instance();
                if (instance is not null)
                {
                    instance.ResearchProjectSubView = editResearchProjectViewModel;
                    instance.SelectedTab = 4;
                }
            }
        }
    }

    private ICommand ?_remove = null;
    public ICommand? Remove
    {
        get
        {
            if (_remove is null)
            {
                _remove = new RelayCommand<object>(RemoveItem);
            }
            return _remove;
        }
    }

    private void RemoveItem(object? obj)
    {
        if (obj is not null)
        {
            if (FirstCondition == "who attends")
            {
                long studentId = (long)obj;
                Student? student = _context.Students.Find(studentId);
                if (student is null)
                {
                    return;
                }

                DialogResult = _dialogService.Show(student.Name + " " + student.LastName);
                if (DialogResult == false)
                {
                    return;
                }
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
            else if (FirstCondition == "attended by Student with PESEL")
            {
                long subjectId = (long)obj;
                Subject? subject = _context.Subjects.Find(subjectId);
                if (subject is null)
                {
                    return;
                }
                DialogResult = _dialogService.Show(subject.Name);
                if (DialogResult == false)
                {
                    return;
                }
                _context.Subjects.Remove(subject);
                _context.SaveChanges();
            }
            else if (FirstCondition == "with ISBN")
            {
                long bookId = (long)obj;
                Book? book = _context.Books.Find(bookId);
                if (book is null)
                {
                    return;
                }
                DialogResult = _dialogService.Show(book.Title);
                if (DialogResult == false)
                {
                    return;
                }
                _context.Books.Remove(book);
                _context.SaveChanges();
            }
            else if (FirstCondition == "attended by FacultyMember with name")
            {
                long facultyMemberID = (long)obj;
                FacultyMember? facultyMember = _context.FacultyMembers.Find(facultyMemberID);
                if (facultyMember is null)
                {
                    return;
                }
                DialogResult = _dialogService.Show(facultyMember.Name);
                if (DialogResult == false)
                {
                    return;
                }
                _context.FacultyMembers.Remove(facultyMember);
                _context.SaveChanges();
            }
            else if (FirstCondition == "with title")
            {
                long projectId = (long)obj;
                ResearchProject? researchProject = _context.ResearchProjects.Find(projectId);
                if (researchProject is null)
                {
                    return;
                }
                DialogResult = _dialogService.Show(researchProject.Title);
                if (DialogResult == false)
                {
                    return;
                }
                _context.ResearchProjects.Remove(researchProject);
                _context.SaveChanges();
            }
        }
    }

    public SearchViewModel(UniversityContext context, IDialogService dialogService, IDatabaseService databaseService)
    {
        _context = context;
        _dialogService = dialogService;
        _databaseService = databaseService;

        IsVisible = false;
        AreStudentsVisible = false;
        AreSubjectsVisible = false;
        AreBooksVisible = false;
        AreFacultyMembersVisible = false;
        AreResearchProjectVisible = false;
    }
}
