// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using WeihanLi.Common;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class CoreExtension
{
    #region Boolean

    /// <summary>
    ///     A bool extension method that execute an Action if the value is true.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="action">The action to execute.</param>
    public static void IfTrue(this bool @this, Action? action)
    {
        if (@this)
        {
            action?.Invoke();
        }
    }

    /// <summary>
    ///     A bool extension method that execute an Action if the value is false.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="action">The action to execute.</param>
    public static void IfFalse(this bool @this, Action? action)
    {
        if (!@this)
        {
            action?.Invoke();
        }
    }

    /// <summary>
    ///     A bool extension method that show the trueValue when the @this value is true; otherwise show the falseValue.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="trueValue">The true value to be returned if the @this value is true.</param>
    /// <param name="falseValue">The false value to be returned if the @this value is false.</param>
    /// <returns>A string that represents of the current boolean value.</returns>
    public static string ToString(this bool @this, string trueValue, string falseValue)
    {
        return @this ? trueValue : falseValue;
    }

    #endregion Boolean

    #region Byte

    /// <summary>
    ///     Returns the larger of two 8-bit unsigned integers.
    /// </summary>
    /// <param name="val1">The first of two 8-bit unsigned integers to compare.</param>
    /// <param name="val2">The second of two 8-bit unsigned integers to compare.</param>
    /// <returns>Parameter  or , whichever is larger.</returns>
    public static byte Max(this byte val1, byte val2)
    {
        return Math.Max(val1, val2);
    }

    /// <summary>
    ///     Returns the smaller of two 8-bit unsigned integers.
    /// </summary>
    /// <param name="val1">The first of two 8-bit unsigned integers to compare.</param>
    /// <param name="val2">The second of two 8-bit unsigned integers to compare.</param>
    /// <returns>Parameter  or , whichever is smaller.</returns>
    public static byte Min(this byte val1, byte val2)
    {
        return Math.Min(val1, val2);
    }

    #endregion Byte

    #region ByteArray

    /// <summary>
    ///     Converts an array of 8-bit unsigned integers to its equivalent string representation that is encoded with
    ///     base-64 digits.
    /// </summary>
    /// <param name="inArray">An array of 8-bit unsigned integers.</param>
    /// <returns>The string representation, in base 64, of the contents of .</returns>
    public static string ToBase64String(this byte[] inArray)
    {
        return Convert.ToBase64String(inArray);
    }

    /// <summary>
    ///     Converts an array of 8-bit unsigned integers to its equivalent string representation that is encoded with
    ///     base-64 digits. A parameter specifies whether to insert line breaks in the return value.
    /// </summary>
    /// <param name="inArray">An array of 8-bit unsigned integers.</param>
    /// <param name="options">to insert a line break every 76 characters, or  to not insert line breaks.</param>
    /// <returns>The string representation in base 64 of the elements in .</returns>
    public static string ToBase64String(this byte[] inArray, Base64FormattingOptions options)
    {
        return Convert.ToBase64String(inArray, options);
    }

    /// <summary>
    ///     Converts a subset of an array of 8-bit unsigned integers to its equivalent string representation that is
    ///     encoded with base-64 digits. Parameters specify the subset as an offset in the input array, and the number of
    ///     elements in the array to convert.
    /// </summary>
    /// <param name="inArray">An array of 8-bit unsigned integers.</param>
    /// <param name="offset">An offset in .</param>
    /// <param name="length">The number of elements of  to convert.</param>
    /// <returns>The string representation in base 64 of  elements of , starting at position .</returns>
    public static string ToBase64String(this byte[] inArray, int offset, int length)
    {
        return Convert.ToBase64String(inArray, offset, length);
    }

    /// <summary>
    ///     Converts a subset of an array of 8-bit unsigned integers to its equivalent string representation that is
    ///     encoded with base-64 digits. Parameters specify the subset as an offset in the input array, the number of
    ///     elements in the array to convert, and whether to insert line breaks in the return value.
    /// </summary>
    /// <param name="inArray">An array of 8-bit unsigned integers.</param>
    /// <param name="offset">An offset in .</param>
    /// <param name="length">The number of elements of  to convert.</param>
    /// <param name="options">to insert a line break every 76 characters, or  to not insert line breaks.</param>
    /// <returns>The string representation in base 64 of  elements of , starting at position .</returns>
    public static string ToBase64String(this byte[] inArray, int offset, int length, Base64FormattingOptions options)
    {
        return Convert.ToBase64String(inArray, offset, length, options);
    }

#if NET5_0_OR_GREATER
    public static string ToHexString(this ReadOnlySpan<byte> bytes, bool isLowerCase = false)
    {
#if NET9_0_OR_GREATER
        return isLowerCase ? Convert.ToHexStringLower(bytes) : Convert.ToHexString(bytes);
#else
        return isLowerCase ? Convert.ToHexString(bytes).ToLowerInvariant() : Convert.ToHexString(bytes);
#endif
    }
#endif

    public static string ToHexString(this byte[] bytes, bool isLowerCase = false)
    {
#if NET9_0_OR_GREATER
        return isLowerCase ? Convert.ToHexStringLower(bytes) : Convert.ToHexString(bytes);
#elif NET5_0_OR_GREATER
        return isLowerCase ? Convert.ToHexString(bytes).ToLowerInvariant() : Convert.ToHexString(bytes);
#else
        var bitString = BitConverter.ToString(bytes).Replace("-", "");
        return isLowerCase ? bitString.ToLowerInvariant() : bitString;
#endif
    }

    /// <summary>
    ///     A byte[] extension method that resizes the byte[].
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="newSize">Size of the new.</param>
    /// <returns>A byte[].</returns>
    public static byte[] Resize(this byte[] @this, int newSize)
    {
        Array.Resize(ref @this, newSize);
        return @this;
    }

    /// <summary>
    ///     A byte[] extension method that converts the @this byteArray to a memory stream.
    /// </summary>
    /// <param name="byteArray">The byteArray to act on</param>
    /// <returns>@this as a MemoryStream.</returns>
    public static MemoryStream ToMemoryStream(this byte[] byteArray)
    {
        return new(byteArray);
    }

    public static string GetString(this byte[]? byteArray)
        => byteArray.HasValue() ? byteArray.GetString(Encoding.UTF8) : string.Empty;

    public static string GetString(this byte[] byteArray, Encoding encoding) => encoding.GetString(byteArray);

    #endregion ByteArray

    #region Char

    /// <summary>
    ///     A char extension method that repeats a character the specified number of times.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="repeatCount">Number of repeats.</param>
    /// <returns>The repeated char.</returns>
    public static string Repeat(this char @this, int repeatCount)
    {
        return new(@this, repeatCount);
    }

    /// <summary>
    ///     Converts the specified numeric Unicode character to a double-precision floating point number.
    /// </summary>
    /// <param name="c">The Unicode character to convert.</param>
    /// <returns>The numeric value of  if that character represents a number; otherwise, -1.0.</returns>
    public static double GetNumericValue(this char c)
    {
        return char.GetNumericValue(c);
    }

    /// <summary>
    ///     Categorizes a specified Unicode character into a group identified by one of the  values.
    /// </summary>
    /// <param name="c">The Unicode character to categorize.</param>
    /// <returns>A  value that identifies the group that contains .</returns>
    public static UnicodeCategory GetUnicodeCategory(this char c)
    {
        return char.GetUnicodeCategory(c);
    }

    /// <summary>
    ///     Indicates whether the specified Unicode character is categorized as a control character.
    /// </summary>
    /// <param name="c">The Unicode character to evaluate.</param>
    /// <returns>true if  is a control character; otherwise, false.</returns>
    public static bool IsControl(this char c)
    {
        return char.IsControl(c);
    }

    /// <summary>
    ///     Indicates whether the specified Unicode character is categorized as a decimal digit.
    /// </summary>
    /// <param name="c">The Unicode character to evaluate.</param>
    /// <returns>true if  is a decimal digit; otherwise, false.</returns>
    public static bool IsDigit(this char c)
    {
        return char.IsDigit(c);
    }

    /// <summary>
    ///     Indicates whether the specified Unicode character is categorized as a Unicode letter.
    /// </summary>
    /// <param name="c">The Unicode character to evaluate.</param>
    /// <returns>true if  is a letter; otherwise, false.</returns>
    public static bool IsLetter(this char c)
    {
        return char.IsLetter(c);
    }

    /// <summary>
    ///     Indicates whether the specified Unicode character is categorized as a letter or a decimal digit.
    /// </summary>
    /// <param name="c">The Unicode character to evaluate.</param>
    /// <returns>true if  is a letter or a decimal digit; otherwise, false.</returns>
    public static bool IsLetterOrDigit(this char c)
    {
        return char.IsLetterOrDigit(c);
    }

    /// <summary>
    ///     Indicates whether the specified Unicode character is categorized as a lowercase letter.
    /// </summary>
    /// <param name="c">The Unicode character to evaluate.</param>
    /// <returns>true if  is a lowercase letter; otherwise, false.</returns>
    public static bool IsLower(this char c)
    {
        return char.IsLower(c);
    }

    /// <summary>
    ///     Indicates whether the specified Unicode character is categorized as an uppercase letter.
    /// </summary>
    /// <param name="c">The Unicode character to evaluate.</param>
    /// <returns>true if  is an uppercase letter; otherwise, false.</returns>
    public static bool IsUpper(this char c)
    {
        return char.IsUpper(c);
    }

    /// <summary>
    ///     Indicates whether the specified Unicode character is categorized as a number.
    /// </summary>
    /// <param name="c">The Unicode character to evaluate.</param>
    /// <returns>true if  is a number; otherwise, false.</returns>
    public static bool IsNumber(this char c)
    {
        return char.IsNumber(c);
    }

    /// <summary>
    ///     Indicates whether the specified Unicode character is categorized as a separator character.
    /// </summary>
    /// <param name="c">The Unicode character to evaluate.</param>
    /// <returns>true if  is a separator character; otherwise, false.</returns>
    public static bool IsSeparator(this char c)
    {
        return char.IsSeparator(c);
    }

    /// <summary>
    ///     Indicates whether the specified Unicode character is categorized as a symbol character.
    /// </summary>
    /// <param name="c">The Unicode character to evaluate.</param>
    /// <returns>true if  is a symbol character; otherwise, false.</returns>
    public static bool IsSymbol(this char c)
    {
        return char.IsSymbol(c);
    }

    /// <summary>
    ///     Indicates whether the specified Unicode character is categorized as white space.
    /// </summary>
    /// <param name="c">The Unicode character to evaluate.</param>
    /// <returns>true if  is white space; otherwise, false.</returns>
    public static bool IsWhiteSpace(this char c)
    {
        return char.IsWhiteSpace(c);
    }

    /// <summary>
    ///     Converts the value of a specified Unicode character to its lowercase equivalent using specified culture-
    ///     specific formatting information.
    /// </summary>
    /// <param name="c">The Unicode character to convert.</param>
    /// <param name="culture">An object that supplies culture-specific casing rules.</param>
    /// <returns>
    ///     The lowercase equivalent of , modified according to , or the unchanged value of , if  is already lowercase or
    ///     not alphabetic.
    /// </returns>
    public static char ToLower(this char c, CultureInfo culture)
    {
        return char.ToLower(c, culture);
    }

    /// <summary>
    ///     Converts the value of a Unicode character to its lowercase equivalent.
    /// </summary>
    /// <param name="c">The Unicode character to convert.</param>
    /// <returns>
    ///     The lowercase equivalent of , or the unchanged value of , if  is already lowercase or not alphabetic.
    /// </returns>
    public static char ToLower(this char c)
    {
        return char.ToLower(c);
    }

    /// <summary>
    ///     Converts the value of a Unicode character to its lowercase equivalent using the casing rules of the invariant
    ///     culture.
    /// </summary>
    /// <param name="c">The Unicode character to convert.</param>
    /// <returns>
    ///     The lowercase equivalent of the  parameter, or the unchanged value of , if  is already lowercase or not
    ///     alphabetic.
    /// </returns>
    public static char ToLowerInvariant(this char c)
    {
        return char.ToLowerInvariant(c);
    }

    /// <summary>
    ///     Converts the value of a specified Unicode character to its uppercase equivalent using specified culture-
    ///     specific formatting information.
    /// </summary>
    /// <param name="c">The Unicode character to convert.</param>
    /// <param name="culture">An object that supplies culture-specific casing rules.</param>
    /// <returns>
    ///     The uppercase equivalent of , modified according to , or the unchanged value of  if  is already uppercase,
    ///     has no uppercase equivalent, or is not alphabetic.
    /// </returns>
    public static char ToUpper(this char c, CultureInfo culture)
    {
        return char.ToUpper(c, culture);
    }

    /// <summary>
    ///     Converts the value of a Unicode character to its uppercase equivalent.
    /// </summary>
    /// <param name="c">The Unicode character to convert.</param>
    /// <returns>
    ///     The uppercase equivalent of , or the unchanged value of  if  is already uppercase, has no uppercase
    ///     equivalent, or is not alphabetic.
    /// </returns>
    public static char ToUpper(this char c)
    {
        return char.ToUpper(c);
    }

    /// <summary>
    ///     Converts the value of a Unicode character to its uppercase equivalent using the casing rules of the invariant
    ///     culture.
    /// </summary>
    /// <param name="c">The Unicode character to convert.</param>
    /// <returns>
    ///     The uppercase equivalent of the  parameter, or the unchanged value of , if  is already uppercase or not
    ///     alphabetic.
    /// </returns>
    public static char ToUpperInvariant(this char c)
    {
        return char.ToUpperInvariant(c);
    }

    #endregion Char

    #region DateTime

    /// <summary>
    ///     A DateTime extension method that ages the given this.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>An int.</returns>
    public static int Age(this DateTime @this)
    {
        if (DateTime.Today.Month < @this.Month ||
            DateTime.Today.Month == @this.Month &&
            DateTime.Today.Day < @this.Day)
        {
            return DateTime.Today.Year - @this.Year - 1;
        }
        return DateTime.Today.Year - @this.Year;
    }

    /// <summary>
    ///     A DateTime extension method that query if 'date' is date equal.
    /// </summary>
    /// <param name="date">The date to act on.</param>
    /// <param name="dateToCompare">Date/Time of the date to compare.</param>
    /// <returns>true if date equal, false if not.</returns>
    public static bool IsDateEqual(this DateTime date, DateTime dateToCompare) => date.Date == dateToCompare.Date;

    /// <summary>
    ///     A DateTime extension method that query if '@this' is today.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>true if today, false if not.</returns>
    public static bool IsToday(this DateTime @this)
    {
        return @this.Date == DateTime.Today;
    }

    /// <summary>
    ///     A DateTime extension method that query if '@this' is a week day.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>true if '@this' is a week day, false if not.</returns>
    public static bool IsWeekDay(this DateTime @this)
    {
        return !(@this.DayOfWeek == DayOfWeek.Saturday || @this.DayOfWeek == DayOfWeek.Sunday);
    }

    /// <summary>
    ///     A DateTime extension method that query if '@this' is a week day.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>true if '@this' is a week day, false if not.</returns>
    public static bool IsWeekendDay(this DateTime @this)
    {
        return @this.DayOfWeek == DayOfWeek.Saturday || @this.DayOfWeek == DayOfWeek.Sunday;
    }

    /// <summary>
    ///     A DateTime extension method that return a DateTime with the time set to "00:00:00:000". The first moment of
    ///     the day.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>A DateTime of the day with the time set to "00:00:00:000".</returns>
    public static DateTime StartOfDay(this DateTime @this)
    {
        return new(@this.Year, @this.Month, @this.Day);
    }

    /// <summary>
    ///     A DateTime extension method that return a DateTime of the first day of the month with the time set to
    ///     "00:00:00:000". The first moment of the first day of the month.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>A DateTime of the first day of the month with the time set to "00:00:00:000".</returns>
    public static DateTime StartOfMonth(this DateTime @this)
    {
        return new(@this.Year, @this.Month, 1);
    }

    /// <summary>
    ///     A DateTime extension method that starts of week.
    /// </summary>
    /// <param name="dt">The dt to act on.</param>
    /// <param name="startDayOfWeek">(Optional) the start day of week.</param>
    /// <returns>A DateTime.</returns>
    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startDayOfWeek = DayOfWeek.Sunday)
    {
        var start = new DateTime(dt.Year, dt.Month, dt.Day);

        if (start.DayOfWeek != startDayOfWeek)
        {
            var d = startDayOfWeek - start.DayOfWeek;
            if (startDayOfWeek <= start.DayOfWeek)
            {
                return start.AddDays(d);
            }
            return start.AddDays(-7 + d);
        }

        return start;
    }

    /// <summary>
    ///     A DateTime extension method that return a DateTime of the first day of the year with the time set to
    ///     "00:00:00:000". The first moment of the first day of the year.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>A DateTime of the first day of the year with the time set to "00:00:00:000".</returns>
    public static DateTime StartOfYear(this DateTime @this)
    {
        return new(@this.Year, 1, 1);
    }

    /// <summary>
    ///     A DateTime extension method that converts the @this to an epoch time span.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>@this as a TimeSpan.</returns>
    public static TimeSpan ToEpochTimeSpan(this DateTime @this) => @this.ToUniversalTime().Subtract(new DateTime(1970, 1, 1));

    /// <summary>
    ///     A T extension method that check if the value is between inclusively the minValue and maxValue.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="maxValue">The maximum value.</param>
    /// <returns>true if the value is between inclusively the minValue and maxValue, otherwise false.</returns>
    public static bool InRange(this DateTime @this, DateTime minValue, DateTime maxValue)
    {
        return @this.CompareTo(minValue) >= 0 && @this.CompareTo(maxValue) <= 0;
    }

    /// <summary>
    ///     Converts a time to the time in a particular time zone.
    /// </summary>
    /// <param name="dateTime">The date and time to convert.</param>
    /// <param name="destinationTimeZone">The time zone to convert  to.</param>
    /// <returns>The date and time in the destination time zone.</returns>
    public static DateTime ConvertTime(this DateTime dateTime, TimeZoneInfo destinationTimeZone)
    {
        return TimeZoneInfo.ConvertTime(dateTime, destinationTimeZone);
    }

    /// <summary>
    ///     Converts a time from one time zone to another.
    /// </summary>
    /// <param name="dateTime">The date and time to convert.</param>
    /// <param name="sourceTimeZone">The time zone of .</param>
    /// <param name="destinationTimeZone">The time zone to convert  to.</param>
    /// <returns>
    ///     The date and time in the destination time zone that corresponds to the  parameter in the source time zone.
    /// </returns>
    public static DateTime ConvertTime(this DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)
    {
        return TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, destinationTimeZone);
    }

    /// <summary>
    ///     Converts a Coordinated Universal Time (UTC) to the time in a specified time zone.
    /// </summary>
    /// <param name="dateTime">The Coordinated Universal Time (UTC).</param>
    /// <param name="destinationTimeZone">The time zone to convert  to.</param>
    /// <returns>
    ///     The date and time in the destination time zone. Its  property is  if  is ; otherwise, its  property is .
    /// </returns>
    public static DateTime ConvertTimeFromUtc(this DateTime dateTime, TimeZoneInfo destinationTimeZone)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, destinationTimeZone);
    }

    /// <summary>
    ///     Converts the current date and time to Coordinated Universal Time (UTC).
    /// </summary>
    /// <param name="dateTime">The date and time to convert.</param>
    /// <returns>
    ///     The Coordinated Universal Time (UTC) that corresponds to the  parameter. The  value&#39;s  property is always
    ///     set to .
    /// </returns>
    public static DateTime ConvertTimeToUtc(this DateTime dateTime)
    {
        return TimeZoneInfo.ConvertTimeToUtc(dateTime);
    }

    /// <summary>
    ///     Converts the time in a specified time zone to Coordinated Universal Time (UTC).
    /// </summary>
    /// <param name="dateTime">The date and time to convert.</param>
    /// <param name="sourceTimeZone">The time zone of .</param>
    /// <returns>
    ///     The Coordinated Universal Time (UTC) that corresponds to the  parameter. The  object&#39;s  property is
    ///     always set to .
    /// </returns>
    public static DateTime ConvertTimeToUtc(this DateTime dateTime, TimeZoneInfo sourceTimeZone)
    {
        return TimeZoneInfo.ConvertTimeToUtc(dateTime, sourceTimeZone);
    }

    /// <summary>
    /// ToDateString("yyyy-MM-dd")
    /// </summary>
    /// <param name="this">dateTime</param>
    /// <param name="format">date format, using `yyyy-MM-dd` by default</param>
    /// <returns>The formatted date string</returns>
    public static string ToDateString(this DateTime @this, string format = "yyyy-MM-dd")
    {
        return @this.ToString(Guard.NotNull(format));
    }

    /// <summary>
    /// ToTimeString("yyyy-MM-dd HH:mm:ss")
    /// </summary>
    /// <param name="this">dateTime</param>
    /// <param name="format">dateTime format, using `yyyy-MM-dd HH:mm:ss` by default</param>
    /// <returns>The formatted time string</returns>
    public static string ToTimeString(this DateTime @this, string format = "yyyy-MM-dd HH:mm:ss")
    {
        return @this.ToString(Guard.NotNull(format));
    }

    #endregion DateTime

    #region Double

    /// <summary>
    ///     Returns the smallest integral value that is greater than or equal to the specified double-precision floating-
    ///     point number.
    /// </summary>
    /// <param name="a">A double-precision floating-point number.</param>
    /// <returns>
    ///     The smallest integral value that is greater than or equal to . If  is equal to , , or , that value is
    ///     returned. Note that this method returns a  instead of an integral type.
    /// </returns>
    public static int Ceiling(this double a) => Convert.ToInt32(Math.Ceiling(a));

    /// <summary>
    ///     Returns the natural (base e) logarithm of a specified number.
    /// </summary>
    /// <param name="d">The number whose logarithm is to be found.</param>
    /// <returns>
    ///     One of the values in the following table.  parameterReturn value Positive The natural logarithm of ; that is,
    ///     ln , or log eZero Negative Equal to Equal to.
    /// </returns>
    public static double Log(this double d)
    {
        return Math.Log(d);
    }

    /// <summary>
    ///     Returns the base 10 logarithm of a specified number.
    /// </summary>
    /// <param name="d">A number whose logarithm is to be found.</param>
    /// <returns>
    ///     One of the values in the following table.  parameter Return value Positive The base 10 log of ; that is, log
    ///     10. Zero Negative Equal to Equal to.
    /// </returns>
    public static double Log10(this double d)
    {
        return Math.Log10(d);
    }

    #endregion Double

    #region Enum

    /// <summary>
    /// A T extension method to determines whether the object is equal to any of the provided values.
    /// </summary>
    /// <param name="this">The object to be compared.</param>
    /// <param name="values">The value list to compare with the object.</param>
    /// <returns>true if the values list contains the object, else false.</returns>
    public static bool In(this Enum @this, params Enum[] values)
    {
        return Array.IndexOf(values, @this) >= 0;
    }

    /// <summary>
    /// An object extension method that gets description attribute.
    /// </summary>
    /// <param name="value">The value to act on.</param>
    /// <returns>The description attribute.</returns>
    public static string GetDescription(this Enum value)
    {
        var stringValue = value.ToString();
        var attr = value.GetType().GetField(stringValue)?
            .GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description ?? stringValue;
    }

    #endregion Enum

    #region Guid

    /// <summary>A GUID extension method that query if '@this' is empty.</summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>true if empty, false if not.</returns>
    public static bool IsNullOrEmpty(this Guid? @this)
    {
        return !@this.HasValue || @this == Guid.Empty;
    }

    /// <summary>A GUID extension method that query if '@this' is not null or empty.</summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>true if empty, false if not.</returns>
    public static bool IsNotNullOrEmpty(this Guid? @this)
    {
        return @this.HasValue && @this.Value != Guid.Empty;
    }

    /// <summary>A GUID extension method that query if '@this' is empty.</summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>true if empty, false if not.</returns>
    public static bool IsEmpty(this Guid @this)
    {
        return @this == Guid.Empty;
    }

    /// <summary>A GUID extension method that queries if a not is empty.</summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>true if a not is empty, false if not.</returns>
    public static bool IsNotEmpty(this Guid @this)
    {
        return @this != Guid.Empty;
    }

    #endregion Guid

    #region int

    /// <summary>
    ///     An Int32 extension method that factor of.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="factorNumber">The factor number.</param>
    /// <returns>true if it succeeds, false if it fails.</returns>
    public static bool FactorOf(this int @this, int factorNumber)
    {
        return factorNumber % @this == 0;
    }

    /// <summary>
    ///     An Int32 extension method that query if '@this' is even.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>true if even, false if not.</returns>
    public static bool IsEven(this int @this)
    {
        return @this % 2 == 0;
    }

    /// <summary>
    ///     An Int32 extension method that query if '@this' is odd.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>true if odd, false if not.</returns>
    public static bool IsOdd(this int @this)
    {
        return @this % 2 != 0;
    }

    /// <summary>
    ///     An Int32 extension method that query if '@this' is multiple of.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="factor">The factor.</param>
    /// <returns>true if multiple of, false if not.</returns>
    public static bool IsMultipleOf(this int @this, int factor)
    {
        return @this % factor == 0;
    }

    /// <summary>
    ///     An Int32 extension method that query if '@this' is prime.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>true if prime, false if not.</returns>
    public static bool IsPrime(this int @this)
    {
        if (@this == 1 || @this == 2)
        {
            return true;
        }

        if (@this % 2 == 0)
        {
            return false;
        }

        var sqrt = (int)Math.Sqrt(@this);
        for (var t = 3; t <= sqrt; t = t + 2)
        {
            if (@this % t == 0)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     Returns the specified 32-bit signed integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 4.</returns>
    public static byte[] GetBytes(this int value)
    {
        return BitConverter.GetBytes(value);
    }

    /// <summary>
    /// Ensures the exitCode success
    /// </summary>
    /// <param name="exitCode">exitCode</param>
    /// <param name="successCode">successCode</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Exception when exitCode not match the successCode</exception>
    [System.Diagnostics.DebuggerStepThrough]
    public static int EnsureSuccessExitCode(this int exitCode, int successCode = 0)
    {
        if (exitCode != 0)
            throw new InvalidOperationException($"Unexpected exit code:{exitCode}");

        return exitCode;
    }

    #endregion int

    #region object

    /// <summary>
    ///     An object extension method that converts the @this to an or default.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <returns>A T.</returns>
    public static T? AsOrDefault<T>(this object? @this)
    {
        if (@this is null)
        {
            return default;
        }
        try
        {
            return (T)@this;
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    ///     An object extension method that converts the @this to an or default.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>A T.</returns>
    public static T AsOrDefault<T>(this object? @this, T defaultValue)
    {
        if (@this is null)
        {
            return defaultValue;
        }
        try
        {
            return (T)@this;
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    /// <summary>
    ///     An object extension method that converts the @this to an or default.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="defaultValueFactory">The default value factory.</param>
    /// <returns>A T.</returns>
    public static T AsOrDefault<T>(this object? @this, Func<T> defaultValueFactory)
    {
        if (@this is null)
        {
            return defaultValueFactory();
        }
        try
        {
            return (T)@this;
        }
        catch (Exception)
        {
            return defaultValueFactory();
        }
    }

    /// <summary>
    ///     A System.Object extension method that toes the given this.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">this.</param>
    /// <returns>A T.</returns>
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T To<T>(this object? @this)
    {
#nullable disable

        if (@this == null || @this == DBNull.Value)
        {
            return (T)(object)null;
        }
#nullable restore

        var targetType = typeof(T).Unwrap();
        var sourceType = @this.GetType().Unwrap();
        if (sourceType == targetType)
        {
            return (T)@this;
        }
        var converter = TypeDescriptor.GetConverter(sourceType);
        if (converter.CanConvertTo(targetType))
        {
            return (T)converter.ConvertTo(@this, targetType)!;
        }

        converter = TypeDescriptor.GetConverter(targetType);
        if (converter.CanConvertFrom(sourceType))
        {
            return (T)converter.ConvertFrom(@this)!;
        }

        return (T)Convert.ChangeType(@this, targetType);
    }

    /// <summary>
    ///     A System.Object extension method that toes the given this.
    /// </summary>
    /// <param name="this">this.</param>
    /// <param name="type">The type.</param>
    /// <returns>An object.</returns>
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static object? To(this object? @this, Type type)
    {
        if (@this == null || @this == DBNull.Value)
        {
            return null;
        }

        var targetType = type.Unwrap();
        var sourceType = @this.GetType().Unwrap();

        if (sourceType == targetType)
        {
            return @this;
        }

        var converter = TypeDescriptor.GetConverter(sourceType);
        if (converter.CanConvertTo(targetType))
        {
            return converter.ConvertTo(@this, targetType);
        }

        converter = TypeDescriptor.GetConverter(targetType);
        if (converter.CanConvertFrom(sourceType))
        {
            return converter.ConvertFrom(@this);
        }

        return Convert.ChangeType(@this, targetType);
    }

    /// <summary>
    ///     A System.Object extension method that converts this object to an or default.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">this.</param>
    /// <param name="defaultValueFactory">The default value factory.</param>
    /// <returns>The given data converted to a T.</returns>
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T ToOrDefault<T>(this object? @this, Func<object?, T> defaultValueFactory)
    {
        try
        {
            return (T)@this.To(typeof(T))!;
        }
        catch (Exception)
        {
            return defaultValueFactory(@this);
        }
    }

    /// <summary>
    ///     A System.Object extension method that converts this object to an or default.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">this.</param>
    /// <param name="defaultValueFactory">The default value factory.</param>
    /// <returns>The given data converted to a T.</returns>
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T ToOrDefault<T>(this object? @this, Func<T> defaultValueFactory)
    {
        return @this.ToOrDefault(_ => defaultValueFactory());
    }

    /// <summary>
    ///     A System.Object extension method that converts this object to an or default.
    /// </summary>
    /// <param name="this">this.</param>
    /// <param name="type">type</param>
    /// <returns>The given data converted to</returns>
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static object? ToOrDefault(this object? @this, Type type)
    {
        Guard.NotNull(type, nameof(type));
        try
        {
            return @this.To(type);
        }
        catch (Exception)
        {
            return type.GetDefaultValue();
        }
    }

    /// <summary>
    ///     A System.Object extension method that converts this object to an or default.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">this.</param>
    /// <returns>The given data converted to a T.</returns>
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T? ToOrDefault<T>(this object? @this)
    {
        return @this.ToOrDefault(_ => default(T));
    }

    /// <summary>
    ///     A System.Object extension method that converts this object to an or default.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">this.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The given data converted to a T.</returns>
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T ToOrDefault<T>(this object? @this, T defaultValue)
    {
        return @this.ToOrDefault(_ => defaultValue);
    }

    /// <summary>
    ///     A T extension method that chains actions.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="action">The action.</param>
    /// <returns>The @this acted on.</returns>
    public static T Chain<T>(this T @this, Action<T>? action)
    {
        action?.Invoke(@this);

        return @this;
    }

    /// <summary>
    ///     A T extension method that gets value or default.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="func">The function.</param>
    /// <returns>The value or default.</returns>
    public static TResult? GetValueOrDefault<T, TResult>(this T @this, Func<T, TResult> func)
    {
        try
        {
            return func(@this);
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <summary>
    ///     A T extension method that gets value or default.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="func">The function.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The value or default.</returns>
    public static TResult GetValueOrDefault<T, TResult>(this T @this, Func<T, TResult> func, TResult defaultValue)
    {
        try
        {
            return func(@this);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    /// <summary>A TType extension method that tries.</summary>
    /// <typeparam name="TType">Type of the type.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="tryFunction">The try function.</param>
    /// <param name="catchValue">The catch value.</param>
    /// <returns>A TResult.</returns>
    public static TResult Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, TResult catchValue)
    {
        try
        {
            return tryFunction(@this);
        }
        catch
        {
            return catchValue;
        }
    }

    /// <summary>A TType extension method that tries.</summary>
    /// <typeparam name="TType">Type of the type.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="tryFunction">The try function.</param>
    /// <param name="catchValueFactory">The catch value factory.</param>
    /// <returns>A TResult.</returns>
    public static TResult Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, Func<TType, TResult> catchValueFactory)
    {
        try
        {
            return tryFunction(@this);
        }
        catch
        {
            return catchValueFactory(@this);
        }
    }

    /// <summary>A TType extension method that tries.</summary>
    /// <typeparam name="TType">Type of the type.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="tryFunction">The try function.</param>
    /// <param name="result">[out] The result.</param>
    /// <returns>A TResult.</returns>
    public static bool Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, out TResult? result)
    {
        try
        {
            result = tryFunction(@this);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    /// <summary>A TType extension method that tries.</summary>
    /// <typeparam name="TType">Type of the type.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="tryFunction">The try function.</param>
    /// <param name="catchValue">The catch value.</param>
    /// <param name="result">[out] The result.</param>
    /// <returns>A TResult.</returns>
    public static bool Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, TResult catchValue, out TResult result)
    {
        try
        {
            result = tryFunction(@this);
            return true;
        }
        catch
        {
            result = catchValue;
            return false;
        }
    }

    /// <summary>A TType extension method that tries.</summary>
    /// <typeparam name="TType">Type of the type.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="tryFunction">The try function.</param>
    /// <param name="catchValueFactory">The catch value factory.</param>
    /// <param name="result">[out] The result.</param>
    /// <returns>A TResult.</returns>
    public static bool Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, Func<TType, TResult> catchValueFactory, out TResult result)
    {
        try
        {
            result = tryFunction(@this);
            return true;
        }
        catch
        {
            result = catchValueFactory(@this);
            return false;
        }
    }

    /// <summary>A TType extension method that attempts to action from the given data.</summary>
    /// <typeparam name="TType">Type of the type.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="tryAction">The try action.</param>
    /// <returns>true if it succeeds, false if it fails.</returns>
    public static bool Try<TType>(this TType @this, Action<TType> tryAction)
    {
        try
        {
            tryAction(@this);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>A TType extension method that attempts to action from the given data.</summary>
    /// <typeparam name="TType">Type of the type.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="tryAction">The try action.</param>
    /// <param name="catchAction">The catch action.</param>
    /// <returns>true if it succeeds, false if it fails.</returns>
    public static bool Try<TType>(this TType @this, Action<TType> tryAction, Action<TType> catchAction)
    {
        try
        {
            tryAction(@this);
            return true;
        }
        catch
        {
            catchAction(@this);
            return false;
        }
    }

    /// <summary>
    ///     A T extension method that check if the value is between inclusively the minValue and maxValue.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="maxValue">The maximum value.</param>
    /// <returns>true if the value is between inclusively the minValue and maxValue, otherwise false.</returns>
    public static bool InRange<T>(this T @this, T minValue, T maxValue) where T : IComparable<T>
    {
        return @this.CompareTo(minValue) >= 0 && @this.CompareTo(maxValue) <= 0;
    }

    /// <summary>
    ///     A T extension method that query if 'source' is the default value.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="source">The source to act on.</param>
    /// <returns>true if default, false if not.</returns>
    public static bool IsDefault<T>(this T source)
    {
        return source is null || source.Equals(default(T));
    }

    /// <summary>
    /// Get param dictionary
    /// </summary>
    public static IDictionary<string, object?> ParseParamDictionary(this object? paramInfo)
    {
        var paramDic = new Dictionary<string, object?>();
        if (paramInfo is null)
        {
            return paramDic;
        }

        var type = paramInfo.GetType();
        if (type.IsValueTuple()) // Tuple
        {
            var fields = CacheUtil.GetTypeFields(type);
            foreach (var field in fields)
            {
                paramDic[field.Name] = field.GetValue(paramInfo);
            }
        }
        else if (paramInfo is IDictionary<string, object?> paramDictionary)
        {
            return paramDictionary;
        }
        else // get properties
        {
            var properties = CacheUtil.GetTypeProperties(type);
            foreach (var property in properties)
            {
                if (property.CanRead)
                {
                    paramDic[property.Name] = property.GetValueGetter()?.Invoke(paramInfo);
                }
            }
        }

        return paramDic;
    }
    #endregion object

    #region Random

    /// <summary>
    ///     A Random extension method that return a random value from the specified values.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="values">A variable-length parameters list containing arguments.</param>
    /// <returns>One of the specified value.</returns>
    public static T OneOf<T>(this Random @this, params T[] values)
    {
        return values[@this.Next(values.Length)];
    }

    /// <summary>
    ///     A Random extension method that flip a coin toss.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>true 50% of time, otherwise false.</returns>
    public static bool CoinToss(this Random @this)
    {
        return @this.Next(2) == 0;
    }

    #endregion Random

    #region string

    /// <summary>
    ///     A string extension method that query if '@this' is null or empty.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>true if null or empty, false if not.</returns>
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? @this) => string.IsNullOrEmpty(@this);

    /// <summary>
    ///     A string extension method that query if '@this' is not null and not empty.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>false if null or empty, true if not.</returns>
    public static bool IsNotNullOrEmpty([NotNullWhen(true)] this string? @this) => !string.IsNullOrEmpty(@this);

    /// <summary>
    ///     A string extension method that query if '@this' is null or whiteSpace.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>true if null or whiteSpace, false if not.</returns>
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? @this) => string.IsNullOrWhiteSpace(@this);

    /// <summary>
    ///     A string extension method that query if '@this' is not null and not whiteSpace.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>false if null or whiteSpace, true if not.</returns>
    public static bool IsNotNullOrWhiteSpace([NotNullWhen(true)] this string? @this) => !string.IsNullOrWhiteSpace(@this);

    /// <summary>
    ///     Concatenates the elements of an object array, using the specified separator between each element.
    /// </summary>
    /// <param name="separator">
    ///     The string to use as a separator.  is included in the returned string only if  has more
    ///     than one element.
    /// </param>
    /// <param name="values">An array that contains the elements to concatenate.</param>
    /// <returns>
    ///     A string that consists of the elements of  delimited by the  string. If  is an empty array, the method
    ///     returns .
    /// </returns>
    public static string Join<T>(this string separator, IEnumerable<T> values) => string.Join(separator, values);

    /// <summary>
    ///     Indicates whether the specified regular expression finds a match in the specified input string.
    /// </summary>
    /// <param name="input">The string to search for a match.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <returns>true if the regular expression finds a match; otherwise, false.</returns>
    public static bool IsMatch(this string input, string pattern) => Regex.IsMatch(input, pattern);

    /// <summary>
    ///     Indicates whether the specified regular expression finds a match in the specified input string, using the
    ///     specified matching options.
    /// </summary>
    /// <param name="input">The string to search for a match.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
    /// <returns>true if the regular expression finds a match; otherwise, false.</returns>
    public static bool IsMatch(this string input, string pattern, RegexOptions options) => Regex.IsMatch(input, pattern, options);

    /// <summary>An IEnumerable&lt;string&gt; extension method that concatenates the given this.</summary>
    /// <param name="stringCollection">The string collection to act on.</param>
    /// <returns>A string.</returns>
    public static string Concatenate(this IEnumerable<string> stringCollection)
    {
        return string.Join(string.Empty, stringCollection);
    }

    /// <summary>An IEnumerable&lt;T&gt; extension method that concatenates.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="source">The source to act on.</param>
    /// <param name="func">The function.</param>
    /// <returns>A string.</returns>
    public static string Concatenate<T>(this IEnumerable<T> source, Func<T, string> func)
    {
        return string.Join(string.Empty, source.Select(func));
    }

    /// <summary>
    ///     A string extension method that query if this object contains the given value.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="value">The value.</param>
    /// <returns>true if the value is in the string, false if not.</returns>
    public static bool Contains(this string @this, string value) => Guard.NotNull(@this).IndexOf(value, StringComparison.Ordinal) != -1;

    /// <summary>
    ///     A string extension method that query if this object contains the given value.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="value">The value.</param>
    /// <param name="comparisonType">Type of the comparison.</param>
    /// <returns>true if the value is in the string, false if not.</returns>
    public static bool Contains(this string @this, string value, StringComparison comparisonType) => Guard.NotNull(@this).IndexOf(value, comparisonType) != -1;

    /// <summary>
    ///     A string extension method that extracts this object.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>A string.</returns>
    public static string Extract(this string @this, Func<char, bool> predicate) => new(Guard.NotNull(@this).ToCharArray().Where(predicate).ToArray());

    /// <summary>
    ///     A string extension method that removes the letter.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>A string.</returns>
    public static string RemoveWhere(this string @this, Func<char, bool> predicate) => new(Guard.NotNull(@this).ToCharArray().Where(x => !predicate(x)).ToArray());

    /// <summary>
    ///     Replaces the format item in a specified String with the text equivalent of the value of a corresponding
    ///     Object instance in a specified array.
    /// </summary>
    /// <param name="this">A String containing zero or more format items.</param>
    /// <param name="values">An Object array containing zero or more objects to format.</param>
    /// <returns>
    ///     A copy of format in which the format items have been replaced by the String equivalent of the corresponding
    ///     instances of Object in args.
    /// </returns>
    public static string FormatWith(this string @this, params object?[] values) => string.Format(Guard.NotNull(@this), values);

    /// <summary>
    ///     A string extension method that query if '@this' satisfies the specified pattern.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="pattern">The pattern to use. Use '*' as a wildcard string.</param>
    /// <returns>true if '@this' satisfies the specified pattern, false if not.</returns>
    public static bool IsLike(this string @this, string pattern)
    {
        // Turn the pattern into a regex pattern, and match the whole string with ^$
        var regexPattern = "^" + Regex.Escape(pattern) + "$";

        // Escape special character ?, #, *, [], and [!]
        regexPattern = regexPattern.Replace(@"\[!", "[^")
            .Replace(@"\[", "[")
            .Replace(@"\]", "]")
            .Replace(@"\?", ".")
            .Replace(@"\*", ".*")
            .Replace(@"\#", @"\d");

        return Regex.IsMatch(Guard.NotNull(@this), regexPattern);
    }

    /// <summary>
    /// SafeSubstring
    /// </summary>
    /// <param name="this"></param>
    /// <param name="startIndex"></param>
    /// <returns></returns>
    public static string SafeSubstring(this string @this, int startIndex)
    {
        if (startIndex < 0 || startIndex > @this.Length)
        {
            return string.Empty;
        }
        return @this.Substring(startIndex);
    }

    /// <summary>
    /// SafeSubstring
    /// </summary>
    /// <param name="str"></param>
    /// <param name="startIndex"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string SafeSubstring(this string str, int startIndex, int length)
    {
        Guard.NotNull(str, nameof(str));
        if (startIndex < 0 || startIndex >= str.Length || length < 0)
        {
            return string.Empty;
        }
        return str.Substring(startIndex, Math.Min(str.Length - startIndex, length));
    }

    /// <summary>
    /// Sub, not only substring but support for negative numbers
    /// </summary>
    /// <param name="this">string to be handled</param>
    /// <param name="startIndex">startIndex to sub-stract</param>
    /// <returns>substring</returns>
    public static string Sub(this string @this, int startIndex)
    {
        Guard.NotNull(@this);
        if (startIndex >= 0)
        {
            return @this.SafeSubstring(startIndex);
        }
        if (Math.Abs(startIndex) > @this.Length)
        {
            return string.Empty;
        }
        return @this.Substring(@this.Length + startIndex);
    }

    /// <summary>
    ///     A string extension method that repeats the string a specified number of times.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="repeatCount">Number of repeats.</param>
    /// <returns>The repeated string.</returns>
    public static string Repeat(this string @this, int repeatCount)
    {
        Guard.NotNull(@this);
        if (@this.Length == 1)
        {
            return new string(@this[0], repeatCount);
        }

        var sb = new StringBuilder(repeatCount * @this.Length);
        while (repeatCount-- > 0)
        {
            sb.Append(@this);
        }

        return sb.ToString();
    }

    /// <summary>
    ///     A string extension method that reverses the given string.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>The string reversed.</returns>
    public static string Reverse(this string? @this)
    {
        if (@this.IsNullOrWhiteSpace())
        {
            return @this ?? string.Empty;
        }

        var chars = @this.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    /// <summary>
    ///     Returns a String array containing the substrings in this string that are delimited by elements of a specified
    ///     String array. A parameter specifies whether to return empty array elements.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="separator">A string that delimit the substrings in this string.</param>
    /// <param name="option">
    ///     (Optional) Specify RemoveEmptyEntries to omit empty array elements from the array returned,
    ///     or None to include empty array elements in the array returned.
    /// </param>
    /// <returns>
    ///     An array whose elements contain the substrings in this string that are delimited by the separator.
    /// </returns>
    public static string[] Split(this string @this, string separator, StringSplitOptions option = StringSplitOptions.None)
        => Guard.NotNull(@this).Split(new[] { separator }, option);

    /// <summary>
    ///     A string extension method that converts the @this to a byte array.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>@this as a byte[].</returns>
    public static byte[] ToBytes(this string @this) => Encoding.UTF8.GetBytes(Guard.NotNull(@this));

    /// <summary>
    ///     A string extension method that converts the @this to a byte array.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="encoding">encoding</param>
    /// <returns>@this as a byte[].</returns>
    public static byte[] ToBytes(this string @this, Encoding encoding) => encoding.GetBytes(Guard.NotNull(@this));

    public static byte[] HexStringToBytes(this string hexString)
    {
        if (string.IsNullOrEmpty(hexString))
            return [];

#if NET6_0_OR_GREATER
        return Convert.FromHexString(hexString);
#else
        var charArray = hexString.ToCharArray();
        var bytes = new byte[charArray.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            var n = Convert.ToInt32(new string([charArray[i * 2], charArray[i * 2 + 1]]), 16);
            bytes[i] = (byte)n;
        }
        return bytes;
#endif
    }

    public static byte[] GetBytes(this string str) => Guard.NotNull(str, nameof(str)).GetBytes(null);

    public static byte[] GetBytes(this string str, Encoding? encoding) => (encoding ?? Encoding.UTF8).GetBytes(Guard.NotNull(str, nameof(str)));

    public static bool ToBoolean(this string? value, bool defaultValue = false)
    {
        return value switch
        {
            null => defaultValue,
            "" or "1" or "yes" or "y" => true,
            "0" or "no" or "n" => false,
            _ => bool.TryParse(value, out var val) ? val : defaultValue
        };
    }

    /// <summary>
    ///     A string extension method that converts the @this to an enum.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <returns>@this as a T.</returns>
    public static T ToEnum<T>(this string @this) => (T)Enum.Parse(typeof(T), Guard.NotNull(@this));

    /// <summary>
    ///     A string extension method that converts the @this to a title case.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>@this as a string.</returns>
    public static string ToTitleCase(this string @this) => new CultureInfo("en-US").TextInfo.ToTitleCase(Guard.NotNull(@this));

    /// <summary>
    ///     A string extension method that converts the @this to a title case.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="cultureInfo">Information describing the culture.</param>
    /// <returns>@this as a string.</returns>
    public static string ToTitleCase(this string @this, CultureInfo cultureInfo) => cultureInfo.TextInfo.ToTitleCase(Guard.NotNull(@this));

    /// <summary>
    ///     A string extension method that truncates.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="maxLength">The maximum length.</param>
    /// <returns>A string.</returns>
    public static string Truncate(this string @this, int maxLength) => Guard.NotNull(@this).Truncate(maxLength, "...");

    /// <summary>
    ///     A string extension method that truncates.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="maxLength">The maximum length.</param>
    /// <param name="suffix">The suffix.</param>
    /// <returns>A string.</returns>
    public static string Truncate(this string @this, int maxLength, string suffix)
    {
        if (Guard.NotNull(@this).Length <= maxLength)
        {
            return @this;
        }
        return @this.Substring(0, maxLength - suffix.Length) + suffix;
    }

    /// <summary>
    /// EqualsIgnoreCase
    /// </summary>
    /// <param name="s1">string1</param>
    /// <param name="s2">string2</param>
    /// <returns></returns>
    public static bool EqualsIgnoreCase(this string? s1, string? s2)
        => string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);

    #endregion string

    #region StringBuilder

    /// <summary>A StringBuilder extension method that substrings.</summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="startIndex">The start index.</param>
    /// <returns>A string.</returns>
    public static string Substring(this StringBuilder @this, int startIndex)
    {
        return @this.ToString(startIndex, @this.Length - startIndex);
    }

    /// <summary>A StringBuilder extension method that substrings.</summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="length">The length.</param>
    /// <returns>A string.</returns>
    public static string Substring(this StringBuilder @this, int startIndex, int length)
    {
        return @this.ToString(startIndex, length);
    }

    /// <summary>A StringBuilder extension method that appends a join.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="values">The values.</param>
    public static StringBuilder AppendJoin<T>(this StringBuilder @this, string separator, IEnumerable<T> values)
    {
        @this.Append(string.Join(separator, values));

        return @this;
    }

    /// <summary>A StringBuilder extension method that appends a line join.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="values">The values.</param>
    public static StringBuilder AppendLineJoin<T>(this StringBuilder @this, string separator, IEnumerable<T> values)
    {
        @this.AppendLine(string.Join(separator, values));

        return @this;
    }

    /// <summary>
    /// Append text when condition is true
    /// </summary>
    /// <param name="builder">StringBuilder</param>
    /// <param name="text">text to append</param>
    /// <param name="condition">condition to evaluate</param>
    /// <returns>StringBuilder</returns>
    public static StringBuilder AppendIf(this StringBuilder builder, string text, bool condition)
    {
        Guard.NotNull(builder, nameof(builder));
        if (condition)
        {
            builder.Append(text);
        }
        return builder;
    }

    /// <summary>
    /// Append text when condition is true
    /// </summary>
    /// <param name="builder">StringBuilder</param>
    /// <param name="textFactory">factory for getting text to append</param>
    /// <param name="condition">condition to evaluate</param>
    /// <returns>StringBuilder</returns>
    public static StringBuilder AppendIf(this StringBuilder builder, Func<string> textFactory, bool condition)
    {
        Guard.NotNull(builder, nameof(builder));
        if (condition)
        {
            builder.Append(textFactory());
        }
        return builder;
    }

    /// <summary>
    /// Append text when condition is true
    /// </summary>
    /// <param name="builder">StringBuilder</param>
    /// <param name="text">text to append</param>
    /// <param name="condition">condition to evaluate</param>
    /// <returns>StringBuilder</returns>
    public static StringBuilder AppendLineIf(this StringBuilder builder, string text, bool condition)
    {
        Guard.NotNull(builder, nameof(builder));

        if (condition)
        {
            builder.AppendLine(text);
        }
        return builder;
    }

    /// <summary>
    /// Append text when condition is true
    /// </summary>
    /// <param name="builder">StringBuilder</param>
    /// <param name="textFactory">text factory to produce text for appending</param>
    /// <param name="condition">condition to evaluate</param>
    /// <returns>StringBuilder</returns>
    public static StringBuilder AppendLineIf(this StringBuilder builder, Func<string> textFactory, bool condition)
    {
        Guard.NotNull(builder, nameof(builder));

        if (condition)
        {
            builder.AppendLine(textFactory());
        }
        return builder;
    }

    #endregion StringBuilder

    #region TimeSpan

    /// <summary>
    ///     A TimeSpan extension method that substract the specified TimeSpan to the current DateTime.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>The current DateTime with the specified TimeSpan substracted from it.</returns>
    public static DateTime Ago(this TimeSpan @this) => DateTime.Now.Subtract(@this);

    /// <summary>
    ///     A TimeSpan extension method that add the specified TimeSpan to the current DateTime.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>The current DateTime with the specified TimeSpan added to it.</returns>
    public static DateTime FromNow(this TimeSpan @this) => DateTime.Now.Add(@this);

    /// <summary>
    ///     A TimeSpan extension method that substract the specified TimeSpan to the current UTC (Coordinated Universal
    ///     Time)
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>The current UTC (Coordinated Universal Time) with the specified TimeSpan substracted from it.</returns>
    public static DateTime UtcAgo(this TimeSpan @this) => DateTime.UtcNow.Subtract(@this);

    /// <summary>
    ///     A TimeSpan extension method that add the specified TimeSpan to the current UTC (Coordinated Universal Time)
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>The current UTC (Coordinated Universal Time) with the specified TimeSpan added to it.</returns>
    public static DateTime UtcFromNow(this TimeSpan @this) => DateTime.UtcNow.Add(@this);

    #endregion TimeSpan

    #region Type

    /// <summary>
    ///     A Type extension method that creates an instance.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>The new instance.</returns>
    public static T? CreateInstance<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] this Type @this, params object?[]? args) => (T?)Activator.CreateInstance(@this, args);

    /// <summary>
    /// if a type has empty constructor
    /// </summary>
    /// <param name="type">type</param>
    /// <returns></returns>
    public static bool HasEmptyConstructor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] this Type type)
        => Guard.NotNull(type, nameof(type)).GetConstructors(BindingFlags.Instance).Any(c => c.GetParameters().Length == 0);

    public static bool IsNullableType(this Type type)
    {
        Guard.NotNull(type, nameof(type));
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    private static readonly ConcurrentDictionary<Type, object?> DefaultValues = new();

    /// <summary>
    /// get default value by type, default(T)
    /// </summary>
    /// <param name="type">type</param>
    /// <returns>default value</returns>
    public static object? GetDefaultValue(this Type type)
    {
        Guard.NotNull(type, nameof(type));
        return type.IsValueType && type != typeof(void)
            ? DefaultValues.GetOrAdd(type, Activator.CreateInstance)
            : null;
    }

    /// <summary>
    /// GetUnderlyingType if nullable else return self
    /// </summary>
    /// <param name="type">type</param>
    /// <returns></returns>
    public static Type Unwrap(this Type type)
        => Nullable.GetUnderlyingType(Guard.NotNull(type, nameof(type))) ?? type;

    /// <summary>
    /// GetUnderlyingType
    /// </summary>
    /// <param name="type">type</param>
    /// <returns></returns>
    public static Type? GetUnderlyingType(this Type type)
        => Nullable.GetUnderlyingType(Guard.NotNull(type, nameof(type)));

    #endregion Type
}
