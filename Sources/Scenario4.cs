using System;

namespace MethodRedirect
{
    class Scenario4
    {
        static void Main(string[] args)
        {
            Console.ReadKey();
        }

        public virtual string PublicVirtualInstanceMethod()
        {
            return "MethodRedirect.Scenario4.PublicVirtualInstanceMethod";
        }
    }

    class Scenario4Ext
    {
        private string PrivateInstanceMethod()
        {
            return "MethodRedirect.Scenario4Ext.PrivateInstanceMethod";
        }
    }
}
