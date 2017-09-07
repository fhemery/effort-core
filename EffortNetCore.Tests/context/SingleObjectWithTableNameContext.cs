using EffortNetCore.Tests.context.model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EffortNetCore.Tests.context
{
    internal class SingleObjectWithTableNameContext : DbContext
    {
        public DbSet<ObjectWithOnlyAnIdAndTableName> ObjectsWithOnlyAnId { get; set; }

        public SingleObjectWithTableNameContext(DbContextOptions<SingleObjectWithTableNameContext> options) : base(options)
        {
        }
    }
}