using System;

namespace MethodRedirect
{
    class Scenario3
    {
        static void Main(string[] args)
        {
            Console.ReadKey();
        }

        internal virtual string InternalVirtualInstanceMethod()
        {
            return "MethodRedirect.Scenario3.InternalVirtualInstanceMethod";
        }
    }

    class Scenario3Ext
    {
        internal static string InternalStaticMethod()
        {
            return "MethodRedirect.Scenario3Ext.InternalStaticMethod";
        }
    }
}
