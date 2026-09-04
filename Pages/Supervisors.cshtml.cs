using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GraduationProject.Data;
using GraduationProject.Models;

namespace GraduationProject.Pages
{
    public class SupervisorsModel : PageModel
    {
        private readonly AppDbContext _context;

        public SupervisorsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Supervisor> Supervisors { get; set; } = new();

        public async Task OnGetAsync()
        {
            Supervisors = await _context.Supervisors
                .Include(s => s.SupervisedProjects)
                .OrderBy(s => s.FullName)
                .ToListAsync();
        }
    }
}