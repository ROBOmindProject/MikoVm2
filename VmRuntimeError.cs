using System;
using System.Collections.Generic;
using System.Text;

namespace MikoVm
{
    /**
     * VM実行時エラー
     */
    class VmRuntimeError : Exception
    {
        public VmRuntimeError(string message) : base(message)
        {
        }
    }
}
