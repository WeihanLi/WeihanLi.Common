using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

// ReSharper disable LocalizableElement


/* Unmerged change from project 'WeihanLi.Common(net6.0)'
Before:
namespace WeihanLi.Common
{
    /// <summary>
    /// Represents an ObjectId
    /// refer to https://github.com/tangxuehua/ecommon/blob/master/src/ECommon/Utilities/ObjectId.cs
    /// </summary>
    [Serializable]
    public struct ObjectId : IComparable<ObjectId>, IEquatable<ObjectId>
    {
        // private static fields
        private static readonly DateTime _unixEpoch;

        private static readonly ObjectId _emptyInstance = default;
        private static readonly int _staticMachine;
        private static readonly short _staticPid;
        private static int _staticIncrement; // high byte will be masked out when generating new ObjectId

        private static readonly uint[] _lookup32 = Enumerable.Range(0, 256).Select(i =>
                 {
                     var s = i.ToString("x2");
                     return ((uint)s[0]) + ((uint)s[1] << 16);
                 }).ToArray();

        // we're using 14 bytes instead of 12 to hold the ObjectId in memory but unlike a byte[] there is no additional object on the heap
        // the extra two bytes are not visible to anyone outside of this class and they buy us considerable simplification
        // an additional advantage of this representation is that it will serialize to JSON without any 64 bit overflow problems
        private readonly int _timestamp;

        private readonly int _machine;
        private readonly short _pid;
        private readonly int _increment;

        // static constructor
        static ObjectId()
        {
            _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            _staticMachine = GetMachineHash();
            _staticIncrement = (new Random()).Next();
            _staticPid = (short)GetCurrentProcessId();
        }

        // constructors
        /// <summary>
        /// Initializes a new instance of the ObjectId class.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        public ObjectId(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            Unpack(bytes, out _timestamp, out _machine, out _pid, out _increment);
        }

        /// <summary>
        /// Initializes a new instance of the ObjectId class.
        /// </summary>
        /// <param name="timestamp">The timestamp (expressed as a DateTime).</param>
        /// <param name="machine">The machine hash.</param>
        /// <param name="pid">The PID.</param>
        /// <param name="increment">The increment.</param>
        public ObjectId(DateTime timestamp, int machine, short pid, int increment)
            : this(GetTimestampFromDateTime(timestamp), machine, pid, increment)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ObjectId class.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="machine">The machine hash.</param>
        /// <param name="pid">The PID.</param>
        /// <param name="increment">The increment.</param>
        public ObjectId(int timestamp, int machine, short pid, int increment)
        {
            if ((machine & 0xff000000) != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(machine), "The machine value must be between 0 and 16777215 (it must fit in 3 bytes).");
            }
            if ((increment & 0xff000000) != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(increment), "The increment value must be between 0 and 16777215 (it must fit in 3 bytes).");
            }

            _timestamp = timestamp;
            _machine = machine;
            _pid = pid;
            _increment = increment;
        }

        /// <summary>
        /// Initializes a new instance of the ObjectId class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ObjectId(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            Unpack(ParseHexString(value), out _timestamp, out _machine, out _pid, out _increment);
        }

        // public static properties
        /// <summary>
        /// Gets an instance of ObjectId where the value is empty.
        /// </summary>
        public static ObjectId Empty => _emptyInstance;

        // public properties
        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        public int Timestamp => _timestamp;

        /// <summary>
        /// Gets the machine.
        /// </summary>
        public int Machine => _machine;

        /// <summary>
        /// Gets the PID.
        /// </summary>
        public short Pid => _pid;

        /// <summary>
        /// Gets the increment.
        /// </summary>
        public int Increment => _increment;

        /// <summary>
        /// Gets the creation time (derived from the timestamp).
        /// </summary>
        public DateTime CreationTime => _unixEpoch.AddSeconds(_timestamp);

        // public operators
        /// <summary>
        /// Compares two ObjectIds.
        /// </summary>
        /// <param name="lhs">The first ObjectId.</param>
        /// <param name="rhs">The other ObjectId</param>
        /// <returns>True if the first ObjectId is less than the second ObjectId.</returns>
        public static bool operator <(ObjectId lhs, ObjectId rhs)
        {
            return lhs.CompareTo(rhs) < 0;
        }

        /// <summary>
        /// Compares two ObjectIds.
        /// </summary>
        /// <param name="lhs">The first ObjectId.</param>
        /// <param name="rhs">The other ObjectId</param>
        /// <returns>True if the first ObjectId is less than or equal to the second ObjectId.</returns>
        public static bool operator <=(ObjectId lhs, ObjectId rhs)
        {
            return lhs.CompareTo(rhs) <= 0;
        }

        /// <summary>
        /// Compares two ObjectIds.
        /// </summary>
        /// <param name="lhs">The first ObjectId.</param>
        /// <param name="rhs">The other ObjectId.</param>
        /// <returns>True if the two ObjectIds are equal.</returns>
        public static bool operator ==(ObjectId lhs, ObjectId rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Compares two ObjectIds.
        /// </summary>
        /// <param name="lhs">The first ObjectId.</param>
        /// <param name="rhs">The other ObjectId.</param>
        /// <returns>True if the two ObjectIds are not equal.</returns>
        public static bool operator !=(ObjectId lhs, ObjectId rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Compares two ObjectIds.
        /// </summary>
        /// <param name="lhs">The first ObjectId.</param>
        /// <param name="rhs">The other ObjectId</param>
        /// <returns>True if the first ObjectId is greater than or equal to the second ObjectId.</returns>
        public static bool operator >=(ObjectId lhs, ObjectId rhs)
        {
            return lhs.CompareTo(rhs) >= 0;
        }

        /// <summary>
        /// Compares two ObjectIds.
        /// </summary>
        /// <param name="lhs">The first ObjectId.</param>
        /// <param name="rhs">The other ObjectId</param>
        /// <returns>True if the first ObjectId is greater than the second ObjectId.</returns>
        public static bool operator >(ObjectId lhs, ObjectId rhs)
        {
            return lhs.CompareTo(rhs) > 0;
        }

        // public static methods
        /// <summary>
        /// Generates a new ObjectId with a unique value.
        /// </summary>
        /// <returns>An ObjectId.</returns>
        public static ObjectId GenerateNewId() => GenerateNewId(GetTimestampFromDateTime(DateTime.UtcNow));

        /// <summary>
        /// Generates a new ObjectId with a unique value (with the timestamp component based on a given DateTime).
        /// </summary>
        /// <param name="timestamp">The timestamp component (expressed as a DateTime).</param>
        /// <returns>An ObjectId.</returns>
        public static ObjectId GenerateNewId(DateTime timestamp) => GenerateNewId(GetTimestampFromDateTime(timestamp));

        /// <summary>
        /// Generates a new ObjectId with a unique value (with the given timestamp).
        /// </summary>
        /// <param name="timestamp">The timestamp component.</param>
        /// <returns>An ObjectId.</returns>
        private static ObjectId GenerateNewId(int timestamp)
        {
            var increment = Interlocked.Increment(ref _staticIncrement) & 0x00ffffff; // only use low order 3 bytes
            return new ObjectId(timestamp, _staticMachine, _staticPid, increment);
        }

        /// <summary>
        /// Generates a new ObjectId string with a unique value.
        /// </summary>
        /// <returns>The string value of the new generated ObjectId.</returns>
        public static string GenerateNewStringId() => GenerateNewId().ToString();

        /// <summary>
        /// Packs the components of an ObjectId into a byte array.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="machine">The machine hash.</param>
        /// <param name="pid">The PID.</param>
        /// <param name="increment">The increment.</param>
        /// <returns>A byte array.</returns>
        private static byte[] Pack(int timestamp, int machine, short pid, int increment)
        {
            if ((machine & 0xff000000) != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(machine), "The machine value must be between 0 and 16777215 (it must fit in 3 bytes).");
            }
            if ((increment & 0xff000000) != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(increment), "The increment value must be between 0 and 16777215 (it must fit in 3 bytes).");
            }

            var bytes = new byte[12];
            bytes[0] = (byte)(timestamp >> 24);
            bytes[1] = (byte)(timestamp >> 16);
            bytes[2] = (byte)(timestamp >> 8);
            bytes[3] = (byte)(timestamp);
            bytes[4] = (byte)(machine >> 16);
            bytes[5] = (byte)(machine >> 8);
            bytes[6] = (byte)(machine);
            bytes[7] = (byte)(pid >> 8);
            bytes[8] = (byte)(pid);
            bytes[9] = (byte)(increment >> 16);
            bytes[10] = (byte)(increment >> 8);
            bytes[11] = (byte)(increment);
            return bytes;
        }

        /// <summary>
        /// Parses a string and creates a new ObjectId.
        /// </summary>
        /// <param name="s">The string value.</param>
        /// <returns>A ObjectId.</returns>
        public static ObjectId Parse(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }
            if (s.Length != 24)
            {
                throw new ArgumentOutOfRangeException(nameof(s), "ObjectId string value must be 24 characters.");
            }
            return new ObjectId(ParseHexString(s));
        }

        /// <summary>
        /// Unpacks a byte array into the components of an ObjectId.
        /// </summary>
        /// <param name="bytes">A byte array.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="machine">The machine hash.</param>
        /// <param name="pid">The PID.</param>
        /// <param name="increment">The increment.</param>
        private static void Unpack(byte[] bytes, out int timestamp, out int machine, out short pid, out int increment)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            if (bytes.Length != 12)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), "Byte array must be 12 bytes long.");
            }
            timestamp = (bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3];
            machine = (bytes[4] << 16) + (bytes[5] << 8) + bytes[6];
            pid = (short)((bytes[7] << 8) + bytes[8]);
            increment = (bytes[9] << 16) + (bytes[10] << 8) + bytes[11];
        }

        // private static methods
        /// <summary>
        /// Gets the current process id.  This method exists because of how CAS operates on the call stack, checking
        /// for permissions before executing the method.  Hence, if we inlined this call, the calling method would not execute
        /// before throwing an exception requiring the try/catch at an even higher level that we don't necessarily control.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int GetCurrentProcessId()
        {
#if NET6_0_OR_GREATER
            return Environment.ProcessId;
#else
            return Process.GetCurrentProcess().Id;
#endif
        }

        private static int GetMachineHash()
        {
            var hostName = Environment.MachineName; // use instead of Dns.HostName so it will work offline
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(hostName));
            return (hash[0] << 16) + (hash[1] << 8) + hash[2]; // use first 3 bytes of hash
        }

        private static int GetTimestampFromDateTime(DateTime timestamp)
        {
            return (int)Math.Floor((ToUniversalTime(timestamp) - _unixEpoch).TotalSeconds);
        }

        // public methods
        /// <summary>
        /// Compares this ObjectId to another ObjectId.
        /// </summary>
        /// <param name="other">The other ObjectId.</param>
        /// <returns>A 32-bit signed integer that indicates whether this ObjectId is less than, equal to, or greather than the other.</returns>
        public int CompareTo(ObjectId other)
        {
            var r = _timestamp.CompareTo(other._timestamp);
            if (r != 0) { return r; }
            r = _machine.CompareTo(other._machine);
            if (r != 0) { return r; }
            r = _pid.CompareTo(other._pid);
            if (r != 0) { return r; }
            return _increment.CompareTo(other._increment);
        }

        /// <summary>
        /// Compares this ObjectId to another ObjectId.
        /// </summary>
        /// <param name="other">The other ObjectId.</param>
        /// <returns>True if the two ObjectIds are equal.</returns>
        public bool Equals(ObjectId other)
        {
            return
                _timestamp == other._timestamp &&
                _machine == other._machine &&
                _pid == other._pid &&
                _increment == other._increment;
        }

        /// <summary>
        /// Compares this ObjectId to another object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>True if the other object is an ObjectId and equal to this one.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is ObjectId id)
            {
                return Equals(id);
            }
            return false;
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            var hash = 17;
            hash = 37 * hash + _timestamp.GetHashCode();
            hash = 37 * hash + _machine.GetHashCode();
            hash = 37 * hash + _pid.GetHashCode();
            hash = 37 * hash + _increment.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Converts the ObjectId to a byte array.
        /// </summary>
        /// <returns>A byte array.</returns>
        public byte[] ToByteArray()
        {
            return Pack(_timestamp, _machine, _pid, _increment);
        }

        /// <summary>
        /// Returns a string representation of the value.
        /// </summary>
        /// <returns>A string representation of the value.</returns>
        public override string ToString()
        {
            return ToHexString(ToByteArray());
        }

        /// <summary>
        /// Parses a hex string into its equivalent byte array.
        /// </summary>
        /// <param name="s">The hex string to parse.</param>
        /// <returns>The byte equivalent of the hex string.</returns>
        private static byte[] ParseHexString(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }
            if (s.Length % 2 == 1)
            {
                throw new ArgumentException("The binary key cannot have an odd number of digits", nameof(s));
            }

            var arr = new byte[s.Length >> 1];
            for (var i = 0; i < s.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(s[i << 1]) << 4) + (GetHexVal(s[(i << 1) + 1])));
            }

            return arr;
        }

        /// <summary>
        /// Converts a byte array to a hex string.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <returns>A hex string.</returns>
        private static string ToHexString(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            var result = new char[bytes.Length * 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                var val = _lookup32[bytes[i]];
                result[2 * i] = (char)val;
                result[2 * i + 1] = (char)(val >> 16);
            }
            return new string(result);
        }

        /// <summary>
        /// Converts a DateTime to UTC (with special handling for MinValue and MaxValue).
        /// </summary>
        /// <param name="dateTime">A DateTime.</param>
        /// <returns>The DateTime in UTC.</returns>
        private static DateTime ToUniversalTime(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
            {
                return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
            }
            if (dateTime == DateTime.MaxValue)
            {
                return DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
            }
            return dateTime.ToUniversalTime();
        }

        private static int GetHexVal(char hex)
        {
            var val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
After:
namespace WeihanLi.Common;

/// <summary>
/// Represents an ObjectId
/// refer to https://github.com/tangxuehua/ecommon/blob/master/src/ECommon/Utilities/ObjectId.cs
/// </summary>
[Serializable]
public struct ObjectId : IComparable<ObjectId>, IEquatable<ObjectId>
{
    // private static fields
    private static readonly DateTime _unixEpoch;

    private static readonly ObjectId _emptyInstance = default;
    private static readonly int _staticMachine;
    private static readonly short _staticPid;
    private static int _staticIncrement; // high byte will be masked out when generating new ObjectId

    private static readonly uint[] _lookup32 = Enumerable.Range(0, 256).Select(i =>
             {
                 var s = i.ToString("x2");
                 return ((uint)s[0]) + ((uint)s[1] << 16);
             }).ToArray();

    // we're using 14 bytes instead of 12 to hold the ObjectId in memory but unlike a byte[] there is no additional object on the heap
    // the extra two bytes are not visible to anyone outside of this class and they buy us considerable simplification
    // an additional advantage of this representation is that it will serialize to JSON without any 64 bit overflow problems
    private readonly int _timestamp;

    private readonly int _machine;
    private readonly short _pid;
    private readonly int _increment;

    // static constructor
    static ObjectId()
    {
        _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        _staticMachine = GetMachineHash();
        _staticIncrement = (new Random()).Next();
        _staticPid = (short)GetCurrentProcessId();
    }

    // constructors
    /// <summary>
    /// Initializes a new instance of the ObjectId class.
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    public ObjectId(byte[] bytes)
    {
        if (bytes == null)
        {
            throw new ArgumentNullException(nameof(bytes));
        }
        Unpack(bytes, out _timestamp, out _machine, out _pid, out _increment);
    }

    /// <summary>
    /// Initializes a new instance of the ObjectId class.
    /// </summary>
    /// <param name="timestamp">The timestamp (expressed as a DateTime).</param>
    /// <param name="machine">The machine hash.</param>
    /// <param name="pid">The PID.</param>
    /// <param name="increment">The increment.</param>
    public ObjectId(DateTime timestamp, int machine, short pid, int increment)
        : this(GetTimestampFromDateTime(timestamp), machine, pid, increment)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ObjectId class.
    /// </summary>
    /// <param name="timestamp">The timestamp.</param>
    /// <param name="machine">The machine hash.</param>
    /// <param name="pid">The PID.</param>
    /// <param name="increment">The increment.</param>
    public ObjectId(int timestamp, int machine, short pid, int increment)
    {
        if ((machine & 0xff000000) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(machine), "The machine value must be between 0 and 16777215 (it must fit in 3 bytes).");
        }
        if ((increment & 0xff000000) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(increment), "The increment value must be between 0 and 16777215 (it must fit in 3 bytes).");
        }

        _timestamp = timestamp;
        _machine = machine;
        _pid = pid;
        _increment = increment;
    }

    /// <summary>
    /// Initializes a new instance of the ObjectId class.
    /// </summary>
    /// <param name="value">The value.</param>
    public ObjectId(string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        Unpack(ParseHexString(value), out _timestamp, out _machine, out _pid, out _increment);
    }

    // public static properties
    /// <summary>
    /// Gets an instance of ObjectId where the value is empty.
    /// </summary>
    public static ObjectId Empty => _emptyInstance;

    // public properties
    /// <summary>
    /// Gets the timestamp.
    /// </summary>
    public int Timestamp => _timestamp;

    /// <summary>
    /// Gets the machine.
    /// </summary>
    public int Machine => _machine;

    /// <summary>
    /// Gets the PID.
    /// </summary>
    public short Pid => _pid;

    /// <summary>
    /// Gets the increment.
    /// </summary>
    public int Increment => _increment;

    /// <summary>
    /// Gets the creation time (derived from the timestamp).
    /// </summary>
    public DateTime CreationTime => _unixEpoch.AddSeconds(_timestamp);

    // public operators
    /// <summary>
    /// Compares two ObjectIds.
    /// </summary>
    /// <param name="lhs">The first ObjectId.</param>
    /// <param name="rhs">The other ObjectId</param>
    /// <returns>True if the first ObjectId is less than the second ObjectId.</returns>
    public static bool operator <(ObjectId lhs, ObjectId rhs)
    {
        return lhs.CompareTo(rhs) < 0;
    }

    /// <summary>
    /// Compares two ObjectIds.
    /// </summary>
    /// <param name="lhs">The first ObjectId.</param>
    /// <param name="rhs">The other ObjectId</param>
    /// <returns>True if the first ObjectId is less than or equal to the second ObjectId.</returns>
    public static bool operator <=(ObjectId lhs, ObjectId rhs)
    {
        return lhs.CompareTo(rhs) <= 0;
    }

    /// <summary>
    /// Compares two ObjectIds.
    /// </summary>
    /// <param name="lhs">The first ObjectId.</param>
    /// <param name="rhs">The other ObjectId.</param>
    /// <returns>True if the two ObjectIds are equal.</returns>
    public static bool operator ==(ObjectId lhs, ObjectId rhs)
    {
        return lhs.Equals(rhs);
    }

    /// <summary>
    /// Compares two ObjectIds.
    /// </summary>
    /// <param name="lhs">The first ObjectId.</param>
    /// <param name="rhs">The other ObjectId.</param>
    /// <returns>True if the two ObjectIds are not equal.</returns>
    public static bool operator !=(ObjectId lhs, ObjectId rhs)
    {
        return !(lhs == rhs);
    }

    /// <summary>
    /// Compares two ObjectIds.
    /// </summary>
    /// <param name="lhs">The first ObjectId.</param>
    /// <param name="rhs">The other ObjectId</param>
    /// <returns>True if the first ObjectId is greater than or equal to the second ObjectId.</returns>
    public static bool operator >=(ObjectId lhs, ObjectId rhs)
    {
        return lhs.CompareTo(rhs) >= 0;
    }

    /// <summary>
    /// Compares two ObjectIds.
    /// </summary>
    /// <param name="lhs">The first ObjectId.</param>
    /// <param name="rhs">The other ObjectId</param>
    /// <returns>True if the first ObjectId is greater than the second ObjectId.</returns>
    public static bool operator >(ObjectId lhs, ObjectId rhs)
    {
        return lhs.CompareTo(rhs) > 0;
    }

    // public static methods
    /// <summary>
    /// Generates a new ObjectId with a unique value.
    /// </summary>
    /// <returns>An ObjectId.</returns>
    public static ObjectId GenerateNewId() => GenerateNewId(GetTimestampFromDateTime(DateTime.UtcNow));

    /// <summary>
    /// Generates a new ObjectId with a unique value (with the timestamp component based on a given DateTime).
    /// </summary>
    /// <param name="timestamp">The timestamp component (expressed as a DateTime).</param>
    /// <returns>An ObjectId.</returns>
    public static ObjectId GenerateNewId(DateTime timestamp) => GenerateNewId(GetTimestampFromDateTime(timestamp));

    /// <summary>
    /// Generates a new ObjectId with a unique value (with the given timestamp).
    /// </summary>
    /// <param name="timestamp">The timestamp component.</param>
    /// <returns>An ObjectId.</returns>
    private static ObjectId GenerateNewId(int timestamp)
    {
        var increment = Interlocked.Increment(ref _staticIncrement) & 0x00ffffff; // only use low order 3 bytes
        return new ObjectId(timestamp, _staticMachine, _staticPid, increment);
    }

    /// <summary>
    /// Generates a new ObjectId string with a unique value.
    /// </summary>
    /// <returns>The string value of the new generated ObjectId.</returns>
    public static string GenerateNewStringId() => GenerateNewId().ToString();

    /// <summary>
    /// Packs the components of an ObjectId into a byte array.
    /// </summary>
    /// <param name="timestamp">The timestamp.</param>
    /// <param name="machine">The machine hash.</param>
    /// <param name="pid">The PID.</param>
    /// <param name="increment">The increment.</param>
    /// <returns>A byte array.</returns>
    private static byte[] Pack(int timestamp, int machine, short pid, int increment)
    {
        if ((machine & 0xff000000) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(machine), "The machine value must be between 0 and 16777215 (it must fit in 3 bytes).");
        }
        if ((increment & 0xff000000) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(increment), "The increment value must be between 0 and 16777215 (it must fit in 3 bytes).");
        }

        var bytes = new byte[12];
        bytes[0] = (byte)(timestamp >> 24);
        bytes[1] = (byte)(timestamp >> 16);
        bytes[2] = (byte)(timestamp >> 8);
        bytes[3] = (byte)(timestamp);
        bytes[4] = (byte)(machine >> 16);
        bytes[5] = (byte)(machine >> 8);
        bytes[6] = (byte)(machine);
        bytes[7] = (byte)(pid >> 8);
        bytes[8] = (byte)(pid);
        bytes[9] = (byte)(increment >> 16);
        bytes[10] = (byte)(increment >> 8);
        bytes[11] = (byte)(increment);
        return bytes;
    }

    /// <summary>
    /// Parses a string and creates a new ObjectId.
    /// </summary>
    /// <param name="s">The string value.</param>
    /// <returns>A ObjectId.</returns>
    public static ObjectId Parse(string s)
    {
        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }
        if (s.Length != 24)
        {
            throw new ArgumentOutOfRangeException(nameof(s), "ObjectId string value must be 24 characters.");
        }
        return new ObjectId(ParseHexString(s));
    }

    /// <summary>
    /// Unpacks a byte array into the components of an ObjectId.
    /// </summary>
    /// <param name="bytes">A byte array.</param>
    /// <param name="timestamp">The timestamp.</param>
    /// <param name="machine">The machine hash.</param>
    /// <param name="pid">The PID.</param>
    /// <param name="increment">The increment.</param>
    private static void Unpack(byte[] bytes, out int timestamp, out int machine, out short pid, out int increment)
    {
        if (bytes == null)
        {
            throw new ArgumentNullException(nameof(bytes));
        }
        if (bytes.Length != 12)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes), "Byte array must be 12 bytes long.");
        }
        timestamp = (bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3];
        machine = (bytes[4] << 16) + (bytes[5] << 8) + bytes[6];
        pid = (short)((bytes[7] << 8) + bytes[8]);
        increment = (bytes[9] << 16) + (bytes[10] << 8) + bytes[11];
    }

    // private static methods
    /// <summary>
    /// Gets the current process id.  This method exists because of how CAS operates on the call stack, checking
    /// for permissions before executing the method.  Hence, if we inlined this call, the calling method would not execute
    /// before throwing an exception requiring the try/catch at an even higher level that we don't necessarily control.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int GetCurrentProcessId()
    {
#if NET6_0_OR_GREATER
        return Environment.ProcessId;
#else
            return Process.GetCurrentProcess().Id;
#endif
    }

    private static int GetMachineHash()
    {
        var hostName = Environment.MachineName; // use instead of Dns.HostName so it will work offline
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(hostName));
        return (hash[0] << 16) + (hash[1] << 8) + hash[2]; // use first 3 bytes of hash
    }

    private static int GetTimestampFromDateTime(DateTime timestamp)
    {
        return (int)Math.Floor((ToUniversalTime(timestamp) - _unixEpoch).TotalSeconds);
    }

    // public methods
    /// <summary>
    /// Compares this ObjectId to another ObjectId.
    /// </summary>
    /// <param name="other">The other ObjectId.</param>
    /// <returns>A 32-bit signed integer that indicates whether this ObjectId is less than, equal to, or greather than the other.</returns>
    public int CompareTo(ObjectId other)
    {
        var r = _timestamp.CompareTo(other._timestamp);
        if (r != 0) { return r; }
        r = _machine.CompareTo(other._machine);
        if (r != 0) { return r; }
        r = _pid.CompareTo(other._pid);
        if (r != 0) { return r; }
        return _increment.CompareTo(other._increment);
    }

    /// <summary>
    /// Compares this ObjectId to another ObjectId.
    /// </summary>
    /// <param name="other">The other ObjectId.</param>
    /// <returns>True if the two ObjectIds are equal.</returns>
    public bool Equals(ObjectId other)
    {
        return
            _timestamp == other._timestamp &&
            _machine == other._machine &&
            _pid == other._pid &&
            _increment == other._increment;
    }

    /// <summary>
    /// Compares this ObjectId to another object.
    /// </summary>
    /// <param name="obj">The other object.</param>
    /// <returns>True if the other object is an ObjectId and equal to this one.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is ObjectId id)
        {
            return Equals(id);
        }
        return false;
    }

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        var hash = 17;
        hash = 37 * hash + _timestamp.GetHashCode();
        hash = 37 * hash + _machine.GetHashCode();
        hash = 37 * hash + _pid.GetHashCode();
        hash = 37 * hash + _increment.GetHashCode();
        return hash;
    }

    /// <summary>
    /// Converts the ObjectId to a byte array.
    /// </summary>
    /// <returns>A byte array.</returns>
    public byte[] ToByteArray()
    {
        return Pack(_timestamp, _machine, _pid, _increment);
    }

    /// <summary>
    /// Returns a string representation of the value.
    /// </summary>
    /// <returns>A string representation of the value.</returns>
    public override string ToString()
    {
        return ToHexString(ToByteArray());
    }

    /// <summary>
    /// Parses a hex string into its equivalent byte array.
    /// </summary>
    /// <param name="s">The hex string to parse.</param>
    /// <returns>The byte equivalent of the hex string.</returns>
    private static byte[] ParseHexString(string s)
    {
        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }
        if (s.Length % 2 == 1)
        {
            throw new ArgumentException("The binary key cannot have an odd number of digits", nameof(s));
        }

        var arr = new byte[s.Length >> 1];
        for (var i = 0; i < s.Length >> 1; ++i)
        {
            arr[i] = (byte)((GetHexVal(s[i << 1]) << 4) + (GetHexVal(s[(i << 1) + 1])));
        }

        return arr;
    }

    /// <summary>
    /// Converts a byte array to a hex string.
    /// </summary>
    /// <param name="bytes">The byte array.</param>
    /// <returns>A hex string.</returns>
    private static string ToHexString(byte[] bytes)
    {
        if (bytes == null)
        {
            throw new ArgumentNullException(nameof(bytes));
        }
        var result = new char[bytes.Length * 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            var val = _lookup32[bytes[i]];
            result[2 * i] = (char)val;
            result[2 * i + 1] = (char)(val >> 16);
        }
        return new string(result);
    }

    /// <summary>
    /// Converts a DateTime to UTC (with special handling for MinValue and MaxValue).
    /// </summary>
    /// <param name="dateTime">A DateTime.</param>
    /// <returns>The DateTime in UTC.</returns>
    private static DateTime ToUniversalTime(DateTime dateTime)
    {
        if (dateTime == DateTime.MinValue)
        {
            return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
        }
        if (dateTime == DateTime.MaxValue)
        {
            return DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
        }
        return dateTime.ToUniversalTime();
    }

    private static int GetHexVal(char hex)
    {
        var val = (int)hex;
        //For uppercase A-F letters:
        //return val - (val < 58 ? 48 : 55);
        //For lowercase a-f letters:
        //return val - (val < 58 ? 48 : 87);
        //Or the two combined, but a bit slower:
        return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
*/
namespace WeihanLi.Common;

