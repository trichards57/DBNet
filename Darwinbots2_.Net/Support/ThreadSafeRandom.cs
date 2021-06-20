using System;

namespace DarwinBots.Support
{
    internal class ThreadSafeRandom
    {
        private static Random _global = new();

        [ThreadStatic]
        private static Random _local;

        public static Random Local
        {
            get
            {
                var inst = _local;
                if (inst == null)
                {
                    int seed;
                    lock (_global) seed = _global.Next();
                    _local = inst = new Random(seed);
                }
                return inst;
            }
        }
    }
}
