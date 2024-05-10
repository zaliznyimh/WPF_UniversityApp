using University.Models;
using Microsoft.EntityFrameworkCore;

namespace University.Data
{
    public class UniversityContext : DbContext
    {
        public UniversityContext()
        {
        }

        public UniversityContext(DbContextOptions<UniversityContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<FacultyMember> FacultyMembers { get; set; }
        public DbSet<ResearchProject> ResearchProjects { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase("UniversityDb");
                optionsBuilder.UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subject>().Ignore(s => s.IsSelected);
            modelBuilder.Entity<FacultyMember>().Ignore(s => s.IsSelected);

            modelBuilder.Entity<Student>().HasData(
                new Student { StudentId = 1, Name = "Wieńczysław", LastName = "Nowakowicz", PESEL = "PESEL1", BirthDate = new DateTime(1987, 05, 22) },
                new Student { StudentId = 2, Name = "Stanisław", LastName = "Nowakowicz", PESEL = "PESEL2", BirthDate = new DateTime(2019, 06, 25) },
                new Student { StudentId = 3, Name = "Eugenia", LastName = "Nowakowicz", PESEL = "PESEL3", BirthDate = new DateTime(2021, 06, 08) });

            modelBuilder.Entity<Subject>().HasData(
                new Subject { SubjectId = 1, Name = "Matematyka", Semester = "1", Lecturer = "Michalina Warszawa" },
                new Subject { SubjectId = 2, Name = "Biologia", Semester = "2", Lecturer = "Halina Katowice" },
                new Subject { SubjectId = 3, Name = "Chemia", Semester = "3", Lecturer = "Jan Nowak" } );

            modelBuilder.Entity<FacultyMember>().HasData(
                new FacultyMember { FacultyMemberId = 1, Name = "Michael Johnson", Age = 45, Department = "Physics", Gender = "Male",
                                    Email = "michaeljohnson@outlook.com", Position = "Lecturer", OfficeRoomNumber = "5C" },
                
                new FacultyMember { FacultyMemberId = 2, Name = "Linus Torvalds", Age = 50, Department = "Computer Science", Gender = "Male", 
                                    Email = "linus@gmail.com", Position = "Professor", OfficeRoomNumber = "10D" },
                
                new FacultyMember { FacultyMemberId = 3, Name = "Albert Einstein",Age = 70,Department = "Physics",Gender = "Male",
                                    Email = "einstein@outlook.com", Position = "Professor", OfficeRoomNumber = "3E" } );

            modelBuilder.Entity<ResearchProject>().HasData(
                new ResearchProject { ProjectId = 1, Title = "Quantum Computing Research", Description = "Exploring the potential of quantum computing in solving complex computational problems.",
                                      TeamMember = "Isaac Chuang, Wolfgang Ketterle", StartDate = new DateTime(2024, 1, 15), 
                                      EndDate = new DateTime(2025, 1, 15), Budget = 50000 },
                
                new ResearchProject { ProjectId = 2, Title = "Artificial Intelligence Ethics Study", Description = "Investigating ethical considerations in the development and deployment of artificial intelligence technologies.",
                                      TeamMember = "Sam Altman, Paul Graham", StartDate = new DateTime(2024, 3, 10),
                                      EndDate = new DateTime(2025, 3, 10), Budget = 75000 },
                
                new ResearchProject { ProjectId = 3, Title = "Space Exploration Mission Planning", Description = "Planning and simulation of manned missions to Mars and beyond.",
                                      TeamMember = "Michael Johnson, Albert Einstein", StartDate = new DateTime(2024, 5, 20),
                                      EndDate = new DateTime(2025, 5, 20), Budget = 100000 } );

            modelBuilder.Entity<Book>().HasData(
                new Book { BookId = 1, Title = "The C Programming Language", Author = "Brian W. Kernighan, Dennis M. Ritchie", Publisher = "Prentice Hall", 
                           ISBN = "9780131103627", Genre = "Programming", Description = "A well-known programming book written by the creators of the C programming language." },
                new Book { BookId = 2, Title = "C Programming Absolute Beginner's Guide", Author = "Greg Perry, Dean Miller", Publisher = "Que Publishing",
                           ISBN = "9780789751980", Genre = "Programming", Description = "A beginner's guide to C programming, providing a solid foundation for learning the language." },
                new Book { BookId = 3, Title = "Head First C", Author = "David Griffiths, Dawn Griffiths", Publisher = "O'Reilly Media", 
                           ISBN = "9780596515800", Genre = "Programming", Description = "A learner-friendly guide to the C programming language, using a visual and engaging approach." } );

        }
    }
}
