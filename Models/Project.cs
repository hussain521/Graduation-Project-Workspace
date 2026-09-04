using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Models
{
    public enum ProjectTrack
    {
        ArtificialIntelligence,
        Cybersecurity,
        CloudAndDevOps,
        SoftwareEngineering,
        IoTAndEmbedded,
        DataScienceAndAnalytics
    }

    public enum ProjectStatus
    {
        Proposed,
        Approved,
        InProgress,
        ReadyForDefense,
        Defended,
        Published
    }

    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(3000)]
        public string Abstract { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ProjectTrack Track { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Proposed;

        [StringLength(20)]
        public string AcademicYear { get; set; } = "2025-2026";

        [StringLength(20)]
        public string Semester { get; set; } = "Spring";

        public int? SupervisorId { get; set; }
        public Supervisor? Supervisor { get; set; }

        public List<Student> TeamMembers { get; set; } = new();
        public List<Milestone> Milestones { get; set; } = new();
        public List<Evaluation> Evaluations { get; set; } = new();

        public double? FinalGrade { get; set; }

        public string? RepositoryUrl { get; set; }
        public string? DemoUrl { get; set; }
        public string? DocumentationUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? TechStack { get; set; }
        public string? ThumbnailUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DefenseDate { get; set; }
        public string? DefenseRoom { get; set; }
    }
}