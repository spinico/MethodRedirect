using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MethodRedirect
{
    interface IMethodsHook
    {
        IEnumerable<MethodInfo> GetMethods();
    }
}
