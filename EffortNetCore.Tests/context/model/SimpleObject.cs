using System;

namespace EffortNetCore.Tests.context
{
    public class SimpleObject
    {
        public enum SimpleEnum { Value1, Value2 }

        public int Id { get; set; }
        public string StringProperty { get; set; }
        public double DoubleProperty { get; set; }
        public bool BoolProperty { get; set; }
        public SimpleEnum EnumProperty { get; set; }
        public Guid GuidProperty { get; set; }
        public bool? OptionalBool { get; set; }
    }
}