using System;
using System.Collections.Generic;
using System.Diagnostics;
using BoC.Extensions;

namespace BoC.Helpers
{
    public class Check
    {
        internal Check()
        {
        }

        public class Argument
        {
            internal Argument()
            {
            }

            [DebuggerStepThrough]
            public static void IsNotEmpty(Guid argument, string argumentName)
            {
                if (argument == Guid.Empty)
                {
                    throw new ArgumentException("\"{0}\" cannot be empty guid.".FormatWith(argumentName), argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotEmpty(string argument, string argumentName)
            {
                if (string.IsNullOrEmpty((argument ?? string.Empty).Trim()))
                {
                    throw new ArgumentException("\"{0}\" cannot be blank.".FormatWith(argumentName), argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotOutOfLength(string argument, int length, string argumentName)
            {
                if (argument.Trim().Length > length)
                {
                    throw new ArgumentException("\"{0}\" cannot be more than {1} character.".FormatWith(argumentName, length), argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotNull(object argument, string argumentName)
            {
                if (argument == null)
                {
                    throw new ArgumentNullException(argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotNegative(int argument, string argumentName)
            {
                if (argument < 0)
                {
                    throw new ArgumentOutOfRangeException(argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotNegativeOrZero(int argument, string argumentName)
            {
                if (argument <= 0)
                {
                    throw new ArgumentOutOfRangeException(argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotNegative(long argument, string argumentName)
            {
                if (argument < 0)
                {
                    throw new ArgumentOutOfRangeException(argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotNegativeOrZero(long argument, string argumentName)
            {
                if (argument <= 0)
                {
                    throw new ArgumentOutOfRangeException(argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotNegative(float argument, string argumentName)
            {
                if (argument < 0)
                {
                    throw new ArgumentOutOfRangeException(argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotNegativeOrZero(float argument, string argumentName)
            {
                if (argument <= 0)
                {
                    throw new ArgumentOutOfRangeException(argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotNegative(decimal argument, string argumentName)
            {
                if (argument < 0)
                {
                    throw new ArgumentOutOfRangeException(argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotNegativeOrZero(decimal argument, string argumentName)
            {
                if (argument <= 0)
                {
                    throw new ArgumentOutOfRangeException(argumentName);
                }
            }


            [DebuggerStepThrough]
            public static void IsNotInPast(DateTime argument, string argumentName)
            {
                if (argument < DateTime.Now)
                {
                    throw new ArgumentOutOfRangeException(argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotInFuture(DateTime argument, string argumentName)
            {
                if (argument > DateTime.Now)
                {
                    throw new ArgumentOutOfRangeException(argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotNegative(TimeSpan argument, string argumentName)
            {
                if (argument < TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotNegativeOrZero(TimeSpan argument, string argumentName)
            {
                if (argument <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotEmpty<T>(ICollection<T> argument, string argumentName)
            {
                IsNotNull(argument, argumentName);

                if (argument.Count == 0)
                {
                    throw new ArgumentException("Collection cannot be empty.", argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotOutOfRange(int argument, int min, int max, string argumentName)
            {
                if ((argument < min) || (argument > max))
                {
                    throw new ArgumentOutOfRangeException(argumentName, "{0} must be between \"{1}\"-\"{2}\".".FormatWith(argumentName, min, max));
                }
            }

            [DebuggerStepThrough]
            public static void IsNotInvalidEmail(string argument, string argumentName)
            {
                IsNotEmpty(argument, argumentName);

                if (!argument.IsEmail())
                {
                    throw new ArgumentException("\"{0}\" is not a valid email address.".FormatWith(argumentName), argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotInvalidWebUrl(string argument, string argumentName)
            {
                IsNotEmpty(argument, argumentName);

                if (!argument.IsWebUrl())
                {
                    throw new ArgumentException("\"{0}\" is not a valid web url.".FormatWith(argumentName), argumentName);
                }
            }
        }
    }
}