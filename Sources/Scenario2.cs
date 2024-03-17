using System;
using System.Diagnostics;
using System.Reflection;

namespace MethodRedirect
{
    class Scenario2
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Redirect : MethodRedirect.Scenario2.InternalVirtualInstanceMethod()");
            Console.WriteLine("To       : MethodRedirect.Scenario2.PrivateInstanceMethod()");            

            Type Scenario_Type = typeof(Scenario2);

            var token = MethodUtil.HookMethod(
                MethodHook.From(Scenario_Type, "InternalVirtualInstanceMethod"),
                MethodHook.From(Scenario_Type, "PrivateInstanceMethod")
            );

            var scenario = (Scenario2)Activator.CreateInstance(Scenario_Type);

            string methodName = scenario.InternalVirtualInstanceMethod();

            Console.WriteLine("Call MethodRedirect.Scenario2.InternalVirtualInstanceMethod => {0}", methodName);

            Debug.Assert(methodName == "MethodRedirect.Scenario2.PrivateInstanceMethod");

            if (methodName == "MethodRedirect.Scenario2.PrivateInstanceMethod")
            {
                Console.WriteLine("\nRestore...");

                token.Restore();

                methodName = scenario.InternalVirtualInstanceMethod();

                Console.WriteLine("Call MethodRedirect.Scenario2.InternalVirtualInstanceMethod => {0}", methodName);

                Debug.Assert(methodName == "MethodRedirect.Scenario2.InternalVirtualInstanceMethod");

                if (methodName == "MethodRedirect.Scenario2.InternalVirtualInstanceMethod")
                {
                    Console.WriteLine("\nSUCCESS!");
                }
                else
                {
                    Console.WriteLine("\nRestore FAILED");
                }
            }
            else
            {
                Console.WriteLine("\nRedirection FAILED");
            }

            Console.ReadKey();
        }

        internal virtual string AnotherInternalVirtualInstanceMethod()
        {
            return "MethodRedirect.Scenario2.AnotherInternalVirtualInstanceMethod";
        }

        internal virtual string InternalVirtualInstanceMethod()
        {
            return "MethodRedirect.Scenario2.InternalVirtualInstanceMethod";
        }

        private string PrivateInstanceMethod()
        {
            return "MethodRedirect.Scenario2.PrivateInstanceMethod";
        }
    }
}