/// <summary>
/// Represents an ObjectId
/// refer to https://github.com/tangxuehua/ecommon/blob/master/src/ECommon/Utilities/ObjectId.cs
/// </summary>
[Serializable]
public struct ObjectId : IComparable<ObjectId>, IEquatable<ObjectId>
{
    // private static fields
    private static readonly DateTime _unixEpoch;

    private static readonly ObjectId _emptyInstance = default;
    private static readonly int _staticMachine;
    private static readonly short _staticPid;
    private static int _staticIncrement; // high byte will be masked out when generating new ObjectId

    private static readonly uint[] _lookup32 = Enumerable.Range(0, 256).Select(i =>
             {
                 var s = i.ToString("x2");
                 return ((uint)s[0]) + ((uint)s[1] << 16);
             }).ToArray();

    // we're using 14 bytes instead of 12 to hold the ObjectId in memory but unlike a byte[] there is no additional object on the heap
    // the extra two bytes are not visible to anyone outside of this class and they buy us considerable simplification
    // an additional advantage of this representation is that it will serialize to JSON without any 64 bit overflow problems
    private readonly int _timestamp;

    private readonly int _machine;
    private readonly short _pid;
    private readonly int _increment;

    // static constructor
    static ObjectId()
    {
        _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        _staticMachine = GetMachineHash();
        _staticIncrement = (new Random()).Next();
        _staticPid = (short)GetCurrentProcessId();
    }

