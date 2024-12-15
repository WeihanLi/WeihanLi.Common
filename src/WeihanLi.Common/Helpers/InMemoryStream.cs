// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Collections.Concurrent;
using WeihanLi.Common.Models;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public class StreamMessage<T>
{
    public required T Id { get; init; }
    public DateTimeOffset Timestamp { get; set; }
    public Dictionary<string, string> Fields { get => field; set => field = Guard.NotNull(value); } = [];
    public Dictionary<string, object> Properties { get => field; set => field = Guard.NotNull(value); } = [];
}

public class StreamInfo<T>
{
    public required T MinId { get; set; }
    public DateTimeOffset MinTimestamp { get; set; }
    public required T MaxId { get; set; }
    public DateTimeOffset MaxTimestamp { get; set; }
    public int Count { get; set; }
}

public class StreamGroupInfo<T>
{
    public required string GroupName { get; set; }
    public required T Offset { get; set; }
}

public interface IStream<T>
{
    string StreamName { get; }

    Task AddAsync(T id, Dictionary<string, string> fields, DateTimeOffset? timestamp = null, Dictionary<string, object>? properties = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<StreamMessage<T>> FetchAsync(T lastId, int count, Ordering order = default, CancellationToken cancellationToken = default);

    Task<int> CountAsync(T? min = default, T? max = default, RangeInclusion inclusion = default, CancellationToken cancellationToken = default);

    Task<StreamInfo<T>> InfoAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<StreamGroupInfo<T>>> GroupsAsync(CancellationToken cancellationToken = default);
    Task<StreamGroupInfo<T>?> GroupInfoAsync(string groupName, CancellationToken cancellationToken = default);
    Task AddGroupAsync(string groupName, T offset, CancellationToken cancellationToken = default);
    Task<bool> RemoveGroupAsync(string groupName, CancellationToken cancellationToken = default);
    Task AckAsync(string groupName, T id, CancellationToken cancellationToken = default);
}

public sealed class InMemoryStream<T>(string name, IComparer<T>? comparer = null) : IStream<T>
{
    private readonly PriorityQueue<StreamMessage<T>, T> _messages = new(comparer);
    private readonly ConcurrentDictionary<string, StreamGroupInfo<T>> _groups = new();

    public string StreamName => name;

    public Task AckAsync(string groupName, T id, CancellationToken cancellationToken = default)
    {
        if (!_groups.TryGetValue(groupName, out var groupInfo))
        {
            throw new InvalidOperationException($"Group [{groupName}] not exists");
        }

        groupInfo.Offset = id;
        return Task.CompletedTask;
    }

    public Task AddAsync(T id, Dictionary<string, string> fields, DateTimeOffset? timestamp = null, Dictionary<string, object>? properties = null, CancellationToken cancellationToken = default)
    {
        var message = new StreamMessage<T>
        {
            Id = id,
            Fields = fields,
            Timestamp = timestamp ?? DateTimeOffset.Now,
            Properties = properties ?? new()
        };
        _messages.Enqueue(message, id);

        return Task.CompletedTask;
    }

    public Task AddGroupAsync(string groupName, T offset, CancellationToken cancellationToken = default)
    {
        if (_groups.ContainsKey(groupName))
        {
            throw new InvalidOperationException($"Group [{groupName}] not exists");
        }

        _groups[groupName] = new StreamGroupInfo<T>()
        {
            GroupName = groupName,
            Offset = offset
        };
        return Task.CompletedTask;
    }

    public Task<int> CountAsync(T? min = default, T? max = default, RangeInclusion inclusion = default, CancellationToken cancellationToken = default)
    {
        // TODO: support min/max filter
        return _messages.Count.WrapTask();
    }

    public IAsyncEnumerable<StreamMessage<T>> FetchAsync(T lastId, int count, Ordering order = Ordering.Ascending, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<StreamGroupInfo<T>?> GroupInfoAsync(string groupName, CancellationToken cancellationToken = default)
    {
        if (_groups.TryGetValue(groupName, out var groupInfo))
        {
            return Task.FromResult<StreamGroupInfo<T>?>(groupInfo);
        }

        return Task.FromResult<StreamGroupInfo<T>?>(null);
    }

    public Task<IReadOnlyCollection<StreamGroupInfo<T>>> GroupsAsync(CancellationToken cancellationToken = default)
    {
        return _groups.Values.AsReadOnly().WrapTask();
    }

    public Task<StreamInfo<T>> InfoAsync(CancellationToken cancellationToken = default)
    {
        var streamInfo = new StreamInfo<T>
        {
            // TODO: update min/max from messages
            MinId = default,
            MaxId = default,
            Count = _messages.Count
        };
        return streamInfo.WrapTask();
    }

    public Task<bool> RemoveGroupAsync(string groupName, CancellationToken cancellationToken = default)
    {
        return _groups.TryRemove(groupName, out _).WrapTask();
    }
}
