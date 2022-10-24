using System.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MSGMicroservice.IDP.Infrastructure.Entities;

namespace MSGMicroservice.IDP.Persistence
{
    public class MsgIdentityContext : IdentityDbContext<User>
    {
        public IDbConnection Connection => Database.GetDbConnection();

        public MsgIdentityContext(DbContextOptions<MsgIdentityContext> options) : base(options)
        {

        }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // builder.ApplyConfiguration(new RoleConfiguration());//cach 2 ben duoi
            builder.ApplyConfigurationsFromAssembly(typeof(MsgIdentityContext).Assembly); //cach 2 hay hon
            // base.OnModelCreating(builder);
            builder.ApplyIdentityConfiguration();
        }
    }
}