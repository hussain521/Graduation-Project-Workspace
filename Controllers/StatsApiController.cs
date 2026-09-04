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
    public class StatsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StatsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetDashboardStats()
        {
            var totalProjects = await _context.Projects.CountAsync();
            var defendedProjects = await _context.Projects.CountAsync(p => p.Status == ProjectStatus.Defended);
            var inProgressProjects = await _context.Projects.CountAsync(p => p.Status == ProjectStatus.InProgress || p.Status == ProjectStatus.ReadyForDefense);
            var totalStudents = await _context.Students.CountAsync();
            var totalSupervisors = await _context.Supervisors.CountAsync();

            var gradedProjects = await _context.Projects.Where(p => p.FinalGrade.HasValue).ToListAsync();
            var avgScore = gradedProjects.Any() ? Math.Round(gradedProjects.Average(p => p.FinalGrade!.Value), 1) : 0;

            var trackCounts = await _context.Projects
                .GroupBy(p => p.Track)
                .Select(g => new { Track = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            var statusCounts = await _context.Projects
                .GroupBy(p => p.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            var upcomingDefenses = await _context.Projects
                .Where(p => p.DefenseDate.HasValue && p.DefenseDate >= DateTime.UtcNow)
                .OrderBy(p => p.DefenseDate)
                .Take(5)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.DefenseDate,
                    p.DefenseRoom,
                    Supervisor = p.Supervisor != null ? p.Supervisor.FullName : "TBD"
                })
                .ToListAsync();

            return Ok(new
            {
                totalProjects,
                defendedProjects,
                inProgressProjects,
                totalStudents,
                totalSupervisors,
                avgScore,
                trackCounts,
                statusCounts,
                upcomingDefenses
            });
        }
    }
}