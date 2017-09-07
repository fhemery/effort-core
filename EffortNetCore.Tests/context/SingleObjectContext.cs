using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EffortNetCore.Tests.context
{
    internal class SingleObjectContext : DbContext
    {
        public DbSet<SimpleObject> SimpleObjects { get; set; }

        public SingleObjectContext(DbContextOptions<SingleObjectContext> options) : base(options)
        {
        }
    }
}