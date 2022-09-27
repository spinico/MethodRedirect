using System;

namespace MethodRedirect
{
    class Scenario5: Scenario5Base
    {
        double _customFee = 0.2; // An updated minimum 20% fixed fee

        public double CustomFee
        {
            get { return _customFee; }
            set { _customFee = value; }
        }

        static void Main(string[] args)
        {
            Console.ReadKey();
        }
    }

    class Scenario5Base
    {
        private double _minimumFee = 0.1; // A minimum 10% fixed fee (a design constraint for example)
        
        private double MinimumFee
        {
            get { return _minimumFee; }                       
            set { _minimumFee = value; }            
        }

        /// <summary>
        /// Increment minimum fee by 1 percent
        /// </summary>
        /// <remarks>
        /// Floating point arithmetic 
        /// </remark>
        public void IncrementMinimumFee()
        {            
            MinimumFee = (MinimumFee * 100 + 1) / 100; 
        }

        public double GetPrice(double cost)
        {            
            return cost * (1 + MinimumFee); 
        }

    }
}
