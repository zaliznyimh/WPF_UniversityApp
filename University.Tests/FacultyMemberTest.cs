using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using University.Data;
using University.Interfaces;
using University.Models;
using University.Services;
using University.ViewModels;

namespace University.Tests;

public class FacultyMemberTest
{
    [TestClass]
    public class BooksTest
    {
        private IDialogService _dialogService;
        private DbContextOptions<UniversityContext> _options;
        private Mock<IDialogService> _dialogServiceMock;

        [TestInitialize()]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<UniversityContext>()
                .UseInMemoryDatabase(databaseName: "UniversityTestDB")
                .Options;
            SeedTestDB();
            _dialogService = new DialogService();
            _dialogServiceMock = new Mock<IDialogService>();
        }

        private void SeedTestDB()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                context.Database.EnsureDeleted();
                List<FacultyMember> facultyMembers = new List<FacultyMember>
                {
                new FacultyMember { FacultyMemberId = 1, Name = "Linus Torvalds", Age = 50, Department = "Computer Science", Gender = "Male", Email = "linus@gmail.com", Position = "Professor", OfficeRoomNumber = "10D" },
                new FacultyMember { FacultyMemberId = 2, Name = "Albert Einstein",Age = 70,Department = "Physics",Gender = "Male", Email = "einstein@outlook.com", Position = "Professor", OfficeRoomNumber = "3E" }
            };
                context.FacultyMembers.AddRange(facultyMembers);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void ShowAllBooks()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                FacultyMemberViewModel facultyMemberViewModel = new FacultyMemberViewModel(context, _dialogService);
                bool hasData = facultyMemberViewModel.FacultyMembers.Any();
                Assert.IsTrue(hasData);
            }
        }

        [TestMethod]
        public void AddFacultyMember_ValidInput_ReturnTrue()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddFacultyMemberViewModel addFacultyMemberViewModel = new AddFacultyMemberViewModel(context, _dialogService)
                {
                    Name = "Stephen Hawking", 
                    Age = 76,
                    Gender = "Male",
                    Position = "Theoretical Physicist",
                    Department = "Physics",
                    Email = "hawking@example.com",
                    OfficeRoomNumber = "101A",
                };

                // Act
                addFacultyMemberViewModel.Save.Execute(null);
                bool isNewFacultyMember = context.FacultyMembers.Any(fm => fm.Name == "Stephen Hawking" && fm.Age == 76 && fm.Email == "hawking@example.com");

                // Assert
                Assert.IsTrue(isNewFacultyMember);
            }
        }

        [TestMethod]
        public void AddFacultyMember_InvalidInput_ReturnTrue()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                // Arrange
                AddFacultyMemberViewModel addFacultyMemberViewModel = new AddFacultyMemberViewModel(context, _dialogService)
                {
                    Age = 76,
                    Gender = "Male",
                    Position = "Theoretical Physicist",
                    Department = "Physics",
                    Email = "hawking@example.com",
                    OfficeRoomNumber = "101A",
                };
                // Act

                addFacultyMemberViewModel.Save.Execute(null);
                bool isNewFacultyMemberExists = context.FacultyMembers.Any(fm => fm.Position == "Theoretical Physicist" && fm.Department == "Physics");
                // Assert
                Assert.IsFalse(isNewFacultyMemberExists);
            }
        }

        [TestMethod]
        public void EditFacultyMember_ValidFacultyMemberProperties_ReturnTrue()
        {
            using (UniversityContext context = new UniversityContext(_options))
            {
                // Arrrange
                long IdFacultyMemberToEdit = 1;
                var viewModel = new FacultyMemberViewModel(context, _dialogService);
                var facultyMemberToEdit = context.FacultyMembers.Find(IdFacultyMemberToEdit);

                // Act
                EditFacultyMemberViewModel editFacultyMemberViewModel = new EditFacultyMemberViewModel(context, _dialogService)
                {
                    FacultyMemberId = 1,
                    Department = "Information Technology"
                };

                editFacultyMemberViewModel.Save.Execute(facultyMemberToEdit);
                viewModel.Edit.Execute(facultyMemberToEdit.FacultyMemberId);
                var editedFacultyMember = context.FacultyMembers.Find(IdFacultyMemberToEdit);

                // Assert
                Assert.AreEqual("Information Technology", editedFacultyMember.Department);
            }
        }

        [TestMethod]
        public void EditFacultyMember_InValidFacultyMemberProperties_ReturnFalse()
        {
            using (UniversityContext context = new UniversityContext(_options))
            {
                // Arrange
                long IdFacultyMemberToEdit = 1;
                var viewModel = new FacultyMemberViewModel(context, _dialogService);
                var facultyMemberToEdit = context.FacultyMembers.Find(IdFacultyMemberToEdit);

                EditFacultyMemberViewModel editFacultyMemberViewModel = new EditFacultyMemberViewModel(context, _dialogService)
                {
                    FacultyMemberId = 20,
                    Department = "Information Technology"
                };

                // Act
                editFacultyMemberViewModel.Save.Execute(facultyMemberToEdit);
                viewModel.Edit.Execute(facultyMemberToEdit.FacultyMemberId);
                var editedFacultyMember = context.FacultyMembers.Find(IdFacultyMemberToEdit);

                // Assert
                Assert.AreNotEqual("Information Technology", editedFacultyMember.Department);
            }
        }

        [TestMethod]
        public void RemoveExistingFacultyMember_ValidMember_RemoveReturnTrue()
        {
            using (UniversityContext context = new UniversityContext(_options))
            {
                // Arrange
                long IdExistingMemberToRemove = 2;
                _dialogServiceMock.Setup(q => q.Show(It.IsAny<string>())).Returns(true);
                var viewModel = new FacultyMemberViewModel(context, _dialogServiceMock.Object);

                // Act
                viewModel.Remove.Execute(IdExistingMemberToRemove);

                // Assert
                Assert.IsFalse(viewModel.FacultyMembers.Any(b => b.FacultyMemberId == IdExistingMemberToRemove));
            }
        }

        [TestMethod]
        public void RemoveExistingFacultyMember_ValidMember_RemoveReturnFalse()
        {
            using (UniversityContext context = new UniversityContext(_options))
            {
                // Arrange
                long IdExistingMemberToRemove = 2;
                _dialogServiceMock.Setup(q => q.Show(It.IsAny<string>())).Returns(false);
                var viewModel = new FacultyMemberViewModel(context, _dialogServiceMock.Object);

                // Act
                viewModel.Remove.Execute(IdExistingMemberToRemove);

                // Assert
                Assert.IsTrue(viewModel.FacultyMembers.Any(b => b.FacultyMemberId == IdExistingMemberToRemove));
            }
        }

        [TestMethod]
        public void AddNewFacultyMember_ValidMemberPropeties_ReturnMessageDataSaved()
        {
            using (UniversityContext context = new UniversityContext(_options))
            {
                // Arrange
                var viewModel = new AddFacultyMemberViewModel(context, _dialogService)
                {
                    Name = "Marie Curie",
                    Age = 66,
                    Gender = "Female",
                    Position = "Physicist",
                    Department = "Physics",
                    Email = "curie@examplemail.pl",
                    OfficeRoomNumber = "Room 202",
                };

                // Act
                viewModel.Save.Execute(null);

                // Assert
                Assert.AreEqual("Data Saved", viewModel.Response);

            }
        }

        public void AddNewFacultyMember_NullMemberPropeties_ReturnMessagePleaseCompleteAllFields()
        {
            using (UniversityContext context = new UniversityContext(_options))
            {
                // Arrange
                var viewModel = new AddFacultyMemberViewModel(context, _dialogService)
                {
                    Gender = "",
                    Position = "",
                    Department = "",
                    Email = "",
                    OfficeRoomNumber = "",
                };

                // Act
                viewModel.Save.Execute(null);

                // Assert
                Assert.AreEqual("Please complete all required fields", viewModel.Response);

            }
        }
    }
}
