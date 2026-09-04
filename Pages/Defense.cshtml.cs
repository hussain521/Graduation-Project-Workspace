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
    public class DefenseModel : PageModel
    {
        private readonly AppDbContext _context;

        public DefenseModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Project> Projects { get; set; } = new();
        public List<Evaluation> RecentEvaluations { get; set; } = new();
        public int? SelectedProjectId { get; set; }

        public async Task OnGetAsync(int? projectId)
        {
            SelectedProjectId = projectId;

            Projects = await _context.Projects
                .Include(p => p.Supervisor)
                .Include(p => p.TeamMembers)
                .Include(p => p.Evaluations)
                .OrderBy(p => p.Title)
                .ToListAsync();

            RecentEvaluations = await _context.Evaluations
                .Include(e => e.Project)
                .OrderByDescending(e => e.EvaluationDate)
                .Take(8)
                .ToListAsync();
        }
    }
}