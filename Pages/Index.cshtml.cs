using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GraduationProject.Data;
using GraduationProject.Models;

namespace GraduationProject.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public int TotalProjects { get; set; }
        public int DefendedProjects { get; set; }
        public int InProgressProjects { get; set; }
        public int TotalStudents { get; set; }
        public int TotalSupervisors { get; set; }
        public double AverageScore { get; set; }

        public List<Project> FeaturedProjects { get; set; } = new();
        public List<Announcement> RecentAnnouncements { get; set; } = new();
        public List<Project> UpcomingDefenses { get; set; } = new();

        public async Task OnGetAsync()
        {
            TotalProjects = await _context.Projects.CountAsync();
            DefendedProjects = await _context.Projects.CountAsync(p => p.Status == ProjectStatus.Defended);
            InProgressProjects = await _context.Projects.CountAsync(p => p.Status == ProjectStatus.InProgress || p.Status == ProjectStatus.ReadyForDefense);
            TotalStudents = await _context.Students.CountAsync();
            TotalSupervisors = await _context.Supervisors.CountAsync();

            var graded = await _context.Projects.Where(p => p.FinalGrade.HasValue).ToListAsync();
            AverageScore = graded.Any() ? Math.Round(graded.Average(p => p.FinalGrade!.Value), 1) : 0;

            FeaturedProjects = await _context.Projects
                .Include(p => p.Supervisor)
                .Include(p => p.TeamMembers)
                .Include(p => p.Milestones)
                .OrderByDescending(p => p.FinalGrade ?? 0)
                .Take(4)
                .ToListAsync();

            RecentAnnouncements = await _context.Announcements
                .OrderByDescending(a => a.PublishedDate)
                .Take(3)
                .ToListAsync();

            UpcomingDefenses = await _context.Projects
                .Include(p => p.Supervisor)
                .Where(p => p.DefenseDate.HasValue && p.DefenseDate >= DateTime.UtcNow)
                .OrderBy(p => p.DefenseDate)
                .Take(4)
                .ToListAsync();
        }
    }
}