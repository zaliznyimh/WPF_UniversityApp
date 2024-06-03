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
    public Book FindBook(long BookID);
    public void UpdateBook(Book book);
    #endregion
}
