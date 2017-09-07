using EffortNetCore.Tests.context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EffortNetCore.Tests
{
    [TestClass]
    public class ScalarMemberTests
    {
        private List<SimpleObject> simpleObjectsFromContext;

        [TestInitialize]
        public void SetUp()
        {
            var effortContext = new EffortContext<SingleObjectContext>("data/simpleObject");
            simpleObjectsFromContext = effortContext.GetContext().SimpleObjects.ToList();
        }

        [TestMethod]
        public void EffortContext_ShouldReturnRightNumberOfLines()
        {
            Assert.AreEqual(2, simpleObjectsFromContext.Count());
        }

        [TestMethod]
        public void EffortContext_ShouldParseCorrectlyIntegers()
        {
            Assert.AreEqual(12, simpleObjectsFromContext.First().Id);
        }

        [TestMethod]
        public void EffortContext_ShouldParseCorrectlyStrings()
        {
            Assert.AreEqual("Hello", simpleObjectsFromContext.First().StringProperty);
        }

        [TestMethod]
        public void EffortContext_ShouldParseCorrectlyDoubles()
        {
            Assert.AreEqual(3.1415, simpleObjectsFromContext.First().DoubleProperty, 0.00001);
        }

        [TestMethod]
        public void EffortContext_ShouldParseCorrectlyBoolean()
        {
            Assert.IsTrue(simpleObjectsFromContext.First().BoolProperty);
            Assert.IsTrue(simpleObjectsFromContext.Last().BoolProperty);
        }

        [TestMethod]
        public void EffortContext_ShouldParseCorrectlyEnums()
        {
            Assert.AreEqual(SimpleObject.SimpleEnum.Value2, simpleObjectsFromContext.First().EnumProperty);
        }

        [TestMethod]
        public void EffortContext_ShouldParseCorrectlyGuids()
        {
            Assert.AreEqual("06711111-0821-4caa-ae4e-681622684691", simpleObjectsFromContext.First().GuidProperty.ToString());
        }

        [TestMethod]
        public void EffortContext_ShouldParseCorrectlyOptionals()
        {
            Assert.IsTrue(simpleObjectsFromContext.First().OptionalBool.GetValueOrDefault());
            Assert.IsFalse(simpleObjectsFromContext.Last().OptionalBool.GetValueOrDefault());
        }
    }
}