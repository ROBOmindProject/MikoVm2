using System;
using System.Collections.Generic;
using System.Text;

namespace MikoVm
{
    /**
     * 文法エラー。
     */
    class SyntaxError : Exception
    {
        public SyntaxError(string message) : base(message)
        {
        }
    }
}
