using System;
using System.Collections.Generic;
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
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetProjects(
            [FromQuery] string? search,
            [FromQuery] ProjectTrack? track,
            [FromQuery] ProjectStatus? status,
            [FromQuery] string? sortBy)
        {
            var query = _context.Projects
                .Include(p => p.Supervisor)
                .Include(p => p.TeamMembers)
                .Include(p => p.Milestones)
                .Include(p => p.Evaluations)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(p => 
                    p.Title.ToLower().Contains(s) || 
                    p.Abstract.ToLower().Contains(s) ||
                    (p.TechStack != null && p.TechStack.ToLower().Contains(s)) ||
                    p.TeamMembers.Any(m => m.FullName.ToLower().Contains(s)) ||
                    (p.Supervisor != null && p.Supervisor.FullName.ToLower().Contains(s)));
            }

            if (track.HasValue)
            {
                query = query.Where(p => p.Track == track.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            // Sorting
            query = sortBy?.ToLower() switch
            {
                "rating" => query.OrderByDescending(p => p.FinalGrade ?? 0),
                "title" => query.OrderBy(p => p.Title),
                "date_asc" => query.OrderBy(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var projects = await query.ToListAsync();

            var result = projects.Select(p => new
            {
                p.Id,
                p.Title,
                p.Abstract,
                p.Description,
                Track = p.Track.ToString(),
                TrackId = (int)p.Track,
                Status = p.Status.ToString(),
                StatusId = (int)p.Status,
                p.AcademicYear,
                p.Semester,
                Supervisor = p.Supervisor != null ? new
                {
                    p.Supervisor.Id,
                    p.Supervisor.FullName,
                    p.Supervisor.Title,
                    p.Supervisor.Department,
                    p.Supervisor.Email,
                    p.Supervisor.AvatarUrl
                } : null,
                TeamMembers = p.TeamMembers.Select(m => new
                {
                    m.Id,
                    m.FullName,
                    m.StudentId,
                    m.Role,
                    m.Email,
                    m.AvatarUrl
                }),
                Milestones = p.Milestones.Select(m => new
                {
                    m.Id,
                    m.Title,
                    m.Status,
                    StatusName = m.Status.ToString(),
                    m.DueDate,
                    m.WeightPercentage
                }),
                CompletedMilestonesCount = p.Milestones.Count(m => m.Status == MilestoneStatus.Approved),
                TotalMilestonesCount = p.Milestones.Count,
                p.FinalGrade,
                p.RepositoryUrl,
                p.DemoUrl,
                p.DocumentationUrl,
                p.VideoUrl,
                p.TechStack,
                p.ThumbnailUrl,
                p.CreatedAt,
                p.DefenseDate,
                p.DefenseRoom,
                EvaluationsCount = p.Evaluations.Count,
                AverageEvaluationScore = p.Evaluations.Any() ? Math.Round(p.Evaluations.Average(e => e.TotalScore), 1) : (double?)null
            });

            return Ok(result);
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetProject(int id)
        {
            var p = await _context.Projects
                .Include(x => x.Supervisor)
                .Include(x => x.TeamMembers)
                .Include(x => x.Milestones)
                .Include(x => x.Evaluations)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null)
            {
                return NotFound(new { message = $"Project #{id} was not found." });
            }

            return Ok(new
            {
                p.Id,
                p.Title,
                p.Abstract,
                p.Description,
                Track = p.Track.ToString(),
                TrackId = (int)p.Track,
                Status = p.Status.ToString(),
                StatusId = (int)p.Status,
                p.AcademicYear,
                p.Semester,
                Supervisor = p.Supervisor != null ? new
                {
                    p.Supervisor.Id,
                    p.Supervisor.FullName,
                    p.Supervisor.Title,
                    p.Supervisor.Department,
                    p.Supervisor.Email,
                    p.Supervisor.OfficeLocation,
                    p.Supervisor.AvatarUrl,
                    p.Supervisor.Phone
                } : null,
                TeamMembers = p.TeamMembers.Select(m => new
                {
                    m.Id,
                    m.FullName,
                    m.StudentId,
                    m.Role,
                    m.Email,
                    m.Major,
                    m.GPA,
                    m.AvatarUrl
                }),
                Milestones = p.Milestones.OrderBy(m => m.DueDate).Select(m => new
                {
                    m.Id,
                    m.Title,
                    m.Description,
                    Status = m.Status.ToString(),
                    StatusId = (int)m.Status,
                    m.DueDate,
                    m.CompletedDate,
                    m.WeightPercentage,
                    m.Feedback,
                    m.DeliverableUrl
                }),
                Evaluations = p.Evaluations.Select(e => new
                {
                    e.Id,
                    e.EvaluatorName,
                    e.EvaluatorRole,
                    e.PresentationScore,
                    e.ImplementationScore,
                    e.DocumentationScore,
                    e.InnovationScore,
                    e.TotalScore,
                    e.Comments,
                    e.IsPassed,
                    e.EvaluationDate
                }),
                p.FinalGrade,
                p.RepositoryUrl,
                p.DemoUrl,
                p.DocumentationUrl,
                p.VideoUrl,
                p.TechStack,
                p.ThumbnailUrl,
                p.CreatedAt,
                p.DefenseDate,
                p.DefenseRoom
            });
        }

        public class CreateProjectDto
        {
            [Required]
            public string Title { get; set; } = string.Empty;
            [Required]
            public string Abstract { get; set; } = string.Empty;
            public string? Description { get; set; }
            public ProjectTrack Track { get; set; }
            public int? SupervisorId { get; set; }
            public string? TechStack { get; set; }
            public string? RepositoryUrl { get; set; }
            public string? DemoUrl { get; set; }
            public string? DocumentationUrl { get; set; }
            public string? ThumbnailUrl { get; set; }
            public List<CreateStudentDto> TeamMembers { get; set; } = new();
        }

        public class CreateStudentDto
        {
            public string FullName { get; set; } = string.Empty;
            public string StudentId { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Role { get; set; } = "Team Lead";
            public string Major { get; set; } = "Software Engineering";
        }

        // POST: api/Projects
        [HttpPost]
        public async Task<ActionResult<object>> CreateProject([FromBody] CreateProjectDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = new Project
            {
                Title = dto.Title,
                Abstract = dto.Abstract,
                Description = dto.Description ?? dto.Abstract,
                Track = dto.Track,
                Status = ProjectStatus.Proposed,
                SupervisorId = dto.SupervisorId,
                TechStack = dto.TechStack,
                RepositoryUrl = dto.RepositoryUrl,
                DemoUrl = dto.DemoUrl,
                DocumentationUrl = dto.DocumentationUrl,
                ThumbnailUrl = string.IsNullOrWhiteSpace(dto.ThumbnailUrl) 
                    ? "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=600&auto=format&fit=crop&q=80" 
                    : dto.ThumbnailUrl,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var s in dto.TeamMembers)
            {
                if (!string.IsNullOrWhiteSpace(s.FullName))
                {
                    project.TeamMembers.Add(new Student
                    {
                        FullName = s.FullName,
                        StudentId = string.IsNullOrWhiteSpace(s.StudentId) ? $"ST-{DateTime.UtcNow.Year}-{new Random().Next(1000, 9999)}" : s.StudentId,
                        Email = string.IsNullOrWhiteSpace(s.Email) ? $"{s.FullName.ToLower().Replace(" ", ".")}@university.edu" : s.Email,
                        Role = string.IsNullOrWhiteSpace(s.Role) ? "Team Member" : s.Role,
                        Major = string.IsNullOrWhiteSpace(s.Major) ? "Computer Science" : s.Major
                    });
                }
            }

            // Add standard default milestones
            project.Milestones.Add(new Milestone { Title = "Project Proposal & Requirements", DueDate = DateTime.UtcNow.AddDays(14), WeightPercentage = 15, Status = MilestoneStatus.Pending });
            project.Milestones.Add(new Milestone { Title = "Architecture & Prototype", DueDate = DateTime.UtcNow.AddDays(45), WeightPercentage = 25, Status = MilestoneStatus.Pending });
            project.Milestones.Add(new Milestone { Title = "Full Implementation & Testing", DueDate = DateTime.UtcNow.AddDays(90), WeightPercentage = 40, Status = MilestoneStatus.Pending });
            project.Milestones.Add(new Milestone { Title = "Final Defense & Report Submission", DueDate = DateTime.UtcNow.AddDays(120), WeightPercentage = 20, Status = MilestoneStatus.Pending });

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, new { id = project.Id, message = "Project submitted successfully!" });
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Project #{id} removed." });
        }
    }
}