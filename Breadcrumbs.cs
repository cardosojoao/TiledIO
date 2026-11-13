#nullable enable

using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace TiledIO
{
    public static class Breadcrumbs
    {
        private static readonly AsyncLocal<ImmutableStack<object>?> Stack = new();

        public static string CurrentPath => "$" + string.Concat(GetStack().Select(FormatSegment));

        public static void Push(string value)
        {
            ArgumentNullException.ThrowIfNull(value);
            Stack.Value = (Stack.Value ?? ImmutableStack<object>.Empty).Push(value);
        }

        public static void Push(int value)
        {
            Stack.Value = (Stack.Value ?? ImmutableStack<object>.Empty).Push(value);
        }

        /// <summary>Removes the most recently pushed value if the stack has one.</summary>
        public static void Pop()
        {
            if (Stack.Value is { IsEmpty: false } stack)
            {
                Stack.Value = stack.Pop();
            }
        }

        /// <summary>Returns a snapshot from the first value pushed to the most recent.</summary>
        public static object[] GetStack()
        {
            return Stack.Value?.Reverse().ToArray() ?? [];
        }

        private static string FormatSegment(object value)
        {
            if (value is int index)
            {
                return "[" + index.ToString(CultureInfo.InvariantCulture) + "]";
            }

            string propertyName = (string)value;
            bool simpleName = propertyName.Length > 0
                && (char.IsAsciiLetter(propertyName[0]) || propertyName[0] == '_')
                && propertyName.All(c => char.IsAsciiLetterOrDigit(c) || c == '_');
            return simpleName ? "." + propertyName : "[" + JsonSerializer.Serialize(propertyName) + "]";
        }
    }
}
