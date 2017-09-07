using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EffortNetCore.Tests.context
{
    internal class EmptyContext : DbContext
    {
        public EmptyContext(DbContextOptions<EmptyContext> options) : base(options)
        {
        }
    }
}