﻿using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test;

public class IdGeneratorTest
{
    [Fact]
    public void GuidIdTest()
    {
        var id = GuidIdGenerator.Instance.NewId();
        Assert.NotEqual(id, GuidIdGenerator.Instance.NewId());
    }

    [Fact]
    public void SequentialGuidIdTest()
    {
        var idGenerator = new SequentialGuidIdGenerator(SequentialGuidType.SequentialAsString);
        var id = idGenerator.NewId();
        Assert.NotEqual(id, idGenerator.NewId());
    }

    [Fact]
    public void SnowflakeIdTest()
    {
        var idGenerator = new SnowflakeIdGenerator();
        var id = idGenerator.NewId();
        Assert.NotEqual(id, idGenerator.NewId());
    }
}
