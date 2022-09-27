using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MethodRedirect
{
    static class Extensions
    {
        /// <summary>
        /// Redirect origin method calls to the specified target method.
        /// Use redirection when there is no need to call the origin method when redirected to the target method.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <param name="verbose"></param>
        /// <returns>
        /// A MethodRedirection operation result object
        /// </returns>
        public static MethodOperation RedirectTo(this MethodInfo origin, MethodInfo target, bool verbose = false)
        {
            IntPtr ori = GetMethodAddress(origin);
            IntPtr tar = GetMethodAddress(target);

            // The function pointer gives the value located at the method address...
            // This can be used for validation that the method address matches     
            Debug.Assert(Marshal.ReadIntPtr(ori) == origin.MethodHandle.GetFunctionPointer());
            Debug.Assert(Marshal.ReadIntPtr(tar) == target.MethodHandle.GetFunctionPointer());

            if (verbose)
            {
                Console.WriteLine("\nPlatform      : {0}", IntPtr.Size == 4 ? "x86" : "x64");
                Console.WriteLine("IntPtr.Size   : {0}", IntPtr.Size);

                Console.WriteLine("\nFrom origin method    : {0}", origin.Name);
                OutputMethodDetails(origin, ori);

                Console.WriteLine("\nTo target method      : {0}", target.Name);
                OutputMethodDetails(target, tar);
            }

            return Redirect(ori, tar, verbose);
        }

        /// <summary>
        /// Show the details of the method address evaluation
        /// </summary>
        /// <param name="mi"></param>
        /// <param name="address">
        /// The unconditional jmp address to the JIT-compiled method
        /// </param>
        private static void OutputMethodDetails(MethodInfo mi, IntPtr address)
        {
            IntPtr mt = mi.DeclaringType.TypeHandle.Value; // MethodTable address
            IntPtr md = mi.MethodHandle.Value;             // MethodDescriptor address
            
            // MethodTable address > MethodDescriptor address
            int offset = (int)((long)mt - (long)md);

            Console.WriteLine("Method is virtual     : {0}", mi.IsVirtual);
            Console.WriteLine("MethodDescriptor (MD) : \t{0}", md.ToString("x").PadLeft(8, '0'));
            Console.WriteLine("MethodTable (MT)      : \t{0}", mt.ToString("x").PadLeft(8,'0'));            
            Console.WriteLine("Offset (MT - MD)      : \t{0}", offset.ToString("x").PadLeft(8, '0'));

            if (mi.IsVirtual)
            {
                // The fixed-size portion of the MethodTable structure depends on the process type:
                // For 32-bit process (IntPtr.Size == 4), the fixed-size portion is 40 (0x28) bytes
                // For 64-bit process (IntPtr.Size == 8), the fixed-size portion is 64 (0x40) bytes
                offset = IntPtr.Size == 4 ? 0x28 : 0x40;
                                
                // First method slot = MethodTable address + fixed-size offset
                // This is the address of the first method of any type (i.e. ToString)
                IntPtr ms = mt + offset;
              
                Console.WriteLine("MethodTable offset    : \t{0}", offset.ToString("x").PadLeft(8, '0'));
                Console.WriteLine("First method slot     : {0}\t{1}", 
                    Marshal.ReadIntPtr(ms).ToString("x").PadLeft(8,'0'),
                    ms.ToString("x").PadLeft(8,'0')); 

                // To get the slot number of the virtual method entry from the MethodDesc data structure
                //
                //  a. Get the value at the address of the MethodDescriptor 
                //
                //                         MethodDesc data structure  
                //                        ---------------------------          
                //    MethodDescriptor -> | Token Remainder         | 2 bytes
                //                        | Chunck Index            | 1 bytes  
                //    MethodTableSlot  -> | Stub (op code + target) | 5 bytes
                //                        | Slot Number             | 2 bytes
                //                        | Flags                   | 2 bytes
                //                        | CodeOrIL                | 4 bytes
                //                        ---------------------------      
                //
                //  b. Right shift the value by 8 bytes (32 bits)
                //  c. Mask the slot number field to get its value  

                long shift = Marshal.ReadInt64(md) >> 32;
                ushort mask = 0xffff; // 16 bit (2 bytes) mask for the slot number value
                int slot = (int)(shift & mask);

                Console.WriteLine("\nMethodDesc data       : {0}", Marshal.ReadInt64(md).ToString("x").PadLeft(8,'0'));
                Console.WriteLine("Right-shift 32 bits   : {0}", shift.ToString("x").PadLeft(8,'0'));
                Console.WriteLine("Mask                  : {0}", mask.ToString("x").PadLeft(8,'0'));
                Console.WriteLine("Method slot number    : {0}", slot);

                Console.WriteLine("\nMethodDesc data       : {0}", Convert.ToString(Marshal.ReadInt64(md), 2).PadLeft(32, '0'));
                Console.WriteLine("Right-shift 32 bits   : {0}", Convert.ToString(shift, 2).PadLeft(32, '0'));
                Console.WriteLine("Mask                  : {0}", Convert.ToString(mask, 2).PadLeft(32, '0'));
                Console.WriteLine("Method slot number    : {0}\n", Convert.ToString(slot,2).PadLeft(32, '0'));
                
                Console.WriteLine("Relative offset       : {0}", (IntPtr.Size * slot).ToString("x").PadLeft(8,'0'));
            }
            
            offset = (int)((long)address - (long)mt);

            Console.WriteLine("Jitted method (JM)    : {0}\t{1}", address.ToString("x").PadLeft(8,'0'), 
                Marshal.ReadIntPtr(address).ToString("x").PadLeft(8,'0'));

            Console.WriteLine("Offset (JM - MT)      : {0}", offset);
        }

        /// <summary>
        /// Obtain the unconditional jump address to the JIT-compiled method
        /// </summary>
        /// <param name="mi"></param>
        /// <remarks>
        /// Before JIT compilation:
        ///   - call to PreJITStub to initiate compilation.            
        ///   - the CodeOrIL field contains the Relative Virtual Address (IL RVA) of the method implementation in IL.
        ///
        /// After on-demand JIT compilation:
        ///   - CRL changes the call to the PreJITStub for an unconditional jump to the JITed method.
        ///   - the CodeOrIL field contains the Virtual Address (VA) of the JIT-compiled method.
        /// </remarks>
        /// <returns>The JITed method address</returns>
        private static IntPtr GetMethodAddress(MethodInfo mi)
        {
            const ushort SLOT_NUMBER_MASK = 0xffff; // 2 bytes mask
            const int MT_OFFSET_32BIT = 0x28;       // 40 bytes offset
            const int MT_OFFSET_64BIT = 0x40;       // 64 bytes offset

            IntPtr address;

            // JIT compilation of the method
            RuntimeHelpers.PrepareMethod(mi.MethodHandle);

            IntPtr md = mi.MethodHandle.Value;             // MethodDescriptor address
            IntPtr mt = mi.DeclaringType.TypeHandle.Value; // MethodTable address

            if (mi.IsVirtual)
            {
                // The fixed-size portion of the MethodTable structure depends on the process type:
                // For 32-bit process (IntPtr.Size == 4), the fixed-size portion is 40 (0x28) bytes
                // For 64-bit process (IntPtr.Size == 8), the fixed-size portion is 64 (0x40) bytes
                int offset = IntPtr.Size == 4 ? MT_OFFSET_32BIT : MT_OFFSET_64BIT;

                // First method slot = MethodTable address + fixed-size offset
                // This is the address of the first method of any type (i.e. ToString)
                IntPtr ms = Marshal.ReadIntPtr(mt + offset);

                // Get the slot number of the virtual method entry from the MethodDesc data structure
                long shift = Marshal.ReadInt64(md) >> 32;
                int slot = (int)(shift & SLOT_NUMBER_MASK);
                
                // Get the virtual method address relative to the first method slot
                address = ms + (slot * IntPtr.Size);                                
            }
            else
            {
                // Bypass default MethodDescriptor padding (8 bytes) 
                // Reach the CodeOrIL field which contains the address of the JIT-compiled code
                address = md + 8;
            }

            return address;
        }

        private static MethodRedirection Redirect(IntPtr ori, IntPtr tar, bool verbose)
        {            
            // Must create the token before address is assigned
            var token = new MethodRedirection(ori);

            if (verbose)
            {
                Console.WriteLine("\nRedirect...");
                Console.WriteLine("From {0} [{1}] => To {2} [{3}]",
                    ori.ToString("x").PadLeft(8, '0'),
                    Marshal.ReadIntPtr(ori).ToString("x").PadLeft(8, '0'),
                    tar.ToString("x").PadLeft(8, '0'),
                    Marshal.ReadIntPtr(tar).ToString("x").PadLeft(8, '0'));
            }

            // Redirect origin method to target method            
            Marshal.Copy(new IntPtr[] { Marshal.ReadIntPtr(tar) }, 0, ori, 1);

            return token;
        }
    }
}
