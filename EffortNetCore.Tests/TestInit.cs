using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EffortNetCore.Tests.context;
using EffortNetCore;
using System.Linq;

namespace EffortNetCore.Tests
{
    [TestClass]
    public class TestInit
    {
        [TestMethod]
        public void ShouldInitializeContextProperly()
        {
            var context = new EffortContext<EmptyContext>("data/emptydb");
        }

        [TestMethod]
        [ExpectedException(typeof(EffortException))]
        public void ShouldThrowAnErrorIfContextDoesNotHaveParametrizedConstructor()
        {
            new EffortContext<ContextWithBadConstructor>("data/emptydb");
        }

        [TestMethod]
        [ExpectedException(typeof(EffortException))]
        public void ShouldThrowAnErrorIfAnExcelFileDoesNotMatchADbSet()
        {
            new EffortContext<SingleObjectContext>("data/singleObject_WrongFileName");
        }

        [TestMethod]
        public void ShouldMapCorrectlyTheObjectIfTableNameHasBeenAnnotated()
        {
            var effortContext = new EffortContext<SingleObjectWithTableNameContext>("data/singleObject_WithTableAnnotation");
            Assert.AreEqual(12, effortContext.GetContext().ObjectsWithOnlyAnId.First().Id);
        }
    }
}