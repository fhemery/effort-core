using System;
using System.Collections.Generic;
using System.Text;

namespace EffortNetCore.Tests.context.model.relationships
{
    public class Office
    {
        public int Id { get; set; }
        public string Address { get; set; }

        public int CompanyId { get; set; }
    }
}