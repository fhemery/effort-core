using EffortNetCore.Tests.context;
using EffortNetCore.Tests.context.model.relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EffortNetCore.Tests
{
    [TestClass]
    public class RelationshipTests
    {
        private RelationshipsContext context;

        [TestInitialize]
        public void SetUp()
        {
            context = new EffortContext<RelationshipsContext>("data/relationships").GetContext();
        }

        [TestMethod]
        public void EffortContext_ShouldReturnTheOfficeDetails_WhichHasIdReference()
        {
            Assert.IsNotNull(context.Find(typeof(CompanyDetails), 1));
            Assert.IsNotNull(context.Companies.Find(12).Details);
            Assert.AreEqual(1, context.Companies.Find(12).Details.Id);
        }

        [TestMethod]
        public void EffortContext_ShouldReturnTheOfficeStatistics_WhichIsOnlyNavigationProperty()
        {
            Assert.IsNotNull(context.Companies.Find(12).MainOfficeStats);
            Assert.AreEqual(1, context.Companies.Find(12).MainOfficeStats.Id);
        }

        [TestMethod]
        public void EffortContext_ShouldReturnTheOfficeEmployes_WhichHasIdReference()
        {
            Assert.AreEqual(3, context.Employees.Count());
            Assert.AreEqual(12, context.Employees.Find(1001).OfficeId);

            Assert.AreEqual(12, context.Employees.Find(1001).Office.Id);
            Assert.IsNotNull(context.Companies.Find(14));
            Assert.AreEqual(2, context.Companies.Find(14).Employees.Count());
        }

        // Currently ignored as it was just a scratchpad test to prove constraint integrity is not checked (#2166 of EntityframeworkCore github)
        public void Scratchpad_Test()
        {
            var options = new DbContextOptionsBuilder<RelationshipsContext>().UseInMemoryDatabase().Options;
            var newContext = new RelationshipsContext(options);
            newContext.Add(new Employee { Id = 1, Name = "Adam", OfficeId = 14 });
            newContext.Add(new Company { Id = 1, Name = "Earth Inc." });
            newContext.SaveChanges();

            Assert.IsNotNull(newContext.Employees.Find(1).Office);
            Assert.AreEqual(1, newContext.Employees.Find(1).Office.Id);
        }
    }
}