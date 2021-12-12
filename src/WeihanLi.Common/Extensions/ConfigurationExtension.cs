using System.Text.RegularExpressions;
using WeihanLi.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration;

/// <summary>
/// Provides extensions for <see cref="IConfiguration"/> instances.
/// </summary>
public static class ConfigurationExtension
{
    #region Placeholder
    /// <summary>
    /// A regex which matches tokens in the following format: $(Item:Sub1:Sub2).
    /// inspired by https://github.com/henkmollema/ConfigurationPlaceholders
    /// </summary>
    private static readonly Regex _configPlaceholderRegex = new(@"\$\(([A-Za-z0-9:_]+?)\)");

    /// <summary>
    /// Replaces the placeholders in the specified <see cref="IConfiguration"/> instance.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance to replace placeholders in.</param>
    /// <returns>The given <see cref="IConfiguration"/> instance.</returns>
    public static IConfiguration ReplacePlaceholders(this IConfiguration configuration)
    {
        foreach (var kvp in configuration.AsEnumerable())
        {
            if (string.IsNullOrEmpty(kvp.Value))
            {
                // Skip empty configuration values.
                continue;
            }

            // Replace placeholders in the configuration value.
            var result = _configPlaceholderRegex.Replace(kvp.Value, match =>
            {
                if (!match.Success)
                {
                    // Return the original value.
                    return kvp.Value;
                }

                if (match.Groups.Count != 2)
                {
                    // There is a match, but somehow no group for the placeholder.
                    throw new InvalidConfigurationPlaceholderException(match.ToString());
                }

                var placeholder = match.Groups[1].Value;
                if (placeholder.StartsWith(":") || placeholder.EndsWith(":"))
                {
                    // Placeholders cannot start or end with a colon.
                    throw new InvalidConfigurationPlaceholderException(placeholder);
                }

                // Return the value in the configuration instance.
                return configuration[placeholder];
            });

            // Replace the value in the configuration instance.
            configuration[kvp.Key] = result;
        }

        return configuration;
    }

    private sealed class InvalidConfigurationPlaceholderException : InvalidOperationException
    {
        public InvalidConfigurationPlaceholderException(string placeholder) : base($"Invalid configuration placeholder: '{placeholder}'.")
        {
        }
    }

    #endregion

    #region GetAppSetting

    /// <summary>
    /// GetAppSetting
    /// Shorthand for GetSection("AppSettings")[key]
    /// </summary>
    /// <param name="configuration">IConfiguration instance</param>
    /// <param name="key">appSettings key</param>
    /// <returns>app setting value</returns>
    public static string? GetAppSetting(this IConfiguration configuration, string key)
    {
        return configuration.GetSection("AppSettings")?[key];
    }

    /// <summary>
    /// GetAppSetting
    /// Shorthand for GetSection("AppSettings")[key]
    /// </summary>
    /// <param name="configuration">IConfiguration instance</param>
    /// <param name="key">appSettings key</param>
    /// <returns>app setting value</returns>
    public static T GetAppSetting<T>(this IConfiguration configuration, string key)
    {
        return configuration.GetAppSetting(key).To<T>();
    }

    /// <summary>
    /// GetAppSetting
    /// Shorthand for GetSection("AppSettings")[key]
    /// </summary>
    /// <param name="configuration">IConfiguration instance</param>
    /// <param name="key">appSettings key</param>
    /// <param name="defaultValue">default value if not exist</param>
    /// <returns>app setting value</returns>
    public static T? GetAppSetting<T>(this IConfiguration configuration, string key, T defaultValue)
    {
        return configuration.GetAppSetting(key).ToOrDefault(defaultValue);
    }

    /// <summary>
    /// GetAppSetting
    /// Shorthand for GetSection("AppSettings")[key]
    /// </summary>
    /// <param name="configuration">IConfiguration instance</param>
    /// <param name="key">appSettings key</param>
    /// <param name="defaultValueFunc">default value func if not exist to get a default value</param>
    /// <returns>app setting value</returns>
    public static T? GetAppSetting<T>(this IConfiguration configuration, string key, Func<T> defaultValueFunc)
    {
        return configuration.GetAppSetting(key).ToOrDefault(defaultValueFunc);
    }
    #endregion

    #region FeatureFlag
    /// <summary>
    /// Feature flags configuration section name, FeatureFlags by default
    /// </summary>
    public static string FeatureFlagsSectionName = "FeatureFlags";

    /// <summary>
    /// FeatureFlag extension, (FeatureFlags:Feature) is feature enabled
    /// </summary>
    /// <param name="configuration">configuration</param>
    /// <param name="featureFlagName">feature flag name</param>
    /// <param name="featureFlagValue">featureFlagValue value if config not exists</param>
    /// <returns><c>true</c> enabled, otherwise disabled</returns>
    public static bool TryGetFeatureFlagValue(this IConfiguration configuration, string featureFlagName, out bool featureFlagValue)
    {
        featureFlagValue = false;
        var section = configuration.GetSection(FeatureFlagsSectionName);
        if (section.Exists())
        {
            return bool.TryParse(section[featureFlagName], out featureFlagValue);
        }
        return false;
    }

    /// <summary>
    /// FeatureFlag extension, (FeatureFlags:Feature) is feature enabled
    /// </summary>
    /// <param name="configuration">configuration</param>
    /// <param name="featureFlagName">feature flag name</param>
    /// <param name="defaultValue">default value if config not exists</param>
    /// <returns><c>true</c> enabled, otherwise disabled</returns>
    public static bool IsFeatureEnabled(this IConfiguration configuration, string featureFlagName, bool defaultValue = false)
    {
        if (TryGetFeatureFlagValue(configuration, featureFlagName, out var value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion
}
