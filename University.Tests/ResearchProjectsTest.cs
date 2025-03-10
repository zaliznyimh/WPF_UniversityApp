﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using University.Data;
using University.Interfaces;
using University.Models;
using University.Services;
using University.ViewModels;

namespace University.Tests;

[TestClass]
public class ResearchProjectsTest
{
    private DbContextOptions<UniversityContext> _options;
    private Mock<IDialogService> _dialogServiceMock;
    private UniversityContext _context;
    private IDatabaseService _databaseService;

    [TestInitialize()]
    public void Initialize()
    {
        _options = new DbContextOptionsBuilder<UniversityContext>()
            .UseInMemoryDatabase(databaseName: "UniversityTestDB")
            .Options;
        _context = new UniversityContext(_options);
        SeedTestDB();

        _dialogServiceMock = new Mock<IDialogService>();
        _dialogServiceMock.Setup(q => q.Show(It.IsAny<string>())).Returns(true);
        _databaseService = new DatabaseService(_context, _dialogServiceMock.Object);
    }

    private void SeedTestDB()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            context.Database.EnsureDeleted();
            List<ResearchProject> researchProjects = new List<ResearchProject>
            {
                new ResearchProject { ProjectId = 1, Title = "Exploration of Artificial Intelligence Applications", Description = "Exploring various applications of AI",
                                      StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2024, 1, 1), Budget = 8000 },
                new ResearchProject { ProjectId = 10, Title = "Investigation of Climate Change Effects", Description = "Studying the impact of climate change on ecosystems",
                                      StartDate = new DateTime(2022, 1, 1), EndDate = new DateTime(2025, 1, 1), Budget = 10000 }
            };
            List<FacultyMember> facultyMembers = new List<FacultyMember>
            {
                new FacultyMember { FacultyMemberId = 1, Name = "Linus Torvalds", Age = 50, Department = "Computer Science", Gender = "Male", Email = "linus@gmail.com", Position = "Professor", OfficeRoomNumber = "10D" },
                new FacultyMember { FacultyMemberId = 2, Name = "Albert Einstein",Age = 70,Department = "Physics",Gender = "Male", Email = "einstein@outlook.com", Position = "Professor", OfficeRoomNumber = "3E" } 
            };

            context.ResearchProjects.AddRange(researchProjects);
            context.FacultyMembers.AddRange(facultyMembers);
            context.SaveChanges();
        }
    }

    [TestMethod]
    public void AddReseachProject_WithoutFacultyMembers_ReturnTrue()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            AddResearchProjectViewModel addResearchProjectViewModel = new AddResearchProjectViewModel(_context, _dialogServiceMock.Object, _databaseService)
            {
                Title = "Study of Urbanization Trends",
                Description = "Analyzing patterns of urbanization, urban growth, and their impact on infrastructure",
                StartDate = new DateTime(2022, 1, 1), 
                EndDate = new DateTime(2023, 1, 1),
                Budget = 4500
            };
            addResearchProjectViewModel.Save.Execute(null);

            bool isNewResearchProjectExist = context.ResearchProjects.Any(rp => rp.Title == "Study of Urbanization Trends" && rp.Budget == 4500);
            Assert.IsTrue(isNewResearchProjectExist);
        }
    }

    [TestMethod]
    public void AddReseachProject_WithFacultyMembers_ReturnTrue()
    {
            // Arrange
            long IdFacultyMember = 2;
            FacultyMember facultyMember = _context.FacultyMembers.Find(IdFacultyMember);
            facultyMember.IsSelected = true;

            // Act
            AddResearchProjectViewModel addResearchProjectViewModel = new AddResearchProjectViewModel(_context, _dialogServiceMock.Object, _databaseService)
            {
                Title = "Study of Urbanization Trends",
                Description = "Analyzing patterns of urbanization, urban growth, and their impact on infrastructure",
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2023, 1, 1),
                Budget = 4500,
                AssignedFacultyMembers = new ObservableCollection<FacultyMember>
                {
                    facultyMember
                }
            };
            addResearchProjectViewModel.AssignedFacultyMembers.Add(facultyMember);


            addResearchProjectViewModel.Save.Execute(null);

            // Assert
            bool isNewResearchProjectExist = _context.ResearchProjects.Any(rp => rp.Title == "Study of Urbanization Trends" && rp.Budget == 4500 && rp.Supervisor.Any());
            Assert.IsTrue(isNewResearchProjectExist);
    }

    [TestMethod]
    public void AddReseachProject_WithFacultyMembers_ReturnFalse()
    {
            // Arrange
            long IdFacultyMember = 2;
            FacultyMember facultyMember = _context.FacultyMembers.Find(IdFacultyMember);
            facultyMember.IsSelected = true;

            // Act
            AddResearchProjectViewModel addResearchProjectViewModel = new AddResearchProjectViewModel(_context, _dialogServiceMock.Object, _databaseService)
            {
                Title = "",
                Description = "Analyzing patterns of urbanization, urban growth, and their impact on infrastructure",
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2023, 1, 1),
                AssignedFacultyMembers = new ObservableCollection<FacultyMember>
                {
                    facultyMember
                }
            };
            addResearchProjectViewModel.Save.Execute(null);

            // Assert
            bool isNewResearchProjectExist = _context.ResearchProjects.Any(rp => rp.Title == "Study of Urbanization Trends" && rp.Budget == 4500 && rp.Supervisor.Any());
            Assert.IsFalse(isNewResearchProjectExist);
    }

    [TestMethod]
    public void EditResearchProject_ValidResearchProjectProperties_ReturnTrue()
    {
        using (UniversityContext context = new UniversityContext(_options))
        {
            // Arrrange
            long IdResearchProjectToEdit = 1;
            var researchProjectEdit = context.ResearchProjects.Find(IdResearchProjectToEdit);

            // Act
            EditResearchProjectViewModel editResearchProjectViewModel = new EditResearchProjectViewModel(context, _dialogServiceMock.Object, _databaseService)
            {
                ProjectId = 1,
                Budget = 9500
            };

            editResearchProjectViewModel.Save.Execute(null);
            
            // Assert
            var editedtResearchProject = _context.ResearchProjects.Find((long)1);
            Assert.AreEqual(9500, editedtResearchProject.Budget);
        }
    }

    [TestMethod]
    public void EditResearchProject_ValidResearchProjectPropertiesWithFacultyMember_ReturnTrue()
    {
        using (UniversityContext context = new UniversityContext(_options))
        {
            // Arrrange
            long IdResearchProjectToEdit = 1;
            long IdFacultyMember = 2;
            FacultyMember facultyMember = _context.FacultyMembers.Find(IdFacultyMember);
            facultyMember.IsSelected = true;
            var researchProjectEdit = context.ResearchProjects.Find(IdResearchProjectToEdit);

            // Act
            EditResearchProjectViewModel editResearchProjectViewModel = new EditResearchProjectViewModel(context, _dialogServiceMock.Object, _databaseService)
            {
                ProjectId = 1,
                Budget = 9500,
                AssignedFacultyMembers = new ObservableCollection<FacultyMember>
                {
                    facultyMember
                }
            };

            editResearchProjectViewModel.Save.Execute(null);

            // Assert
            var editedtResearchProject = _context.ResearchProjects.Find((long)1);
            Assert.AreEqual(9500, editedtResearchProject.Budget);
        }
    }

    [TestMethod]
    public void EditResearchProject_InvalidResearchProjectPropertiesWithFacultyMember_ReturnFalse()
    {
        using (UniversityContext context = new UniversityContext(_options))
        {
            // Arrrange
            long IdResearchProjectToEdit = 1;
            long IdFacultyMember = 2;
            FacultyMember facultyMember = _context.FacultyMembers.Find(IdFacultyMember);
            facultyMember.IsSelected = true;
            var viewModel = new ResearchProjectViewModel(context, _dialogServiceMock.Object, _databaseService);
            var researchProjectEdit = context.ResearchProjects.Find(IdResearchProjectToEdit);

            // Act
            EditResearchProjectViewModel editResearchProjectViewModel = new EditResearchProjectViewModel(context, _dialogServiceMock.Object, _databaseService)
            {
                ProjectId = 3,
                Budget = 9500,
                AssignedFacultyMembers = new ObservableCollection<FacultyMember>
                {
                    facultyMember
                }
            };

            editResearchProjectViewModel.Save.Execute(researchProjectEdit);
            viewModel.Edit.Execute(researchProjectEdit.ProjectId);

            // Assert
            var editedtResearchProject = context.ResearchProjects.Find(IdResearchProjectToEdit);
            Assert.AreNotEqual(9500, editedtResearchProject.Budget);
        }
    }

    [TestMethod]
    public void EditResearchProject_InvalidResearchProjectProperties_ReturnFalse()
    {
        using (UniversityContext context = new UniversityContext(_options))
        {
            // Arrrange
            long IdResearchProjectToEdit = 1;
            var viewModel = new ResearchProjectViewModel(context, _dialogServiceMock.Object, _databaseService);
            var researchProjectEdit = context.ResearchProjects.Find(IdResearchProjectToEdit);

            // Act
            EditResearchProjectViewModel editResearchProjectViewModel = new EditResearchProjectViewModel(context, _dialogServiceMock.Object, _databaseService)
            {
                ProjectId = 3,
                Budget = 9500
            };

            editResearchProjectViewModel.Save.Execute(researchProjectEdit);
            viewModel.Edit.Execute(researchProjectEdit.ProjectId);

            // Assert
            var editedtResearchProject = context.ResearchProjects.Find(IdResearchProjectToEdit);
            Assert.AreNotEqual(9500, editedtResearchProject.Budget);
        }
    }

    [TestMethod]
    public void RemoveExistingResearchProject_DialogChooseYesToDelete_ReturnTrue()
    {
        using (UniversityContext context = new UniversityContext(_options))
        {
            // Arrange
            long IdResearchProjectRemove = 2;
            var viewModel = new ResearchProjectViewModel(_context, _dialogServiceMock.Object, _databaseService);

            // Act
            viewModel.Remove.Execute(IdResearchProjectRemove);

            // Assert
            Assert.IsFalse(viewModel.ResearchProjects.Any(rp => rp.ProjectId == IdResearchProjectRemove));
        }
    }

    [TestMethod]
    public void RemoveExistingResearchProject_DialogChooseNoToDelete_ReturnFalse()
    {
        using (UniversityContext context = new UniversityContext(_options))
        {
            // Arrange
            long IdResearchProjectRemove = 10;
            _dialogServiceMock.Setup(qq => qq.Show(It.IsAny<string>())).Returns(false);
            var viewModel = new ResearchProjectViewModel(context, _dialogServiceMock.Object, _databaseService);

            // Act
            viewModel.Remove.Execute(IdResearchProjectRemove);

            // Assert
            Assert.IsTrue(viewModel.ResearchProjects.Any(rp => rp.ProjectId == IdResearchProjectRemove));
        }
    }

    [TestMethod]
    public void AddNewResearchProject_ValidMemberPropeties_ReturnMessageDataSaved()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            var addResearchProjectViewModel = new AddResearchProjectViewModel(context, _dialogServiceMock.Object, _databaseService)
            {
                Title = "Study of Urbanization Trends",
                Description = "Analyzing patterns of urbanization, urban growth, and their impact on infrastructure",
                TeamMember = "Michał, Radosław",
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2023, 1, 1),
                Budget = 4500
            };

            addResearchProjectViewModel.Save.Execute(null);

            // Assert
            Assert.AreEqual("Data Saved", addResearchProjectViewModel.Response);
        }
    }

    [TestMethod]
    public void AddNewResearchProject_ValidMemberPropeties_ReturnMessagePleaseCompleteAllFields()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            var addResearchProjectViewModel = new AddResearchProjectViewModel(context, _dialogServiceMock.Object, _databaseService)
            {
                Title = "",
                Description = "",
                TeamMember = "",
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2023, 1, 1),
                Budget = 4500
            };

            addResearchProjectViewModel.Save.Execute(null);

            // Assert
            Assert.AreEqual("Please complete all required fields", addResearchProjectViewModel.Response);
        }
    }

}
