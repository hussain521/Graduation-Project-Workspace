using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GraduationProject.Data;
using GraduationProject.Models;

namespace GraduationProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EvaluationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EvaluationsController(AppDbContext context)
        {
            _context = context;
        }

        public class SubmitEvaluationDto
        {
            public int ProjectId { get; set; }
            public int? SupervisorId { get; set; }
            public string EvaluatorName { get; set; } = string.Empty;
            public string EvaluatorRole { get; set; } = "Committee Examiner";
            public double PresentationScore { get; set; } // max 20
            public double ImplementationScore { get; set; } // max 40
            public double DocumentationScore { get; set; } // max 20
            public double InnovationScore { get; set; } // max 20
            public string Comments { get; set; } = string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitEvaluation([FromBody] SubmitEvaluationDto dto)
        {
            var project = await _context.Projects
                .Include(p => p.Evaluations)
                .FirstOrDefaultAsync(p => p.Id == dto.ProjectId);

            if (project == null)
            {
                return NotFound(new { message = $"Project #{dto.ProjectId} not found." });
            }

            var eval = new Evaluation
            {
                ProjectId = dto.ProjectId,
                SupervisorId = dto.SupervisorId,
                EvaluatorName = string.IsNullOrWhiteSpace(dto.EvaluatorName) ? "Committee Examiner" : dto.EvaluatorName,
                EvaluatorRole = dto.EvaluatorRole,
                PresentationScore = Math.Clamp(dto.PresentationScore, 0, 20),
                ImplementationScore = Math.Clamp(dto.ImplementationScore, 0, 40),
                DocumentationScore = Math.Clamp(dto.DocumentationScore, 0, 20),
                InnovationScore = Math.Clamp(dto.InnovationScore, 0, 20),
                Comments = dto.Comments ?? string.Empty,
                EvaluationDate = DateTime.UtcNow
            };

            _context.Evaluations.Add(eval);
            await _context.SaveChangesAsync();

            // Recompute project final grade
            var allEvals = await _context.Evaluations.Where(e => e.ProjectId == dto.ProjectId).ToListAsync();
            project.FinalGrade = Math.Round(allEvals.Average(e => e.TotalScore), 1);
            if (project.Status != ProjectStatus.Defended)
            {
                project.Status = ProjectStatus.Defended;
            }
            await _context.SaveChangesAsync();

            return Ok(new
            {
                evaluationId = eval.Id,
                totalScore = eval.TotalScore,
                isPassed = eval.IsPassed,
                projectFinalGrade = project.FinalGrade,
                message = "Defense evaluation recorded successfully!"
            });
        }
    }
}