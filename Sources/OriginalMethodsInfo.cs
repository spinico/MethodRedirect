using System;
using System.Collections.Generic;
using System.Linq;

namespace MethodRedirect
{
    /// <summary>
    /// Result of a method redirection (Origin => *)
    /// </summary>
    public class OriginalMethodsInfo : IDisposable
    {
        List<MethodToken> Origins = new List<MethodToken>();
        
        public void Dispose()
        {
            Restore();
        }

        /// <summary>
        /// Release method hook without patching back original code
        /// </summary>
        public void Release()
        {
            Origins.Clear();
        }

        public void Restore()
        { 
            Origins.ForEach(x => x.Restore());
            Origins.Clear();
        }

        public void AddOrigin(IntPtr address)
        {
            Origins.Add(new MethodToken(address));
        }

        public override string ToString()
        {
            return Origins.First().ToString();
        }
    }
}
