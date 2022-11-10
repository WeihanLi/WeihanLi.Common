using System.Net.NetworkInformation;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common;

/// <summary>
/// IdGenerator
/// </summary>
public interface IIdGenerator
{
    /// <summary>
    /// Generate a new id
    /// </summary>
    /// <returns>new id</returns>
    string NewId();
}

/// <summary>
/// IdGenerator based on Guid
/// </summary>
public sealed class GuidIdGenerator : IIdGenerator
{
    public static readonly GuidIdGenerator Instance = new();

    public string NewId() => Guid.NewGuid().ToString("N");
}

public sealed class SequentialGuidIdGenerator : IIdGenerator
{
    private readonly SequentialGuidType _sequentialGuidType;

    public SequentialGuidIdGenerator(SequentialGuidType sequentialGuidType)
    {
        _sequentialGuidType = sequentialGuidType;
    }

    public string NewId() => SequentialGuidGenerator.Create(_sequentialGuidType).ToString("N");
}

/// <summary>
/// Snowflake IdGenerator
/// WARNING: NotImplemented, do not use
/// </summary>
public sealed class SnowflakeIdGenerator : IIdGenerator
{
    public string NewId()
    {
        return SnowflakeId.Default().NextId().ToString();
    }
}

/// <summary>
/// SnowflakeId
/// https://raw.githubusercontent.com/dotnetcore/CAP/51534f541049d369ecf7376a5d6e0393061d3f42/src/DotNetCore.CAP/Internal/SnowflakeId.cs
/// </summary>
public class SnowflakeId
{
    /// <summary>
    /// Start time 2010-11-04 09:42:54
    /// </summary>
    private const long Twepoch = 1288834974657L;

    /// <summary>
    /// The number of bits occupied by workerId
    /// </summary>
    private const int WorkerIdBits = 10;

    /// <summary>
    /// The number of bits occupied by timestamp
    /// </summary>
    private const int TimestampBits = 41;

    /// <summary>
    /// The number of bits occupied by sequence
    /// </summary>
    private const int SequenceBits = 12;

    /// <summary>
    /// Maximum supported machine id, the result is 1023
    /// </summary>
    private const int MaxWorkerId = ~(-1 << WorkerIdBits);

    /// <summary>
    /// mask that help to extract timestamp and sequence from a long
    /// </summary>
    private const long TimestampAndSequenceMask = ~(-1L << (TimestampBits + SequenceBits));

    /// <summary>
    /// business meaning: machine ID (0 ~ 1023)
    /// actual layout in memory:
    /// highest 1 bit: 0
    /// middle 10 bit: workerId
    /// lowest 53 bit: all 0
    /// </summary>
    private long _workerId { get; }

    /// <summary>
    /// timestamp and sequence mix in one Long
    /// highest 11 bit: not used
    /// middle  41 bit: timestamp
    /// lowest  12 bit: sequence
    /// </summary>
    private long _timestampAndSequence;

    private static volatile SnowflakeId? _snowflakeId;

    private static readonly object SLock = new();
    private readonly object _lock = new();

    public SnowflakeId(long workerId)
    {
        InitTimestampAndSequence();
        // sanity check for workerId
        if (workerId > MaxWorkerId || workerId < 0)
            throw new ArgumentException($"worker Id can't be greater than {MaxWorkerId} or less than 0");

        _workerId = workerId << (TimestampBits + SequenceBits);
    }

    public static SnowflakeId Default()
    {
        if (_snowflakeId != null)
        {
            return _snowflakeId;
        }

        lock (SLock)
        {
            if (_snowflakeId != null)
            {
                return _snowflakeId;
            }

            if (!long.TryParse(Environment.GetEnvironmentVariable("WORKERID"), out var workerId))
            {
                workerId = Util.GenerateWorkerId(MaxWorkerId);
            }

            return _snowflakeId = new SnowflakeId(workerId);
        }
    }

    public virtual long NextId()
    {
        lock (_lock)
        {
            WaitIfNecessary();
            var timestampWithSequence = _timestampAndSequence & TimestampAndSequenceMask;
            return _workerId | timestampWithSequence;
        }
    }

    /// <summary>
    /// init first timestamp and sequence immediately
    /// </summary>
    private void InitTimestampAndSequence()
    {
        var timestamp = GetNewestTimestamp();
        var timestampWithSequence = timestamp << SequenceBits;
        _timestampAndSequence = timestampWithSequence;
    }

    /// <summary>
    /// block current thread if the QPS of acquiring UUID is too high
    /// that current sequence space is exhausted
    /// </summary>
    private void WaitIfNecessary()
    {
        var currentWithSequence = ++_timestampAndSequence;
        var current = currentWithSequence >> SequenceBits;
        var newest = GetNewestTimestamp();

        if (current >= newest)
        {
            Thread.Sleep(5);
        }
    }

    /// <summary>
    /// get newest timestamp relative to twepoch
    /// </summary>
    /// <returns></returns>
    private long GetNewestTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Twepoch;
    }
}

internal static class Util
{
    /// <summary>
    /// auto generate workerId, try using mac first, if failed, then randomly generate one
    /// </summary>
    /// <returns>workerId</returns>
    public static long GenerateWorkerId(int maxWorkerId)
    {
        try
        {
            return GenerateWorkerIdBaseOnMac();
        }
        catch
        {
            return GenerateRandomWorkerId(maxWorkerId);
        }
    }

    /// <summary>
    /// use lowest 10 bit of available MAC as workerId
    /// </summary>
    /// <returns>workerId</returns>
    private static long GenerateWorkerIdBaseOnMac()
    {
        var nics = NetworkInterface.GetAllNetworkInterfaces();
        //exclude virtual and Loopback
        var firstUpInterface = nics.OrderByDescending(x => x.Speed).FirstOrDefault(x => !x.Description.Contains("Virtual") && x.NetworkInterfaceType != NetworkInterfaceType.Loopback && x.OperationalStatus == OperationalStatus.Up);
        if (firstUpInterface == null)
        {
            throw new Exception("no available mac found");
        }
        var address = firstUpInterface.GetPhysicalAddress();
        var mac = address.GetAddressBytes();

        return ((mac[4] & 0B11) << 8) | (mac[5] & 0xFF);
    }

    /// <summary>
    /// randomly generate one as workerId
    /// </summary>
    /// <returns></returns>
    private static long GenerateRandomWorkerId(int maxWorkerId)
    {
        return new Random().Next(maxWorkerId + 1);
    }
}
