using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MethodRedirect
{
    public class MethodHook : IMethodsHook
    {
        Type Type { get; set; }
        MethodInfo MethodInfo { get; set; }

        private MethodHook(Type type, string methodName, bool isStatic = false)
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

            MethodInfo = type.GetMethod(methodName, flags);
        }

        private MethodHook()
        {
        }

        static public MethodHook From<T>(Func<T> target)
        {
            return new MethodHook() { MethodInfo = target.Method };
        }
        static public MethodHook From<T, R>(Func<T, R> target)
        {
            return new MethodHook() { MethodInfo = target.Method };
        }

        static public MethodHook From(Type type, string methodName, bool isStatic = false)
        {
            return new MethodHook(type, methodName, isStatic);
        }

        public IEnumerable<MethodInfo> GetMethods()
        {
            yield return MethodInfo;
        }
    }
}
