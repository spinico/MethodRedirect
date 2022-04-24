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

            Assembly assembly = Assembly.GetAssembly(typeof(Scenario2));
            Type Scenario_Type = assembly.GetType("MethodRedirect.Scenario2");

            MethodInfo Scenario_InternalVirtualInstanceMethod = Scenario_Type.GetMethod("InternalVirtualInstanceMethod", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo Scenario_PrivateInstanceMethod = Scenario_Type.GetMethod("PrivateInstanceMethod", BindingFlags.Instance | BindingFlags.NonPublic);

            var token = Scenario_InternalVirtualInstanceMethod.RedirectTo(Scenario_PrivateInstanceMethod, true);

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
