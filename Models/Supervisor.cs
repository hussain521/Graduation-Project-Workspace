using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Models
{
    public class Supervisor
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        public string Title { get; set; } = "Dr."; // Prof., Dr., Assoc. Prof., Eng.

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Department { get; set; } = "Computer Science";
        public string ResearchInterests { get; set; } = string.Empty;
        public string OfficeLocation { get; set; } = "Engineering Hall B-304";
        public int MaxProjectsQuota { get; set; } = 5;
        public string? AvatarUrl { get; set; }
        public string Phone { get; set; } = "+1 (555) 382-9912";

        public List<Project> SupervisedProjects { get; set; } = new();
    }
}