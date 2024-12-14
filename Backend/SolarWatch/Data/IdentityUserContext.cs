using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SolarWatch.Data;

public class IdentityUserContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public IdentityUserContext(DbContextOptions<IdentityUserContext> options) : base(options)
    {
    }
}