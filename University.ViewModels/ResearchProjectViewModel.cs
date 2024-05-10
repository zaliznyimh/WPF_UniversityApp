using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public class ResearchProjectViewModel : ViewModelBase
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

        private ObservableCollection<ResearchProject>? _researchProjects = null;
        public ObservableCollection<ResearchProject> ResearchProjects
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

        private ICommand? _add = null;
        public ICommand? Add
        {
            get
            {
                if (_add is null)
                {
                    _add = new RelayCommand<object>(AddNewResearchProject);
                }
                return _add;
            }
        }

        private void AddNewResearchProject(object? obj)
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.ResearchProjectSubView = new AddResearchProjectViewModel(_context, _dialogService);
            }
        }

        private ICommand? _edit;
        public ICommand? Edit
        {
            get
            {
                if (_edit is null)
                {
                    _edit = new RelayCommand<object>(EditResearchProject);
                }
                return _edit;
            }
        }

        private void EditResearchProject(object? obj)
        {
            if (obj is not null)
            {
                long projectId = (long)obj;
                EditResearchProjectViewModel editResearchProjectViewModel = new EditResearchProjectViewModel(_context, _dialogService)
                {
                    ProjectId = projectId
                };
                var instance = MainWindowViewModel.Instance();
                if (instance is not null)
                {
                    instance.ResearchProjectSubView = editResearchProjectViewModel;
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
                    _remove = new RelayCommand<object>(RemoveResearchProject);
                }
                return _remove;
            }
        }

        private void RemoveResearchProject(object? obj)
        {
            if (obj is not null)
            {
                long projectId = (long)obj;
                ResearchProject? researchProject = _context.ResearchProjects.Find(projectId);
                if (researchProject is not null)
                {
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

        // Ctor
        public ResearchProjectViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            _context.Database.EnsureCreated();
            _context.ResearchProjects.Load();
            ResearchProjects = _context.ResearchProjects.Local.ToObservableCollection();
        }
    }
}
