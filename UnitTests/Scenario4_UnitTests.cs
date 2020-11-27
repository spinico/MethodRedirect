using Microsoft.VisualStudio.TestTools.UnitTesting;
using MethodRedirect;
using System;
using System.Reflection;

namespace Scenarios_UT
{
    [TestClass]
    public class Scenario4_UnitTests
    {
        [TestMethod]
        public void Redirect_PublicVirtualInstanceMethod_To_PrivateInstanceMethod_DifferentInstance()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Scenario4));
            Type Scenario_Type = assembly.GetType("MethodRedirect.Scenario4");
            Type ScenarioExt_Type = assembly.GetType("MethodRedirect.Scenario4Ext");

            MethodInfo Scenario_PublicVirtualInstanceMethod = Scenario_Type.GetMethod("PublicVirtualInstanceMethod", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo ScenarioExt_PrivateInstanceMethod = ScenarioExt_Type.GetMethod("PrivateInstanceMethod", BindingFlags.Instance | BindingFlags.NonPublic);

            var token = Scenario_PublicVirtualInstanceMethod.RedirectTo(ScenarioExt_PrivateInstanceMethod);

            var scenario = (Scenario4)Activator.CreateInstance(Scenario_Type);
                       
            string methodName = scenario.PublicVirtualInstanceMethod();

            Assert.IsTrue(methodName == "MethodRedirect.Scenario4Ext.PrivateInstanceMethod");

            token.Restore();

            methodName = scenario.PublicVirtualInstanceMethod();

            Assert.IsTrue(methodName == "MethodRedirect.Scenario4.PublicVirtualInstanceMethod");
        }
    }
}
