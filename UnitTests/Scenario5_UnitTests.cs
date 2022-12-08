using Microsoft.VisualStudio.TestTools.UnitTesting;
using MethodRedirect;
using System;
using System.Reflection;

namespace Scenarios_UT
{
    [TestClass]
    public class Scenario5_UnitTests
    {
        [TestMethod]
        public void Redirect_PrivateAccessorMethods_To_PublicAccessorMethods_DerivedInstance()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Scenario5));
            Type Scenario_Type = assembly.GetType("MethodRedirect.Scenario5");
            Type ScenarioBase_Type = assembly.GetType("MethodRedirect.Scenario5Base");

            PropertyInfo Scenario_CustomFeeProperty = Scenario_Type.GetProperty("CustomFee", BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo ScenarioBase_MinimumFeeProperty = ScenarioBase_Type.GetProperty("MinimumFee", BindingFlags.Instance | BindingFlags.NonPublic);

            // Obtain public accessor methods from main class
            MethodInfo Scenario_PublicGetCustomFeeMethod = Scenario_CustomFeeProperty.GetGetMethod();
            MethodInfo Scenario_PublicSetCustomFeeMethod = Scenario_CustomFeeProperty.GetSetMethod();

            // Obtain private accessor methods from base class
            MethodInfo ScenarioBase_PrivateGetMinimumFeeMethod = ScenarioBase_MinimumFeeProperty.GetGetMethod(true); // "true" for private accessor
            MethodInfo ScenarioBase_PrivateSetMinimumFeeMethod = ScenarioBase_MinimumFeeProperty.GetSetMethod(true); // "true" for private accessor

            FieldInfo ScenarioBase_PrivateMinimumFeeField = ScenarioBase_Type.GetField("_minimumFee", BindingFlags.Instance | BindingFlags.NonPublic);

            // Redirect the base class' private property accessors with the main's class public ones
            // Important: this must be done BEFORE creating an instance of the scenario otherwise the redirected addresses won't be set correctly.
            var tokenGet = ScenarioBase_PrivateGetMinimumFeeMethod.RedirectTo(Scenario_PublicGetCustomFeeMethod);
            var tokenSet = ScenarioBase_PrivateSetMinimumFeeMethod.RedirectTo(Scenario_PublicSetCustomFeeMethod);

            // Create instance of scenario
            var scenario = new Scenario5(); // (Scenario5)Activator.CreateInstance(Scenario_Type);

            // Test the "Get" accessor redirection
            double price = scenario.GetPrice(100);

            Assert.IsTrue(price == 120); // The price has a 20% fee added (instead of 10%) which is the custom value set on the main class

            // Test the "Set" accessor redirection
            scenario.IncrementMinimumFee();

            Assert.IsTrue(scenario.CustomFee == 0.21); // The custom fee should now be 21%

            // Test "Get" accessor from base class 
            double customFee = (double) ScenarioBase_PrivateGetMinimumFeeMethod.Invoke(scenario, null);

            Assert.IsTrue(customFee == 0.21); // The fee value should be the same value as the same redirected accessor was called

            // Another verification is to check the private member value of the base class
            double minimumFee = (double) ScenarioBase_PrivateMinimumFeeField.GetValue(scenario);

            Assert.IsTrue(minimumFee == 0.1); // Should be the default 10% field fee

            // Restoring accessors methods to original addresses
            tokenGet.Restore();
            tokenSet.Restore();

            // The base method addresses have been restored, for example a call to the private "Get"
            // accessor of the base class should return the original value
            minimumFee = (double) ScenarioBase_PrivateGetMinimumFeeMethod.Invoke(scenario, null);

            Assert.IsTrue(minimumFee == 0.1); // The 10% base class default value

            // **IMPORTANT** the restore operations have no effect on calls to
            // redirected methods made within already compiled methods.

            // For example, calling the GetPrice() method again that uses the private MinimumFee
            // property value to evaluate the result will not use the original value.
            double value = scenario.GetPrice(100);

            Assert.IsTrue(value == 121); // The GetPrice() method still uses the custom fee value of 21% set earlier

            // For the same reason stated above, the base class property "Set" accessor won't be
            // called within the IncrementMinimumFee() method.

            scenario.IncrementMinimumFee();

            minimumFee = (double) ScenarioBase_MinimumFeeProperty.GetValue(scenario);

            Assert.IsTrue(minimumFee == 0.1); // The value of the base's class MinimumFee remains unchanged.

            // But, since the call used the redirected method, now the custom fee value of the main derived class has been updated.
            Assert.IsTrue(scenario.CustomFee == 0.22);

        }
    }
}
