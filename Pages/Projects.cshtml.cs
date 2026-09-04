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
    public class ProjectsModel : PageModel
    {
        private readonly AppDbContext _context;

        public ProjectsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Project> Projects { get; set; } = new();
        public List<Supervisor> Supervisors { get; set; } = new();

        public string? SearchTerm { get; set; }
        public ProjectTrack? SelectedTrack { get; set; }
        public ProjectStatus? SelectedStatus { get; set; }
        public int? SelectedProjectId { get; set; }
        public Project? ModalProject { get; set; }

        public async Task OnGetAsync(string? search, ProjectTrack? track, ProjectStatus? status, int? id)
        {
            SearchTerm = search;
            SelectedTrack = track;
            SelectedStatus = status;
            SelectedProjectId = id;

            Supervisors = await _context.Supervisors.OrderBy(s => s.FullName).ToListAsync();

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
                    (p.TechStack != null && p.TechStack.ToLower().Contains(s)));
            }

            if (track.HasValue)
            {
                query = query.Where(p => p.Track == track.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            Projects = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();

            if (id.HasValue)
            {
                ModalProject = await _context.Projects
                    .Include(p => p.Supervisor)
                    .Include(p => p.TeamMembers)
                    .Include(p => p.Milestones)
                    .Include(p => p.Evaluations)
                    .FirstOrDefaultAsync(p => p.Id == id.Value);
            }
        }
    }
}