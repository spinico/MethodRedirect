using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MethodRedirect
{
    public class PropertyHook : IMethodsHook
    {
        Type Type { get; set; }
        MethodInfo GetMethodInfo { get; set; }
        MethodInfo SetMethodInfo { get; set; }

        public PropertyHook(Type type, string propertyName, bool isStatic = false)
        {
            Type = type;
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public;
            if (isStatic)
            {
                flags |= BindingFlags.Static;
            }
            else
            {
                flags |= BindingFlags.Instance;
            }

            var propInfo = type.GetProperty(propertyName, flags);
            GetMethodInfo = propInfo.GetGetMethod(true);
            SetMethodInfo = propInfo.GetSetMethod(true);
        }

        public IEnumerable<MethodInfo> GetMethods()
        {
            yield return GetMethodInfo;
            yield return SetMethodInfo;
        }
    }
}
