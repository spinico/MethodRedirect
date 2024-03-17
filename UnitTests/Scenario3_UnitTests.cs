using Microsoft.VisualStudio.TestTools.UnitTesting;
using MethodRedirect;
using System;
using System.Reflection;

namespace Scenarios_UT
{
    [TestClass]
    public class Scenario3_UnitTests
    {
        [TestMethod]
        public void Redirect_InternalVirtualInstanceMethod_To_InternalStaticMethod_DifferentInstance()
        {
            Type Scenario_Type = typeof(Scenario3);

            var token = MethodUtil.HookMethod(
                MethodHook.From(Scenario_Type, "InternalVirtualInstanceMethod"),
                MethodHook.From(typeof(Scenario3Ext), "InternalStaticMethod", true)
            );

            var scenario = (Scenario3)Activator.CreateInstance(Scenario_Type);
                       
            string methodName = scenario.InternalVirtualInstanceMethod();

            Assert.IsTrue(methodName == "MethodRedirect.Scenario3Ext.InternalStaticMethod");

            token.Restore();

            methodName = scenario.InternalVirtualInstanceMethod();

            Assert.IsTrue(methodName == "MethodRedirect.Scenario3.InternalVirtualInstanceMethod");
        }
    }
}
