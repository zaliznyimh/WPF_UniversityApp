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

namespace University.Tests
{
    [TestClass]
    public class BooksTest
    {
        private IDatabaseService _databaseService;
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
                List<Book> books = new List<Book>
                {
                    new Book { BookId = 1, Title = "It", Author = "Stephen King", Publisher = "Publisher 1", ISBN = "ISBN1", Genre = "Fable", Description = "Description 1" },
                    new Book { BookId = 2, Title = "Dune", Author = "Frank Herbert", Publisher = "Publisher 2", ISBN = "ISBN2", Genre = "Science Fiction", Description = "Description 2" },
                    new Book { BookId = 3, Title = "Redwall", Author = "Brian Jacques", Publisher = "Publisher 3", ISBN = "ISBN3", Genre = "Fantasy", Description = "Description 3" }
                };
                context.Books.AddRange(books);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void ShowAllBooks()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                BooksViewModel booksViewModel = new BooksViewModel(context, _dialogService);
                bool hasData = booksViewModel.Books.Any();
                Assert.IsTrue(hasData);
            }
        }

        [TestMethod]
        public void AddBook_ValidBookProperties_ReturnTrue()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                // Arrange
                AddBookViewModel addBookViewModel = new AddBookViewModel(context, _dialogService)
                {
                    Title = "New Book Title",
                    Author = "New Author",
                    Publisher = "New Publisher",
                    ISBN = "New ISBN",
                    Genre = "New Genre",
                    Description = "New Description"
                };

                // Act
                addBookViewModel.Save.Execute(null);
                bool newBookExists = context.Books.Any(b => b.Title == "New Book Title" && b.Author == "New Author" && b.Publisher == "New Publisher" && b.ISBN == "New ISBN" && b.Genre == "New Genre" && b.Description == "New Description");

                // Assert
                Assert.IsTrue(newBookExists);
            }
        }

        [TestMethod]
        public void AddBook_WithoutAllValidProperties_ReturnFalse()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                // Arrange
                AddBookViewModel addBookViewModel = new AddBookViewModel(context, _dialogService)
                {
                    Author = "Author without Title",
                    Publisher = "Publisher without Title",
                    ISBN = "ISBN without Title",
                    Genre = "Genre without Title",
                    Description = "Description without Title"
                };
                // Act

                addBookViewModel.Save.Execute(null);
                bool newBookExists = context.Books.Any(b => b.Author == "Author without Title" && b.Publisher == "Publisher without Title" && b.ISBN == "ISBN without Title" && b.Genre == "Genre without Title" && b.Description == "Description without Title");
                // Assert
                Assert.IsFalse(newBookExists);
            }
        }

        [TestMethod]
        public void EditBook_ValidBookProperties_ReturnTrue()
        {
            using (UniversityContext context = new UniversityContext(_options))
            {
                // Arrrange
                var viewModel = new BooksViewModel(context, _dialogService);
                var bookToEdit = context.Books.Find((long)2);

                // Act
                EditBookViewModel editBookViewModel = new EditBookViewModel(context, _dialogService)
                {
                    BookId = 2,
                    Genre = "NewGenre",
                    Description = "Description"
                };

                editBookViewModel.Save.Execute(bookToEdit);
                viewModel.Edit.Execute(bookToEdit.BookId);
                var editedBook = context.Books.Find((long)2);

                // Assert
                Assert.AreEqual("NewGenre", editedBook.Genre);
            }
        }

        [TestMethod]
        public void EditBook_NotValidBookEnteties_ReturnFalse()
        {
            using (UniversityContext context = new UniversityContext(_options))
            {
                // Arrange
                long IdExistingOfBook = 2;
                var viewModel = new BooksViewModel(context, _dialogService);
                var bookToEdit = context.Books.Find(IdExistingOfBook);

                EditBookViewModel editBookViewModel = new EditBookViewModel(context, _dialogService)
                {
                    BookId = 3,
                    Genre = "NewGenre",
                    Description = "NewDescription"
                };

                // Act
                editBookViewModel.Save.Execute(bookToEdit);
                viewModel.Edit.Execute(bookToEdit.BookId);
                var editedBook = context.Books.Find(IdExistingOfBook);

                // Assert
                Assert.AreNotEqual("NewGenre", editedBook.Genre);
            }
        }

        [TestMethod]
        public void RemoveExistingBook_ValidBook_RemoveTrue()
        {
            using (UniversityContext context = new UniversityContext(_options))
            {
                // Arrange
                long IdExistingBookToRemove = 2;
                _dialogServiceMock.Setup(q => q.Show(It.IsAny<string>())).Returns(true);
                var viewModel = new BooksViewModel(context, _dialogServiceMock.Object);

                // Act
                viewModel.Remove.Execute(IdExistingBookToRemove);

                // Assert
                Assert.IsFalse(viewModel.Books.Any(b => b.BookId == IdExistingBookToRemove));
            }
        }

        [TestMethod]
        public void RemoveExistingBook_ValidBook_RemoveFalse()
        {
            using (UniversityContext context = new UniversityContext(_options))
            {
                // Arrange
                long IdExistingBookToRemove = 3;
                _dialogServiceMock.Setup(q => q.Show(It.IsAny<string>())).Returns(false);
                var viewModel = new BooksViewModel(context, _dialogServiceMock.Object);

                // Act
                viewModel.Remove.Execute(IdExistingBookToRemove);

                // Assert
                Assert.IsTrue(viewModel.Books.Any(b => b.BookId == IdExistingBookToRemove));
            }
        }

        [TestMethod]
        public void AddNewBook_ValidBookPropeties_ReturnDataSaved()
        {
            using (UniversityContext context = new UniversityContext(_options))
            {
                // Arrange
                var viewModel = new AddBookViewModel(context, _dialogService)
                {
                    Title = "Test Title",
                    Author = "Test Author",
                    Publisher = "Test Publisher",
                    ISBN = "Test ISBN",
                    Genre = "Test Genre",
                    Description = "Test Description"
                };

                // Act
                viewModel.Save.Execute(null);

                // Assert
                Assert.AreEqual("Data Saved", viewModel.Response);

            }
        }

        [TestMethod]
        public void SaveData_InvalidBookProperties_ReturnsPleaseCompleteAllFields()
        {
            using (UniversityContext context = new UniversityContext(_options))
            {
                // Arrange
                var viewModel = new AddBookViewModel(context, _dialogServiceMock.Object)
                {
                    // Здесь не устанавливаем значения для обязательных полей
                    Title = "",
                    Author = "",
                    Publisher = "",
                    ISBN = "",
                    Genre = "",
                    Description = ""
                };

                // Act
                viewModel.Save.Execute(null);

                // Assert
                Assert.AreEqual("Please complete all required fields", viewModel.Response);
            }
        }
    }
}
