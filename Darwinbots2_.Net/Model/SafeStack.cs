using System.Collections.Generic;

namespace DarwinBots.Model
{
    public class SafeStack<T> : Stack<T>
    {
        public T DefaultValue { init; get; }

        public void Over()
        {
            if (Count == 0)
                return;

            if (Count == 1)
            {
                Push(DefaultValue);
                return;
            }

            var a = Pop();
            var b = Pop();
            Push(a);
            Push(b);
            Push(a);
        }

        public new T Peek()
        {
            if (Count == 0)
                return DefaultValue;

            return Peek();
        }

        public new T Pop()
        {
            if (Count == 0)
                return DefaultValue;

            return Pop();
        }

        public void Swap()
        {
            if (Count <= 1)
                return;

            var a = Pop();
            var b = Pop();
            Push(a);
            Push(b);
        }
    }
}
