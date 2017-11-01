using System;
using System.Collections.Generic;
using System.Text;

namespace EffortNetCore
{
    public class ReconcileStep
    {
        public object TargetObject { get; set; }
        public string TargetProperty { get; set; }
        public Type PrincipalClrType { get; set; }
        public int PrincipalId { get; set; }
    }
}