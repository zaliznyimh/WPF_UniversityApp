using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using University.Models;

namespace University.Interfaces;

public interface IDatabaseService
{
    #region Books Methods
    public void RemoveBook(object? obj);
    public void SaveBook(Book book);
    public Book FindBook(long bookID);
    public void UpdateBook(Book book);

    #endregion // Books Methods

    #region FacultyMember Methods
    public void RemoveFacultyMember(object? obj);
    public void SaveFacultyMember(FacultyMember facultyMember);
    public FacultyMember FindFacultyMember(long facultyMemberID);
    public void UpdateFacultyMember(FacultyMember facultyMember);

    #endregion // FacultyMember Methods

    #region ResearchProject Methods
    public void RemoveResearchProject(object? obj);
    public void SaveResearchProject(ResearchProject researchProject);
    public ResearchProject FindResearchProject(long researchProjectID);
    public void UpdateResearchProject(ResearchProject researchProject);

    #endregion // ResearchProject Methods
}
