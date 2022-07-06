using System;
using System.Collections.Generic;

namespace MonadicBits
{
    public static class TryExtensions
    {
        public static Try<T> Try<T>(this Func<T> function, IEnumerable<Type> types) =>
            Functional.Try(types, function);

        public static Try<T> Try<T>(this Func<T> function) =>
            Functional.Try(System.Linq.Enumerable.Empty<Type>(), function);
    }
}