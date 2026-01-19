using System;

namespace Utils.Types
{
    public class AbstractSingleton<T> where T : new()
    {
        static readonly Lazy<T> Instance = new(() => new T());

        public static T instance
        {
            get { return Instance.Value; }
        }
    }
}
