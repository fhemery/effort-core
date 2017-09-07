using System;
using System.Collections.Generic;
using System.Text;

namespace EffortNetCore
{
    public class EffortException : Exception
    {
        public EffortException(string message) : base(message)
        {
        }
    }
}