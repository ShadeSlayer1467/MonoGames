using System;

namespace Pong
{
    public static class RandomSingleton
    {
        private static readonly Random _instance = new Random();

        static RandomSingleton() { }

        public static Random Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
