using System;

namespace WeihanLi.Common
{
    public static class Guard
    {
        public static T NotNull<T>(T t, string paramName)
        {
            if (t is null)
            {
                throw new ArgumentNullException(paramName);
            }
            return t;
        }

        public static string NotNullOrEmpty(string str, string paramName)
        {
            NotNull(str, paramName);
            if (string.IsNullOrEmpty(paramName))
            {
                throw new ArgumentException("The argument is empty", paramName);
            }
            return str;
        }
    }
}
