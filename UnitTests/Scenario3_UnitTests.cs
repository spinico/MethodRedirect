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
            Assembly assembly = Assembly.GetAssembly(typeof(Scenario3));
            Type Scenario_Type = assembly.GetType("MethodRedirect.Scenario3");
            Type ScenarioExt_Type = assembly.GetType("MethodRedirect.Scenario3Ext");

            MethodInfo Scenario_InternalVirtualInstanceMethod = Scenario_Type.GetMethod("InternalVirtualInstanceMethod", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo ScenarioExt_InternalStaticMethod = ScenarioExt_Type.GetMethod("InternalStaticMethod", BindingFlags.Static | BindingFlags.NonPublic);

            var token = Scenario_InternalVirtualInstanceMethod.RedirectTo(ScenarioExt_InternalStaticMethod);

            var scenario = (Scenario3)Activator.CreateInstance(Scenario_Type);
                       
            string methodName = scenario.InternalVirtualInstanceMethod();

            Assert.IsTrue(methodName == "MethodRedirect.Scenario3Ext.InternalStaticMethod");

            token.Restore();

            methodName = scenario.InternalVirtualInstanceMethod();

            Assert.IsTrue(methodName == "MethodRedirect.Scenario3.InternalVirtualInstanceMethod");
        }
    }
}
