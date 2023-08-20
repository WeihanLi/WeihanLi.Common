using Microsoft.Extensions.Configuration;
using Xunit;

namespace WeihanLi.Common.Test.ExtensionsTest;

public class ConfigurationExtensionTest
{
    [Theory]
    [InlineData("true")]
    [InlineData("false")]
    [InlineData("false-value")]
    [InlineData(null)]
    public void FeatureFlagTest(string value)
    {
        var featureName = "TestFeature";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new[]
            {
                    new KeyValuePair<string, string?>($"{ConfigurationExtension.FeatureFlagsSectionName}:{featureName}", value)
            })
            .Build();

        var parseResult = bool.TryParse(value, out var flagValue);
        Assert.Equal(parseResult, configuration.TryGetFeatureFlagValue(featureName, out _));
        Assert.Equal(flagValue, configuration.IsFeatureEnabled(featureName));
    }

    [Fact]
    public void FeatureFlag_ConfigrationSectionNotExists()
    {
        var featureName = "TestFeature";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        Assert.False(configuration.TryGetFeatureFlagValue(featureName, out _));

        Assert.False(configuration.IsFeatureEnabled(featureName));
        Assert.True(configuration.IsFeatureEnabled(featureName, true));
    }

    [Fact]
    public void FeatureFlag_FeatureNameNotExists()
    {
        var featureName = "TestFeature";
        var configuration = new ConfigurationBuilder()
                 .AddInMemoryCollection(new[]
                 {
                         new KeyValuePair<string, string?>($"{ConfigurationExtension.FeatureFlagsSectionName}:Test","")
                 })
                 .Build();
        Assert.False(configuration.TryGetFeatureFlagValue(featureName, out _));

        Assert.False(configuration.IsFeatureEnabled(featureName));
        Assert.True(configuration.IsFeatureEnabled(featureName, true));
    }

    [Fact]
    public void FeatureFlag_FeatureFlagValueIsNull()
    {
        var featureName = "TestFeature";
        var configuration = new ConfigurationBuilder()
                 .AddInMemoryCollection(new[]
                 {
                         new KeyValuePair<string, string?>($"{ConfigurationExtension.FeatureFlagsSectionName}:{featureName}", null)
                 })
                 .Build();
        Assert.False(configuration.TryGetFeatureFlagValue(featureName, out _));

        Assert.False(configuration.IsFeatureEnabled(featureName));
        Assert.True(configuration.IsFeatureEnabled(featureName, true));
    }
}
