using System;
using System.Collections.Generic;
using System.Text;

namespace MikoVm
{
    class CompilerError : Exception
    {
        public CompilerError(string message) : base(message)
        {
        }
    }
}
