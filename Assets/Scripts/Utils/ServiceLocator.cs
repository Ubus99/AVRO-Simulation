using System;
using System.Collections.Concurrent;

namespace Utils
{
    public class ServiceLocator : AbstractSingleton<ServiceLocator>
    {
        readonly ConcurrentDictionary<Type, object> _services = new();

        public bool TryRegister<T>(object service)
        {
            return _services.TryAdd(typeof(T), service);
        }

        public bool TryGet<T>(out T service)
        {
            var s = _services.TryGetValue(typeof(T), out var r);
            service = (T)r;
            return s;
        }
    }
}
