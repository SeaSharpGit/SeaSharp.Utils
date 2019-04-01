using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaSharp.Utils.CSharp
{
    using System;
    using System.Reflection;
    namespace Artech.MemLeakByEvents
    {
        public class WeakEventHandler<TEventArgs> where TEventArgs : EventArgs
        {
            public WeakReference Reference { get; private set; }

            public MethodInfo Method { get; private set; }

            public EventHandler<TEventArgs> Handler { get; private set; }

            public WeakEventHandler(EventHandler<TEventArgs> eventHandler)
            {
                Reference = new WeakReference(eventHandler.Target);
                Method = eventHandler.Method;
                Handler = Invoke;
            }

            public void Invoke(object sender, TEventArgs e)
            {
                object target = Reference.Target;
                if (null != target)
                {
                    Method.Invoke(target, new object[] { sender, e });
                }
            }

            public static implicit operator EventHandler<TEventArgs>(WeakEventHandler<TEventArgs> weakHandler)
            {
                return weakHandler.Handler;
            }
        }
    }
}
