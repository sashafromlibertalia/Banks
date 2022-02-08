using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Banks.Tools
{
    public static class Validator
    {
        public static void NotNull(this object argument, string message)
        {
            if (argument == null)
                throw new ArgumentNullException(string.Empty, message);
        }

        public static void NotEmpty<T>(this IEnumerable<T> argument, string message)
        {
            if (argument.ToList().Count == 0)
                throw new BanksException(message);
        }

        public static void WithoutNull<T>(this IEnumerable<T> argument, string message)
        {
            if (argument.Any(item => item == null || string.IsNullOrEmpty(item.ToString())))
                throw new ArgumentNullException(string.Empty, message);
        }

        public static void WithoutDuplicates<T>(this IEnumerable<T> argument, T item, string message)
        {
            if (argument.Any(arg => arg.Equals(item)))
                throw new DuplicateNameException(message);
        }

        public static void AddedToCollection<T>(this IEnumerable<T> argument, T item, string message)
        {
            if (argument.All(arg => !arg.Equals(item)))
                throw new DuplicateNameException(message);
        }
    }
}