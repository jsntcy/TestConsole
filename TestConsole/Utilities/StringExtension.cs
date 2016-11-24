namespace TestConsole.Utilities
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    public static class StringExtension
    {
        public static bool Contains(this string source, string value, StringComparison comparisonType)
        {
            Guard.ArgumentNotNull(source, nameof(source));
            Guard.ArgumentNotNull(value, nameof(value));

            return source.IndexOf(value, comparisonType) >= 0;
        }

        public static string ToNormalizedRelativePath(this string relativePath, char pathSeparator = '/')
        {
            Guard.ArgumentNotNullOrEmpty(relativePath, nameof(relativePath));
            Guard.Argument(() => !Path.IsPathRooted(relativePath), nameof(relativePath), $"{nameof(relativePath)} cannot be absolute.");

            string[] pathParts = relativePath.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

            return string.Join(pathSeparator.ToString(), pathParts);
        }

        public static string TrimStart(this string input, string prefixToRemove)
        {
            Guard.ArgumentNotNull(input, nameof(input));
            Guard.ArgumentNotNullOrEmpty(prefixToRemove, nameof(prefixToRemove));

            if (input.StartsWith(prefixToRemove))
            {
                return input.Substring(prefixToRemove.Length);
            }
            else
            {
                return input;
            }
        }

        public static string TrimEnd(this string input, string suffixToRemove)
        {
            if (input.EndsWith(suffixToRemove))
            {
                return input.Substring(0, input.LastIndexOf(suffixToRemove));
            }
            else
            {
                return input;
            }
        }

        public static string BackSlashToForwardSlash(this string input)
        {
            return input?.Replace('\\', '/');
        }

        public static string Replace(this string seed, char[] charsToBeReplaced, char replacementChar)
        {
            return charsToBeReplaced.Aggregate(seed, (str, charToBeReplaced) => str.Replace(charToBeReplaced, replacementChar));
        }

        public static string UppercaseFirst(this string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(s);
            }

            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }

    public static class StringUtility
    {
        static void CheckEquality(string value1, string value2)
        {
            Console.WriteLine("value1: {0}", value1);
            Console.WriteLine("value2: {0}", value2);

            Console.WriteLine("value1 == value2:      {0}", value1 == value2);
            Console.WriteLine("value1.Equals(value2): {0}", value1.Equals(value2));
        }

        public static string ToSnakeCaseName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return propertyName;
            }

            var snakeCaseName = new StringBuilder();
            var upperPart = new StringBuilder();

            for (int index = 0; index < propertyName.Length; index++)
            {
                if (char.IsUpper(propertyName[index]))
                {
                    upperPart.Append(char.ToLower(propertyName[index]));

                    if (index == propertyName.Length - 1)
                    {
                        if (upperPart.Length != propertyName.Length)
                        {
                            snakeCaseName.Append("_");
                        }
                        snakeCaseName.Append(upperPart);

                        upperPart.Clear();
                    }
                }
                else
                {
                    if (upperPart.Length > 0)
                    {
                        if (upperPart.Length != index)
                        {
                            snakeCaseName.Append("_");
                        }

                        if (upperPart.Length > 1)
                        {
                            snakeCaseName.Append(upperPart.ToString().Substring(0, upperPart.Length - 1));
                            snakeCaseName.Append("_");
                            snakeCaseName.Append(upperPart.ToString().Substring(upperPart.Length - 1));
                        }
                        else
                        {
                            snakeCaseName.Append(upperPart);
                        }

                        upperPart.Clear();
                    }

                    snakeCaseName.Append(propertyName[index]);
                }
            }

            return snakeCaseName.ToString();
        }
    }
}
