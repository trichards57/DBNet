using System.Collections.Generic;

namespace DarwinBots.Model
{
    internal class SafeStack<T> : Stack<T>
    {
        public T DefaultValue { init; get; }

        public void Over()
        {
            switch (Count)
            {
                case 0:
                    return;

                case 1:
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
            return Count == 0 ? DefaultValue : base.Peek();
        }

        public new T Pop()
        {
            return Count == 0 ? DefaultValue : base.Pop();
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
