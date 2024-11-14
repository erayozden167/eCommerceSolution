using AuthenticationApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationApi.Infrastructure.Data
{
    public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : DbContext(options)
    {
        public DbSet<AppUserEntity> Users { get; set; }
    }
}
