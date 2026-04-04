using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class IdentityEfDbContext:IdentityDbContext
{
    public IdentityEfDbContext(DbContextOptions<IdentityEfDbContext> options) : base(options)
    {
    }
    
}