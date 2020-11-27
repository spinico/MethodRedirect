using System;

namespace MethodRedirect
{
    /// <summary>
    /// Base method operation result
    /// </summary>
    public abstract class MethodOperation : IDisposable
    {
        public abstract void Restore();

        public void Dispose()
        {
            Restore();
        }
    }

    /// <summary>
    /// Result of a method redirection (Origin => *)
    /// </summary>
    public class MethodRedirection : MethodOperation
    {
        public MethodToken Origin { get; private set; }

        public MethodRedirection(IntPtr address)
        {
            Origin = new MethodToken(address);
        }

        public override void Restore()
        {
            Origin.Restore();
        }

        public override string ToString()
        {
            return Origin.ToString();
        }
    }
}
