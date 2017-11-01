using System;
using System.Collections.Generic;
using System.Text;

namespace EffortNetCore.Tests.context.model.relationships
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int OfficeId { get; set; }
        public Company Office { get; set; }
    }
}