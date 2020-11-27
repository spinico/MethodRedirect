using System;
using System.Diagnostics;
using System.Reflection;

namespace MethodRedirect
{
    class Scenario1
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Redirect : MethodRedirect.Scenario1.InternalInstanceMethod()");
            Console.WriteLine("To       : MethodRedirect.Scenario1.PrivateInstanceMethod()");

            Assembly assembly = Assembly.GetAssembly(typeof(Scenario1));
            Type Scenario_Type = assembly.GetType("MethodRedirect.Scenario1");

            MethodInfo Scenario_InternalInstanceMethod = Scenario_Type.GetMethod("InternalInstanceMethod", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo Scenario_PrivateInstanceMethod = Scenario_Type.GetMethod("PrivateInstanceMethod", BindingFlags.Instance | BindingFlags.NonPublic);

            var token = Scenario_InternalInstanceMethod.RedirectTo(Scenario_PrivateInstanceMethod);

            // Using "dynamic" type to resolve the following issue in x64 and Release (with code optimizations) builds.
            //
            // Symptom: the second call to method InternalInstanceMethod() does not return the expected string value
            //
            // Cause: the result from the first call to method InternalInstanceMethod() is cached and so the second
            // call gets the cached value instead of making the call again.
            //
            // Remark: for some reason, without "dynamic" type, only the "Debug x86" build configuration would reevaluate 
            // the second call to InternalInstanceMethod() without using the cached string value.
            //
            // Also, using the [MethodImpl(MethodImplOptions.NoOptimization)] attribute on the method did not work.
            dynamic scenario = (Scenario1)Activator.CreateInstance(Scenario_Type);

            string methodName = scenario.InternalInstanceMethod();

            Console.WriteLine("Call MethodRedirect.Scenario1.InternalInstanceMethod => {0}", methodName);

            Debug.Assert(methodName == "MethodRedirect.Scenario1.PrivateInstanceMethod");

            if (methodName == "MethodRedirect.Scenario1.PrivateInstanceMethod")
            {
                Console.WriteLine("\nRestore...");
                
                token.Restore();

                Console.WriteLine(token);

                methodName = scenario.InternalInstanceMethod();

                Console.WriteLine("Call MethodRedirect.Scenario1.InternalInstanceMethod => {0}", methodName);

                Debug.Assert(methodName == "MethodRedirect.Scenario1.InternalInstanceMethod");

                if (methodName == "MethodRedirect.Scenario1.InternalInstanceMethod")
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
        
        internal string InternalInstanceMethod()
        {
            return "MethodRedirect.Scenario1.InternalInstanceMethod";
        }
        
        private string PrivateInstanceMethod()
        {
            return "MethodRedirect.Scenario1.PrivateInstanceMethod";
        }

        public string PublicInstanceMethod()
        {
            return "MethodRedirect.Scenario1.PublicInstanceMethod";
        }

        public static string PublicStaticMethod()
        {
            return "MethodRedirect.Scenario1.PublicStaticMethod";
        }
    }
}
