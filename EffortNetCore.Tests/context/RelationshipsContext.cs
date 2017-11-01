using EffortNetCore.Tests.context.model.relationships;
using Microsoft.EntityFrameworkCore;

namespace EffortNetCore.Tests.context
{
    internal class RelationshipsContext : DbContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public RelationshipsContext(DbContextOptions<RelationshipsContext> options) : base(options)
        {
        }
    }
}