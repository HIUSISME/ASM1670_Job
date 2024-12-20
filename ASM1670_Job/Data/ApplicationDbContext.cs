using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ASM1670_Job.Models;

namespace ASM1670_Job.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ASM1670_Job.Models.Seeker> Seeker { get; set; } = default!;
        public DbSet<ASM1670_Job.Models.Employer> Employer { get; set; } = default!;
        public DbSet<ASM1670_Job.Models.Job> Job { get; set; } = default!;
        public DbSet<ASM1670_Job.Models.Application> Application { get; set; } = default!;
    }
}