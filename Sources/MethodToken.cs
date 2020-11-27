using System;
using System.Runtime.InteropServices;

namespace MethodRedirect
{
    public struct MethodToken : IDisposable
    {
        public IntPtr Address { get; private set; }
        public IntPtr Value { get; private set; }

        public MethodToken(IntPtr address)
        {
            // On token creation, preserve the address and the current value at this address
            Address = address;
            Value = Marshal.ReadIntPtr(address);
        }

        public void Restore()
        {
            // Restore the value at the address            
            Marshal.Copy(new IntPtr[] { Value }, 0, Address, 1);
        }

        public override string ToString()
        {
            IntPtr met = Address;
            IntPtr tar = Marshal.ReadIntPtr(Address);
            IntPtr ori = Value;

            return "Method address = " + met.ToString("x").PadLeft(8, '0') + "\n" +
                   "Target address = " + tar.ToString("x").PadLeft(8, '0') + "\n" +
                   "Origin address = " + ori.ToString("x").PadLeft(8, '0');
        }

        public void Dispose()
        {
            Restore();
        }
    }
}
