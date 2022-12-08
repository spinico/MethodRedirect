using System;

namespace MethodRedirect
{
    class Scenario6
    {
        static void Main(string[] args)
        {
            Console.ReadKey();
        }

        public virtual string PublicVirtualInstanceMethod()
        {
            return "MethodRedirect.Scenario6.PublicVirtualInstanceMethod";
        }

        public virtual string PublicVirtualInstanceMethodWithParameter(int x)
        {
            return "MethodRedirect.Scenario6.PublicVirtualInstanceMethodWithParameter." + x;
        }

        public virtual int AnotherPublicInstanceMethodWithParameter(int x)
        {
            return x + 1;
        }
    }

    class Scenario6Ext
    {
        private string PrivateInstanceMethodWithParameter(int x)
        {
            return "MethodRedirect.Scenario6Ext.PrivateInstanceMethodWithParameter." + x;
        }
    }
}