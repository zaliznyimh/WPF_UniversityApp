using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Models
{
    public class Book
    {
        public long BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author {  get; set; } = string.Empty;
        public string Publisher {  get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string Genre {  get; set; } = string.Empty;
        public string Description { get; set; }= string.Empty;
    }
}
