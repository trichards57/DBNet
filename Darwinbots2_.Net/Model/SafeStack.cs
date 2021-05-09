using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iersera.Model
{
    public class SafeStack<T> : Stack<T>
    {
        public T DefaultValue { init; get; }

        public new T Pop()
        {
            if (Count == 0)
                return DefaultValue;

            return Pop();
        }

        public new T Peek()
        {
            if (Count == 0)
                return DefaultValue;

            return Peek();
        }
    }
}
