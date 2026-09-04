using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GraduationProject.Data;
using GraduationProject.Models;

namespace GraduationProject.Pages
{
    public class KanbanModel : PageModel
    {
        private readonly AppDbContext _context;

        public KanbanModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Milestone> PendingMilestones { get; set; } = new();
        public List<Milestone> InProgressMilestones { get; set; } = new();
        public List<Milestone> ApprovedMilestones { get; set; } = new();

        public async Task OnGetAsync()
        {
            var all = await _context.Milestones
                .Include(m => m.Project)
                .ThenInclude(p => p!.Supervisor)
                .OrderBy(m => m.DueDate)
                .ToListAsync();

            PendingMilestones = all.Where(m => m.Status == MilestoneStatus.Pending).ToList();
            InProgressMilestones = all.Where(m => m.Status == MilestoneStatus.InProgress || m.Status == MilestoneStatus.Submitted).ToList();
            ApprovedMilestones = all.Where(m => m.Status == MilestoneStatus.Approved).ToList();
        }
    }
}