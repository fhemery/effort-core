using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EffortNetCore.Tests.context.model.relationships
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Office> Offices { get; set; }

        public CompanyDetails Details { get; set; }
        public int DetailsId { get; set; }

        public CompanyStatistics MainOfficeStats { get; set; }
    }
}