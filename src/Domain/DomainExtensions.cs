using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Domain
{
    public static class DomainExtensions
    {
        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> collection)
        {
            if (collection is null)
            {
                return Enumerable.Empty<T>();
            }

            return collection;
        }

        public static string GetAssemblyQualifiedName(this Type type)
        {
            return $"{type.FullName}, {type.Assembly.GetName().Name}";
        }
    }
}
