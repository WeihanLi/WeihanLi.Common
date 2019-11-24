namespace WeihanLi.Common.Helpers
{
    public static class ArrayHelper
    {
#if NET45
        public static T[] Empty<T>() => EmptyArray<T>.Value;

        private static class EmptyArray<T>
        {
            public static readonly T[] Value = new T[0];
        }
#endif
    }
}
