using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace University.Models;

public class ResearchProject
{
    [Key]
    public long ProjectId { get; set; } = 0;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    // public string TeamMember { get; set; } = string.Empty;
    public virtual ICollection<Student>? TeamMember { get; set; } = null;
    public virtual ICollection<FacultyMember>? Supervisor { get; set; } = null;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Budget { get; set; }
}
