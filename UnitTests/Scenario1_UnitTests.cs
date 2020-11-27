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
            Assembly assembly = Assembly.GetAssembly(typeof(Scenario1));
            Type Scenario_Type = assembly.GetType("MethodRedirect.Scenario1");

            MethodInfo Scenario_InternalInstanceMethod = Scenario_Type.GetMethod("InternalInstanceMethod", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo Scenario_PrivateInstanceMethod = Scenario_Type.GetMethod("PrivateInstanceMethod", BindingFlags.Instance | BindingFlags.NonPublic);

            var token = Scenario_InternalInstanceMethod.RedirectTo(Scenario_PrivateInstanceMethod);

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
            Assembly assembly = Assembly.GetAssembly(typeof(Scenario1));
            Type Scenario_Type = assembly.GetType("MethodRedirect.Scenario1");

            MethodInfo Scenario_PublicInstanceMethod = Scenario_Type.GetMethod("PublicInstanceMethod", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo Scenario_PrivateInstanceMethod = Scenario_Type.GetMethod("PrivateInstanceMethod", BindingFlags.Instance | BindingFlags.NonPublic);

            var token = Scenario_PublicInstanceMethod.RedirectTo(Scenario_PrivateInstanceMethod);

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
            Assembly assembly = Assembly.GetAssembly(typeof(Scenario1));
            Type Scenario_Type = assembly.GetType("MethodRedirect.Scenario1");

            MethodInfo Scenario_PublicInstanceMethod = Scenario_Type.GetMethod("PublicInstanceMethod", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo Scenario_PublicStaticMethod = Scenario_Type.GetMethod("PublicStaticMethod", BindingFlags.Static | BindingFlags.Public);

            var token = Scenario_PublicInstanceMethod.RedirectTo(Scenario_PublicStaticMethod);

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
