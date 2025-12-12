using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WaterTankerManager.Models;

namespace WaterTankerManager.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<RoundEntry> RoundEntries { get; set; }
    public DbSet<ExpenseEntry> ExpenseEntries { get; set; }
}
