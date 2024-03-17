using Microsoft.VisualStudio.TestTools.UnitTesting;
using MethodRedirect;
using System;
using System.Reflection;

namespace Scenarios_UT
{
    [TestClass]
    public class Scenario1_UnitTests
    {
        [TestMethod]
        public void Redirect_InternalInstanceMethod_To_PrivateInstanceMethod_SameInstance()
        {
            Type Scenario_Type = typeof(Scenario1);

            var token = MethodUtil.HookMethod(
                MethodHook.From(Scenario_Type, "InternalInstanceMethod"),
                MethodHook.From(Scenario_Type, "PrivateInstanceMethod")
            );

            // Using "dynamic" type to prevent caching the first call result and make the second assert fail
            dynamic scenario = (Scenario1)Activator.CreateInstance(Scenario_Type);
                       
            string methodName = scenario.InternalInstanceMethod();

            Assert.IsTrue(methodName == "MethodRedirect.Scenario1.PrivateInstanceMethod");

            token.Restore();

            methodName = scenario.InternalInstanceMethod();

            Assert.IsTrue(methodName == "MethodRedirect.Scenario1.InternalInstanceMethod");
        }

        [TestMethod]
        public void Redirect_PublicInstanceMethod_To_PrivateInstanceMethod_SameInstance()
        {
            Type Scenario_Type = typeof(Scenario1);

            var token = MethodUtil.HookMethod(
                MethodHook.From(Scenario_Type, "PublicInstanceMethod"),
                MethodHook.From(Scenario_Type, "PrivateInstanceMethod")
            );

            // Using "dynamic" type to prevent caching the first call result and make the second assert fail
            dynamic scenario = (Scenario1)Activator.CreateInstance(Scenario_Type);

            string methodName = scenario.PublicInstanceMethod();

            Assert.IsTrue(methodName == "MethodRedirect.Scenario1.PrivateInstanceMethod");

            token.Restore();

            methodName = scenario.PublicInstanceMethod();

            Assert.IsTrue(methodName == "MethodRedirect.Scenario1.PublicInstanceMethod");
        }

        [TestMethod]
        public void Redirect_PublicInstanceMethod_To_PublicStaticMethod_SameInstance()
        {
            Type Scenario_Type = typeof(Scenario1);

            var token = MethodUtil.HookMethod(
                MethodHook.From(Scenario_Type, "PublicInstanceMethod"),
                MethodHook.From(Scenario_Type, "PublicStaticMethod", true)
            );

            // Using "dynamic" type to prevent caching the first call result and make the second assert fail
            dynamic scenario = (Scenario1)Activator.CreateInstance(Scenario_Type);

            string methodName = scenario.PublicInstanceMethod();

            Assert.IsTrue(methodName == "MethodRedirect.Scenario1.PublicStaticMethod");

            token.Restore();

            methodName = scenario.PublicInstanceMethod();

            Assert.IsTrue(methodName == "MethodRedirect.Scenario1.PublicInstanceMethod");
        }
    }
}