    // constructors
    /// <summary>
    /// Initializes a new instance of the ObjectId class.
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    public ObjectId(byte[] bytes)
    {
        if (bytes == null)
        {
            throw new ArgumentNullException(nameof(bytes));
        }
        Unpack(bytes, out _timestamp, out _machine, out _pid, out _increment);
    }

    /// <summary>
    /// Initializes a new instance of the ObjectId class.
    /// </summary>
    /// <param name="timestamp">The timestamp (expressed as a DateTime).</param>
    /// <param name="machine">The machine hash.</param>
    /// <param name="pid">The PID.</param>
    /// <param name="increment">The increment.</param>
    public ObjectId(DateTime timestamp, int machine, short pid, int increment)
        : this(GetTimestampFromDateTime(timestamp), machine, pid, increment)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ObjectId class.
    /// </summary>
    /// <param name="timestamp">The timestamp.</param>
    /// <param name="machine">The machine hash.</param>
    /// <param name="pid">The PID.</param>
    /// <param name="increment">The increment.</param>
    public ObjectId(int timestamp, int machine, short pid, int increment)
    {
        if ((machine & 0xff000000) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(machine), "The machine value must be between 0 and 16777215 (it must fit in 3 bytes).");
        }
        if ((increment & 0xff000000) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(increment), "The increment value must be between 0 and 16777215 (it must fit in 3 bytes).");
        }

        _timestamp = timestamp;
        _machine = machine;
        _pid = pid;
        _increment = increment;
    }

    /// <summary>
    /// Initializes a new instance of the ObjectId class.
    /// </summary>
    /// <param name="value">The value.</param>
    public ObjectId(string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        Unpack(ParseHexString(value), out _timestamp, out _machine, out _pid, out _increment);
    }

    // public static properties
    /// <summary>
    /// Gets an instance of ObjectId where the value is empty.
    /// </summary>
    public static ObjectId Empty => _emptyInstance;

    // public properties
    /// <summary>
    /// Gets the timestamp.
    /// </summary>
    public int Timestamp => _timestamp;

    /// <summary>
    /// Gets the machine.
    /// </summary>
    public int Machine => _machine;

    /// <summary>
    /// Gets the PID.
    /// </summary>
    public short Pid => _pid;

    /// <summary>
    /// Gets the increment.
    /// </summary>
    public int Increment => _increment;

    /// <summary>
    /// Gets the creation time (derived from the timestamp).
    /// </summary>
    public DateTime CreationTime => _unixEpoch.AddSeconds(_timestamp);

    // public operators
    /// <summary>
    /// Compares two ObjectIds.
    /// </summary>
    /// <param name="lhs">The first ObjectId.</param>
    /// <param name="rhs">The other ObjectId</param>
    /// <returns>True if the first ObjectId is less than the second ObjectId.</returns>
    public static bool operator <(ObjectId lhs, ObjectId rhs)
    {
        return lhs.CompareTo(rhs) < 0;
    }

    /// <summary>
    /// Compares two ObjectIds.
    /// </summary>
    /// <param name="lhs">The first ObjectId.</param>
    /// <param name="rhs">The other ObjectId</param>
    /// <returns>True if the first ObjectId is less than or equal to the second ObjectId.</returns>
    public static bool operator <=(ObjectId lhs, ObjectId rhs)
    {
        return lhs.CompareTo(rhs) <= 0;
    }

    /// <summary>
    /// Compares two ObjectIds.
    /// </summary>
    /// <param name="lhs">The first ObjectId.</param>
    /// <param name="rhs">The other ObjectId.</param>
    /// <returns>True if the two ObjectIds are equal.</returns>
    public static bool operator ==(ObjectId lhs, ObjectId rhs)
    {
        return lhs.Equals(rhs);
    }

    /// <summary>
    /// Compares two ObjectIds.
    /// </summary>
    /// <param name="lhs">The first ObjectId.</param>
    /// <param name="rhs">The other ObjectId.</param>
    /// <returns>True if the two ObjectIds are not equal.</returns>
    public static bool operator !=(ObjectId lhs, ObjectId rhs)
    {
        return !(lhs == rhs);
    }

    /// <summary>
    /// Compares two ObjectIds.
    /// </summary>
    /// <param name="lhs">The first ObjectId.</param>
    /// <param name="rhs">The other ObjectId</param>
    /// <returns>True if the first ObjectId is greater than or equal to the second ObjectId.</returns>
    public static bool operator >=(ObjectId lhs, ObjectId rhs)
    {
        return lhs.CompareTo(rhs) >= 0;
    }

    /// <summary>
    /// Compares two ObjectIds.
    /// </summary>
    /// <param name="lhs">The first ObjectId.</param>
    /// <param name="rhs">The other ObjectId</param>
    /// <returns>True if the first ObjectId is greater than the second ObjectId.</returns>
    public static bool operator >(ObjectId lhs, ObjectId rhs)
    {
        return lhs.CompareTo(rhs) > 0;
    }

    // public static methods
    /// <summary>
    /// Generates a new ObjectId with a unique value.
    /// </summary>
    /// <returns>An ObjectId.</returns>
    public static ObjectId GenerateNewId() => GenerateNewId(GetTimestampFromDateTime(DateTime.UtcNow));

    /// <summary>
    /// Generates a new ObjectId with a unique value (with the timestamp component based on a given DateTime).
    /// </summary>
    /// <param name="timestamp">The timestamp component (expressed as a DateTime).</param>
    /// <returns>An ObjectId.</returns>
    public static ObjectId GenerateNewId(DateTime timestamp) => GenerateNewId(GetTimestampFromDateTime(timestamp));

    /// <summary>
    /// Generates a new ObjectId with a unique value (with the given timestamp).
    /// </summary>
    /// <param name="timestamp">The timestamp component.</param>
    /// <returns>An ObjectId.</returns>
    private static ObjectId GenerateNewId(int timestamp)
    {
        var increment = Interlocked.Increment(ref _staticIncrement) & 0x00ffffff; // only use low order 3 bytes
        return new ObjectId(timestamp, _staticMachine, _staticPid, increment);
    }

    /// <summary>
    /// Generates a new ObjectId string with a unique value.
    /// </summary>
    /// <returns>The string value of the new generated ObjectId.</returns>
    public static string GenerateNewStringId() => GenerateNewId().ToString();

    /// <summary>
    /// Packs the components of an ObjectId into a byte array.
    /// </summary>
    /// <param name="timestamp">The timestamp.</param>
    /// <param name="machine">The machine hash.</param>
    /// <param name="pid">The PID.</param>
    /// <param name="increment">The increment.</param>
    /// <returns>A byte array.</returns>
    private static byte[] Pack(int timestamp, int machine, short pid, int increment)
    {
        if ((machine & 0xff000000) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(machine), "The machine value must be between 0 and 16777215 (it must fit in 3 bytes).");
        }
        if ((increment & 0xff000000) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(increment), "The increment value must be between 0 and 16777215 (it must fit in 3 bytes).");
        }

        var bytes = new byte[12];
        bytes[0] = (byte)(timestamp >> 24);
        bytes[1] = (byte)(timestamp >> 16);
        bytes[2] = (byte)(timestamp >> 8);
        bytes[3] = (byte)(timestamp);
        bytes[4] = (byte)(machine >> 16);
        bytes[5] = (byte)(machine >> 8);
        bytes[6] = (byte)(machine);
        bytes[7] = (byte)(pid >> 8);
        bytes[8] = (byte)(pid);
        bytes[9] = (byte)(increment >> 16);
        bytes[10] = (byte)(increment >> 8);
        bytes[11] = (byte)(increment);
        return bytes;
    }

    /// <summary>
    /// Parses a string and creates a new ObjectId.
    /// </summary>
    /// <param name="s">The string value.</param>
    /// <returns>A ObjectId.</returns>
    public static ObjectId Parse(string s)
    {
        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }
        if (s.Length != 24)
        {
            throw new ArgumentOutOfRangeException(nameof(s), "ObjectId string value must be 24 characters.");
        }
        return new ObjectId(ParseHexString(s));
    }

    /// <summary>
    /// Unpacks a byte array into the components of an ObjectId.
    /// </summary>
    /// <param name="bytes">A byte array.</param>
    /// <param name="timestamp">The timestamp.</param>
    /// <param name="machine">The machine hash.</param>
    /// <param name="pid">The PID.</param>
    /// <param name="increment">The increment.</param>
    private static void Unpack(byte[] bytes, out int timestamp, out int machine, out short pid, out int increment)
    {
        if (bytes == null)
        {
            throw new ArgumentNullException(nameof(bytes));
        }
        if (bytes.Length != 12)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes), "Byte array must be 12 bytes long.");
        }
        timestamp = (bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3];
        machine = (bytes[4] << 16) + (bytes[5] << 8) + bytes[6];
        pid = (short)((bytes[7] << 8) + bytes[8]);
        increment = (bytes[9] << 16) + (bytes[10] << 8) + bytes[11];
    }

    // private static methods
    /// <summary>
    /// Gets the current process id.  This method exists because of how CAS operates on the call stack, checking
    /// for permissions before executing the method.  Hence, if we inlined this call, the calling method would not execute
    /// before throwing an exception requiring the try/catch at an even higher level that we don't necessarily control.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int GetCurrentProcessId()
    {
#if NET6_0_OR_GREATER
            return Environment.ProcessId;
#else
        return Process.GetCurrentProcess().Id;
#endif
    }

    private static int GetMachineHash()
    {
        var hostName = Environment.MachineName; // use instead of Dns.HostName so it will work offline
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(hostName));
        return (hash[0] << 16) + (hash[1] << 8) + hash[2]; // use first 3 bytes of hash
    }

    private static int GetTimestampFromDateTime(DateTime timestamp)
    {
        return (int)Math.Floor((ToUniversalTime(timestamp) - _unixEpoch).TotalSeconds);
    }

    // public methods
    /// <summary>
    /// Compares this ObjectId to another ObjectId.
    /// </summary>
    /// <param name="other">The other ObjectId.</param>
    /// <returns>A 32-bit signed integer that indicates whether this ObjectId is less than, equal to, or greather than the other.</returns>
    public int CompareTo(ObjectId other)
    {
        var r = _timestamp.CompareTo(other._timestamp);
        if (r != 0) { return r; }
        r = _machine.CompareTo(other._machine);
        if (r != 0) { return r; }
        r = _pid.CompareTo(other._pid);
        if (r != 0) { return r; }
        return _increment.CompareTo(other._increment);
    }

    /// <summary>
    /// Compares this ObjectId to another ObjectId.
    /// </summary>
    /// <param name="other">The other ObjectId.</param>
    /// <returns>True if the two ObjectIds are equal.</returns>
    public bool Equals(ObjectId other)
    {
        return
            _timestamp == other._timestamp &&
            _machine == other._machine &&
            _pid == other._pid &&
            _increment == other._increment;
    }

    /// <summary>
    /// Compares this ObjectId to another object.
    /// </summary>
    /// <param name="obj">The other object.</param>
    /// <returns>True if the other object is an ObjectId and equal to this one.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is ObjectId id)
        {
            return Equals(id);
        }
        return false;
    }

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        var hash = 17;
        hash = 37 * hash + _timestamp.GetHashCode();
        hash = 37 * hash + _machine.GetHashCode();
        hash = 37 * hash + _pid.GetHashCode();
        hash = 37 * hash + _increment.GetHashCode();
        return hash;
    }

    /// <summary>
    /// Converts the ObjectId to a byte array.
    /// </summary>
    /// <returns>A byte array.</returns>
    public byte[] ToByteArray()
    {
        return Pack(_timestamp, _machine, _pid, _increment);
    }

    /// <summary>
    /// Returns a string representation of the value.
    /// </summary>
    /// <returns>A string representation of the value.</returns>
    public override string ToString()
    {
        return ToHexString(ToByteArray());
    }

    /// <summary>
    /// Parses a hex string into its equivalent byte array.
    /// </summary>
    /// <param name="s">The hex string to parse.</param>
    /// <returns>The byte equivalent of the hex string.</returns>
    private static byte[] ParseHexString(string s)
    {
        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }
        if (s.Length % 2 == 1)
        {
            throw new ArgumentException("The binary key cannot have an odd number of digits", nameof(s));
        }

        var arr = new byte[s.Length >> 1];
        for (var i = 0; i < s.Length >> 1; ++i)
        {
            arr[i] = (byte)((GetHexVal(s[i << 1]) << 4) + (GetHexVal(s[(i << 1) + 1])));
        }

        return arr;
    }

    /// <summary>
    /// Converts a byte array to a hex string.
    /// </summary>
    /// <param name="bytes">The byte array.</param>
    /// <returns>A hex string.</returns>
    private static string ToHexString(byte[] bytes)
    {
        if (bytes == null)
        {
            throw new ArgumentNullException(nameof(bytes));
        }
        var result = new char[bytes.Length * 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            var val = _lookup32[bytes[i]];
            result[2 * i] = (char)val;
            result[2 * i + 1] = (char)(val >> 16);
        }
        return new string(result);
    }

    /// <summary>
    /// Converts a DateTime to UTC (with special handling for MinValue and MaxValue).
    /// </summary>
    /// <param name="dateTime">A DateTime.</param>
    /// <returns>The DateTime in UTC.</returns>
    private static DateTime ToUniversalTime(DateTime dateTime)
    {
        if (dateTime == DateTime.MinValue)
        {
            return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
        }
        if (dateTime == DateTime.MaxValue)
        {
            return DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
        }
        return dateTime.ToUniversalTime();
    }

    private static int GetHexVal(char hex)
    {
        var val = (int)hex;
        //For uppercase A-F letters:
        //return val - (val < 58 ? 48 : 55);
        //For lowercase a-f letters:
        //return val - (val < 58 ? 48 : 87);
        //Or the two combined, but a bit slower:
        return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
    }
}
