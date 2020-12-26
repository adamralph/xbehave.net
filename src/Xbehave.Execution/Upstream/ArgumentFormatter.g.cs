// UPSTREAM: https://raw.githubusercontent.com/xunit/assert.xunit/2.4.1/Sdk/ArgumentFormatter.cs
#pragma warning disable CA1305 // Specify IFormatProvider
#pragma warning disable IDE0007 // Use implicit type
#pragma warning disable IDE0011 // Add braces
#pragma warning disable IDE0018 // Inline variable declaration
#pragma warning disable IDE0019 // Use pattern matching
#pragma warning disable IDE0020 // Use pattern matching
#pragma warning disable IDE0022 // Use expression body for methods
#pragma warning disable IDE0040 // Add accessibility modifiers
#pragma warning disable IDE0045 // Convert to conditional expression
#pragma warning disable IDE0046 // Convert to conditional expression
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xunit.Sdk
{
    /// <summary>
    /// Formats arguments for display in theories.
    /// </summary>
    static class ArgumentFormatter
    {
        const int MAX_DEPTH = 3;
        const int MAX_ENUMERABLE_LENGTH = 5;
        const int MAX_OBJECT_PARAMETER_COUNT = 5;
        const int MAX_STRING_LENGTH = 50;

        static readonly object[] EmptyObjects = new object[0];
        static readonly Type[] EmptyTypes = new Type[0];

        // List of system types => C# type names
        static readonly Dictionary<TypeInfo, string> TypeMappings = new Dictionary<TypeInfo, string>
        {
            { typeof(bool).GetTypeInfo(), "bool" },
            { typeof(byte).GetTypeInfo(), "byte" },
            { typeof(sbyte).GetTypeInfo(), "sbyte" },
            { typeof(char).GetTypeInfo(), "char" },
            { typeof(decimal).GetTypeInfo(), "decimal" },
            { typeof(double).GetTypeInfo(), "double" },
            { typeof(float).GetTypeInfo(), "float" },
            { typeof(int).GetTypeInfo(), "int" },
            { typeof(uint).GetTypeInfo(), "uint" },
            { typeof(long).GetTypeInfo(), "long" },
            { typeof(ulong).GetTypeInfo(), "ulong" },
            { typeof(object).GetTypeInfo(), "object" },
            { typeof(short).GetTypeInfo(), "short" },
            { typeof(ushort).GetTypeInfo(), "ushort" },
            { typeof(string).GetTypeInfo(), "string" },
        };

        /// <summary>
        /// Format the value for presentation.
        /// </summary>
        /// <param name="value">The value to be formatted.</param>
        /// <returns>The formatted value.</returns>
        public static string Format(object value)
        {
            return Format(value, 1);
        }

        static string Format(object value, int depth)
        {
            if (value == null)
                return "null";

            var valueAsType = value as Type;
            if (valueAsType != null)
                return $"typeof({FormatTypeName(valueAsType)})";

            try
            {
                if (value.GetType().GetTypeInfo().IsEnum)
                    return value.ToString().Replace(", ", " | ");

                if (value is char)
                {
                    var charValue = (char)value;
                
                    if (charValue == '\'')
                        return @"'\''";
                
                    // Take care of all of the escape sequences
                    string escapeSequence;
                    if (TryGetEscapeSequence(charValue, out escapeSequence))
                    {
                        return $"'{escapeSequence}'";
                    }
                
                    if (char.IsLetterOrDigit(charValue) || char.IsPunctuation(charValue) || char.IsSymbol(charValue) || charValue == ' ')
                        return $"'{charValue}'";

                    // Fallback to hex
                    return $"0x{(int)charValue:x4}";
                }

                if (value is DateTime || value is DateTimeOffset)
                    return $"{value:o}";

                var stringParameter = value as string;
                if (stringParameter != null)
                {
                    stringParameter = EscapeHexChars(stringParameter);
                    stringParameter = stringParameter.Replace(@"""", @"\"""); // escape double quotes
                    if (stringParameter.Length > MAX_STRING_LENGTH)
                    {
                        string displayed = stringParameter.Substring(0, MAX_STRING_LENGTH);
                        return $"\"{displayed}\"...";
                    }

                    return $"\"{stringParameter}\"";
                }

                try
                {
                    var enumerable = value as IEnumerable;
                    if (enumerable != null)
                        return FormatEnumerable(enumerable.Cast<object>(), depth);
                }
                catch
                {
                    // Sometimes enumerables cannot be enumerated when being, and instead thrown an exception.
                    // This could be, for example, because they require state that is not provided by Xunit.
                    // In these cases, just continue formatting. 
                }

                var type = value.GetType();
                var typeInfo = type.GetTypeInfo();
                if (typeInfo.IsValueType)
                    return Convert.ToString(value, CultureInfo.CurrentCulture);

                var task = value as Task;
                if (task != null)
                {
                    var typeParameters = typeInfo.GenericTypeArguments;
                    var typeName = typeParameters.Length == 0 ? "Task" : $"Task<{string.Join(",", typeParameters.Select(FormatTypeName))}>";
                    return $"{typeName} {{ Status = {task.Status} }}";
                }

                var toString = type.GetRuntimeMethod("ToString", EmptyTypes);

                if (toString != null && toString.DeclaringType != typeof(object))
                    return (string)toString.Invoke(value, EmptyObjects);

                return FormatComplexValue(value, depth, type);
            }
            catch (Exception ex)
            {
                // Sometimes an exception is thrown when formatting an argument, such as in ToString.
                // In these cases, we don't want xunit to crash, as tests may have passed despite this.
                return $"{ex.GetType().Name} was thrown formatting an object of type \"{value.GetType()}\"";
            }
        }

        static string FormatComplexValue(object value, int depth, Type type)
        {
            if (depth == MAX_DEPTH)
                return $"{type.Name} {{ ... }}";

            var fields = type.GetRuntimeFields()
                             .Where(f => f.IsPublic && !f.IsStatic)
                             .Select(f => new { name = f.Name, value = WrapAndGetFormattedValue(() => f.GetValue(value), depth) });
            var properties = type.GetRuntimeProperties()
                                 .Where(p => p.GetMethod != null && p.GetMethod.IsPublic && !p.GetMethod.IsStatic)
                                 .Select(p => new { name = p.Name, value = WrapAndGetFormattedValue(() => p.GetValue(value), depth) });
            var parameters = fields.Concat(properties)
                                   .OrderBy(p => p.name)
                                   .Take(MAX_OBJECT_PARAMETER_COUNT + 1)
                                   .ToList();

            if (parameters.Count == 0)
                return $"{type.Name} {{ }}";

            var formattedParameters = string.Join(", ", parameters.Take(MAX_OBJECT_PARAMETER_COUNT)
                                                                  .Select(p => $"{p.name} = {p.value}"));

            if (parameters.Count > MAX_OBJECT_PARAMETER_COUNT)
                formattedParameters += ", ...";

            return $"{type.Name} {{ {formattedParameters} }}";
        }

        static string FormatEnumerable(IEnumerable<object> enumerableValues, int depth)
        {
            if (depth == MAX_DEPTH)
                return "[...]";

            var values = enumerableValues.Take(MAX_ENUMERABLE_LENGTH + 1).ToList();
            var printedValues = string.Join(", ", values.Take(MAX_ENUMERABLE_LENGTH).Select(x => Format(x, depth + 1)));

            if (values.Count > MAX_ENUMERABLE_LENGTH)
                printedValues += ", ...";

            return $"[{printedValues}]";
        }

        static string FormatTypeName(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            var arraySuffix = "";

            // Deconstruct and re-construct array
            while (typeInfo.IsArray)
            {
                var rank = typeInfo.GetArrayRank();
                arraySuffix += $"[{new string(',', rank - 1)}]";
                typeInfo = typeInfo.GetElementType().GetTypeInfo();
            }

            // Map C# built-in type names
            string result;
            if (TypeMappings.TryGetValue(typeInfo, out result))
                return result + arraySuffix;

            // Strip off generic suffix
            var name = typeInfo.FullName;

            // catch special case of generic parameters not being bound to a specific type:
            if (name == null)
                return typeInfo.Name;

            var tickIdx = name.IndexOf('`');
            if (tickIdx > 0)
                name = name.Substring(0, tickIdx);

            if (typeInfo.IsGenericTypeDefinition)
                name = $"{name}<{new string(',', typeInfo.GenericTypeParameters.Length - 1)}>";
            else if (typeInfo.IsGenericType)
            {
                if (typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
                    name = FormatTypeName(typeInfo.GenericTypeArguments[0]) + "?";
                else
                    name = $"{name}<{string.Join(", ", typeInfo.GenericTypeArguments.Select(FormatTypeName))}>";
            }

            return name + arraySuffix;
        }

        static string WrapAndGetFormattedValue(Func<object> getter, int depth)
        {
            try
            {
                return Format(getter(), depth + 1);
            }
            catch (Exception ex)
            {
                return $"(throws {UnwrapException(ex).GetType().Name})";
            }
        }

        static Exception UnwrapException(Exception ex)
        {
            while (true)
            {
                var tiex = ex as TargetInvocationException;
                if (tiex == null)
                    return ex;

                ex = tiex.InnerException;
            }
        }
        
        static string EscapeHexChars(string s)
        {
            var builder = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                char ch = s[i];
                string escapeSequence;
                if (TryGetEscapeSequence(ch, out escapeSequence))
                    builder.Append(escapeSequence);
                else if (ch < 32) // C0 control char
                    builder.AppendFormat(@"\x{0}", (+ch).ToString("x2"));
                else if (char.IsSurrogatePair(s, i)) // should handle the case of ch being the last one
                {
                    // For valid surrogates, append like normal
                    builder.Append(ch);
                    builder.Append(s[++i]);
                }
                // Check for stray surrogates/other invalid chars
                else if (char.IsSurrogate(ch) || ch == '\uFFFE' || ch == '\uFFFF')
                {
                    builder.AppendFormat(@"\x{0}", (+ch).ToString("x4"));
                }
                else
                    builder.Append(ch); // Append the char like normal
            }
            return builder.ToString();
        }
        
        static bool TryGetEscapeSequence(char ch, out string value)
        {
            value = null;
            
            if (ch == '\t') // tab
                value = @"\t";
            if (ch == '\n') // newline
                value = @"\n";
            if (ch == '\v') // vertical tab
                value = @"\v";
            if (ch == '\a') // alert
                value = @"\a";
            if (ch == '\r') // carriage return
                value = @"\r";
            if (ch == '\f') // formfeed
                value = @"\f";
            if (ch == '\b') // backspace
                value = @"\b";
            if (ch == '\0') // null char
                value = @"\0";
            if (ch == '\\') // backslash
                value = @"\\";
            
            return value != null;
        }
    }
}
