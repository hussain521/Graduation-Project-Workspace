using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Department { get; set; } = "Computer Science & Engineering";
        public string Major { get; set; } = "Software Engineering";
        public double GPA { get; set; } = 3.8;
        public string Role { get; set; } = "Team Lead";
        public string? AvatarUrl { get; set; }

        public int? ProjectId { get; set; }
        public Project? Project { get; set; }
    }
}