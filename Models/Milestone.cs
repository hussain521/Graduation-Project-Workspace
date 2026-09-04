using System;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Models
{
    public enum MilestoneStatus
    {
        Pending,
        InProgress,
        Submitted,
        Approved,
        RevisionRequired
    }

    public class Milestone
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public MilestoneStatus Status { get; set; } = MilestoneStatus.Pending;
        public int WeightPercentage { get; set; } = 20;
        public string? Feedback { get; set; }
        public string? DeliverableUrl { get; set; }
    }
}