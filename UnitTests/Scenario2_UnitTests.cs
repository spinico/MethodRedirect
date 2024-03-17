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
            Type Scenario_Type = typeof(Scenario2);

            var token = MethodUtil.HookMethod(
                MethodHook.From(Scenario_Type, "InternalVirtualInstanceMethod"),
                MethodHook.From(Scenario_Type, "PrivateInstanceMethod")
            );

            var scenario = (Scenario2)Activator.CreateInstance(Scenario_Type);
                       
            string methodName = scenario.InternalVirtualInstanceMethod();

            Assert.IsTrue(methodName == "MethodRedirect.Scenario2.PrivateInstanceMethod");

            token.Restore();

            methodName = scenario.InternalVirtualInstanceMethod();

            Assert.IsTrue(methodName == "MethodRedirect.Scenario2.InternalVirtualInstanceMethod");
        }
    }
}
