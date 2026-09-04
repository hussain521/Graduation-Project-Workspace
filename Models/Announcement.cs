using System;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Models
{
    public class Announcement
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string Category { get; set; } = "Deadlines"; // Deadlines, Guidelines, Defense, General, Workshop
        public string Priority { get; set; } = "Important"; // Normal, Important, Urgent
        public string AuthorName { get; set; } = "Academic Committee";
        public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
        public string? ActionUrl { get; set; }
        public string? ActionLabel { get; set; }
    }
}