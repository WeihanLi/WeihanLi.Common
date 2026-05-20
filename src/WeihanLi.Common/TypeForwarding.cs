// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Runtime.CompilerServices;
using WeihanLi.Common.Compressor;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Helpers.Combinatorics;
using WeihanLi.Common.Helpers.PeriodBatching;
using WeihanLi.Common.Otp;
using WeihanLi.Common.Services;
using WeihanLi.Extensions;

[assembly: TypeForwardedTo(typeof(IDataCompressor))]
[assembly: TypeForwardedTo(typeof(NullDataCompressor))]
[assembly: TypeForwardedTo(typeof(GZipDataCompressor))]
[assembly: TypeForwardedTo(typeof(BoundedConcurrentQueue<>))]
[assembly: TypeForwardedTo(typeof(BoundedQueueFullMode))]
[assembly: TypeForwardedTo(typeof(TotpHelper))]
[assembly: TypeForwardedTo(typeof(OtpHashAlgorithm))]
[assembly: TypeForwardedTo(typeof(TotpOptions))]
[assembly: TypeForwardedTo(typeof(Totp))]
[assembly: TypeForwardedTo(typeof(ITotpService))]
[assembly: TypeForwardedTo(typeof(TotpServiceExtensions))]
[assembly: TypeForwardedTo(typeof(TotpService))]
[assembly: TypeForwardedTo(typeof(GenerateOption))]
[assembly: TypeForwardedTo(typeof(Combinations<>))]
[assembly: TypeForwardedTo(typeof(Permutations<>))]
[assembly: TypeForwardedTo(typeof(Variations<>))]
[assembly: TypeForwardedTo(typeof(PeriodicBatching<>))]
[assembly: TypeForwardedTo(typeof(CommandLineParser))]
[assembly: TypeForwardedTo(typeof(LineParseOptions))]
[assembly: TypeForwardedTo(typeof(EnvHelper))]
[assembly: TypeForwardedTo(typeof(EnumHelper))]
[assembly: TypeForwardedTo(typeof(TypeHelper))]
[assembly: TypeForwardedTo(typeof(ValidateHelper))]
[assembly: TypeForwardedTo(typeof(ValueStopwatch))]
[assembly: TypeForwardedTo(typeof(ProfilerStopper))]
[assembly: TypeForwardedTo(typeof(StopwatchStopper))]
[assembly: TypeForwardedTo(typeof(ProfilerHelper))]
[assembly: TypeForwardedTo(typeof(IProfiler))]
[assembly: TypeForwardedTo(typeof(StopwatchProfiler))]
[assembly: TypeForwardedTo(typeof(DictionaryExtension))]
[assembly: TypeForwardedTo(typeof(EnumerableExtension))]
