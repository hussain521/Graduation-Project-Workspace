using System;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Models
{
    public class Evaluation
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        public int? SupervisorId { get; set; }
        public Supervisor? Supervisor { get; set; }

        [Required]
        [StringLength(100)]
        public string EvaluatorName { get; set; } = string.Empty;

        public string EvaluatorRole { get; set; } = "Internal Examiner"; // Supervisor, Internal Examiner, External Examiner, Committee Chair

        [Range(0, 20)]
        public double PresentationScore { get; set; } // Max 20

        [Range(0, 40)]
        public double ImplementationScore { get; set; } // Max 40

        [Range(0, 20)]
        public double DocumentationScore { get; set; } // Max 20

        [Range(0, 20)]
        public double InnovationScore { get; set; } // Max 20

        public double TotalScore => PresentationScore + ImplementationScore + DocumentationScore + InnovationScore;

        public string Comments { get; set; } = string.Empty;
        public bool IsPassed => TotalScore >= 60;
        public DateTime EvaluationDate { get; set; } = DateTime.UtcNow;
    }
}