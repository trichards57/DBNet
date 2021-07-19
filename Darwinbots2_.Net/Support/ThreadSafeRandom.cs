using System;

namespace DarwinBots.Support
{
    internal static class ThreadSafeRandom
    {
        private static readonly Random Global = new();

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
                    lock (Global) seed = Global.Next();
                    _local = inst = new Random(seed);
                }
                return inst;
            }
        }
    }
}
