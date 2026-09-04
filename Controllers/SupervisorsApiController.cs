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
    public class SupervisorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SupervisorsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetSupervisors()
        {
            var supervisors = await _context.Supervisors
                .Include(s => s.SupervisedProjects)
                .ToListAsync();

            var result = supervisors.Select(s => new
            {
                s.Id,
                s.FullName,
                s.Title,
                DisplayName = $"{s.Title} {s.FullName}",
                s.Email,
                s.Department,
                s.ResearchInterests,
                s.OfficeLocation,
                s.MaxProjectsQuota,
                CurrentProjectsCount = s.SupervisedProjects.Count,
                AvailableCapacity = Math.Max(0, s.MaxProjectsQuota - s.SupervisedProjects.Count),
                s.AvatarUrl,
                s.Phone,
                SupervisedProjects = s.SupervisedProjects.Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Status,
                    p.Track
                })
            });

            return Ok(result);
        }
    }
}