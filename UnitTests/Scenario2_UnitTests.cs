using Microsoft.VisualStudio.TestTools.UnitTesting;
using MethodRedirect;
using System;
using System.Reflection;

namespace Scenarios_UT
{
    [TestClass]
    public class Scenario2_UnitTests
    {
        [TestMethod]
        public void Redirect_InternalVirtualInstanceMethod_To_PrivateInstanceMethod_SameInstance()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Scenario2));
            Type Scenario_Type = assembly.GetType("MethodRedirect.Scenario2");

            MethodInfo Scenario_InternalVirtualInstanceMethod = Scenario_Type.GetMethod("InternalVirtualInstanceMethod", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo Scenario_PrivateInstanceMethod = Scenario_Type.GetMethod("PrivateInstanceMethod", BindingFlags.Instance | BindingFlags.NonPublic);

            var token = Scenario_InternalVirtualInstanceMethod.RedirectTo(Scenario_PrivateInstanceMethod);

            var scenario = (Scenario2)Activator.CreateInstance(Scenario_Type);
                       
            string methodName = scenario.InternalVirtualInstanceMethod();

            Assert.IsTrue(methodName == "MethodRedirect.Scenario2.PrivateInstanceMethod");

            token.Restore();

            methodName = scenario.InternalVirtualInstanceMethod();

            Assert.IsTrue(methodName == "MethodRedirect.Scenario2.InternalVirtualInstanceMethod");
        }
    }
}
