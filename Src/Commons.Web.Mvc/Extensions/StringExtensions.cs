using System.Text;

namespace BoC.Web.Mvc.Extensions
{
    public static class StringExtensions
    {
        public static string ToJScriptString(this string value)
        {
            return ToJScriptString(value, false);
        }

        public static string ToJScriptString(this string value, bool forUrl)
        {
            StringBuilder builder = null;
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            int startIndex = 0;
            int count = 0;
            for (int i = 0; i < value.Length; i++)
            {
                switch (value[i])
                {
                    case '%':
                        {
                            if (!forUrl)
                            {
                                break;
                            }
                            if (builder == null)
                            {
                                builder = new StringBuilder(value.Length + 6);
                            }
                            if (count > 0)
                            {
                                builder.Append(value, startIndex, count);
                            }
                            builder.Append("%25");
                            startIndex = i + 1;
                            count = 0;
                            continue;
                        }
                    case '\'':
                        {
                            if (builder == null)
                            {
                                builder = new StringBuilder(value.Length + 5);
                            }
                            if (count > 0)
                            {
                                builder.Append(value, startIndex, count);
                            }
                            builder.Append(@"\'");
                            startIndex = i + 1;
                            count = 0;
                            continue;
                        }
                    case '\\':
                        {
                            if (builder == null)
                            {
                                builder = new StringBuilder(value.Length + 5);
                            }
                            if (count > 0)
                            {
                                builder.Append(value, startIndex, count);
                            }
                            builder.Append(@"\\");
                            startIndex = i + 1;
                            count = 0;
                            continue;
                        }
                    case '\t':
                        {
                            if (builder == null)
                            {
                                builder = new StringBuilder(value.Length + 5);
                            }
                            if (count > 0)
                            {
                                builder.Append(value, startIndex, count);
                            }
                            builder.Append(@"\t");
                            startIndex = i + 1;
                            count = 0;
                            continue;
                        }
                    case '\n':
                        {
                            if (builder == null)
                            {
                                builder = new StringBuilder(value.Length + 5);
                            }
                            if (count > 0)
                            {
                                builder.Append(value, startIndex, count);
                            }
                            builder.Append(@"\n");
                            startIndex = i + 1;
                            count = 0;
                            continue;
                        }
                    case '\r':
                        {
                            if (builder == null)
                            {
                                builder = new StringBuilder(value.Length + 5);
                            }
                            if (count > 0)
                            {
                                builder.Append(value, startIndex, count);
                            }
                            builder.Append(@"\r");
                            startIndex = i + 1;
                            count = 0;
                            continue;
                        }
                    case '"':
                        {
                            if (builder == null)
                            {
                                builder = new StringBuilder(value.Length + 5);
                            }
                            if (count > 0)
                            {
                                builder.Append(value, startIndex, count);
                            }
                            builder.Append("\\\"");
                            startIndex = i + 1;
                            count = 0;
                            continue;
                        }
                }
                count++;
            }
            if (builder == null)
            {
                return value;
            }
            if (count > 0)
            {
                builder.Append(value, startIndex, count);
            }
            return builder.ToString();
        }

    }
}
