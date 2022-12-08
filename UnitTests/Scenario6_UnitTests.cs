using Microsoft.VisualStudio.TestTools.UnitTesting;
using MethodRedirect;
using System;
using System.Reflection;
using System.Diagnostics;

namespace Scenarios_UT
{
    [TestClass]
    public class Scenario6_UnitTests
    {
        [TestMethod]
        public void Redirect_PublicVirtualInstanceMethodWithParameter_To_PrivateInstanceMethodWithParameter_DifferentInstance()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Scenario6));
            Type Scenario_Type = assembly.GetType("MethodRedirect.Scenario6");
            Type ScenarioExt_Type = assembly.GetType("MethodRedirect.Scenario6Ext");

            MethodInfo Scenario_PublicVirtualInstanceMethodWithParameter = Scenario_Type.GetMethod("PublicVirtualInstanceMethodWithParameter", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo ScenarioExt_PrivateInstanceMethodWithParameter = ScenarioExt_Type.GetMethod("PrivateInstanceMethodWithParameter", BindingFlags.Instance | BindingFlags.NonPublic);

            var token = Scenario_PublicVirtualInstanceMethodWithParameter.RedirectTo(ScenarioExt_PrivateInstanceMethodWithParameter);

            var scenario = (Scenario6)Activator.CreateInstance(Scenario_Type);

            int parameter = 123;
            string methodName = scenario.PublicVirtualInstanceMethodWithParameter(parameter);

            Assert.IsTrue(methodName == "MethodRedirect.Scenario6Ext.PrivateInstanceMethodWithParameter." + parameter);

            token.Restore();

            methodName = scenario.PublicVirtualInstanceMethodWithParameter(parameter);

            Assert.IsTrue(methodName == "MethodRedirect.Scenario6.PublicVirtualInstanceMethodWithParameter." + parameter);
        }

        [TestMethod]
        public void Redirect_PublicVirtualInstanceMethod_To_Lambda_NoParameter()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Scenario6));
            Type Scenario_Type = assembly.GetType("MethodRedirect.Scenario6");

            MethodInfo Scenario_PublicVirtualInstanceMethod = Scenario_Type.GetMethod("PublicVirtualInstanceMethod", BindingFlags.Instance | BindingFlags.Public);

            // Test redirection to lambda expression (no parameter)
            var token = Scenario_PublicVirtualInstanceMethod.RedirectTo(() =>
            {
                return "MethodRedirect.LambdaExpression.NoParameter";
            });

            var scenario = (Scenario6)Activator.CreateInstance(Scenario_Type);

            string methodName = scenario.PublicVirtualInstanceMethod();

            Assert.IsTrue(methodName == "MethodRedirect.LambdaExpression.NoParameter");

            token.Restore();

            methodName = scenario.PublicVirtualInstanceMethod();

            Assert.IsTrue(methodName == "MethodRedirect.Scenario6.PublicVirtualInstanceMethod");
        }
        
        [TestMethod]
        public void Redirect_PublicVirtualInstanceMethod_To_Lambda_WithParameter()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Scenario6));
            Type Scenario_Type = assembly.GetType("MethodRedirect.Scenario6");

            MethodInfo Scenario_PublicVirtualInstanceMethodWithParameter = Scenario_Type.GetMethod("PublicVirtualInstanceMethodWithParameter", BindingFlags.Instance | BindingFlags.Public);
            
            // Test redirection to lambda expression with parameter (must use explicit type for parameter)
            var token = Scenario_PublicVirtualInstanceMethodWithParameter.RedirectTo((int x) =>
            {
                return "MethodRedirect.LambdaExpression.WithParameter." + x;
            });

            var scenario = (Scenario6)Activator.CreateInstance(Scenario_Type);

            int parameter = 456;
            string methodName = scenario.PublicVirtualInstanceMethodWithParameter(parameter);

            Assert.IsTrue(methodName == "MethodRedirect.LambdaExpression.WithParameter." + parameter);

            token.Restore();

            methodName = scenario.PublicVirtualInstanceMethodWithParameter(parameter);

            Assert.IsTrue(methodName == "MethodRedirect.Scenario6.PublicVirtualInstanceMethodWithParameter." + parameter);
        }

        [TestMethod]
        public void Redirect_AnotherPublicInstanceMethod_To_Lambda_WithParameter()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Scenario6));
            Type Scenario_Type = assembly.GetType("MethodRedirect.Scenario6");

            MethodInfo Scenario_AnotherPublicInstanceMethodWithParameter = Scenario_Type.GetMethod("AnotherPublicInstanceMethodWithParameter", BindingFlags.Instance | BindingFlags.Public);

            // Test redirection to lambda expression with parameter and integer return value (must use explicit type for parameter)
            var token = Scenario_AnotherPublicInstanceMethodWithParameter.RedirectTo((int x) =>
            {
                Debug.WriteLine("Lambda Expression Parameter = " + x.ToString());
                return x + 10;
            });

            var scenario = (Scenario6)Activator.CreateInstance(Scenario_Type);

            int parameter = 1;
            int value = scenario.AnotherPublicInstanceMethodWithParameter(parameter);

            Assert.IsTrue(value == parameter + 10);

            token.Restore();

            value = scenario.AnotherPublicInstanceMethodWithParameter(parameter);

            Assert.IsTrue(value == parameter + 1);
        }
    }
}
