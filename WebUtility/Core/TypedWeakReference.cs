using System;

namespace Cvv.WebUtility.Core
{
    public class WeakReference<T> : WeakReference
    {
        public WeakReference(T target) : base(target)
        {
        }

        public WeakReference(T target, bool trackResurrection) : base(target, trackResurrection)
        {
        }

        public new T Target
        {
            get { return (T) base.Target;  }
            set { base.Target = value; }
        }
    }
}