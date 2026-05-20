// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Runtime.CompilerServices;
using WeihanLi.Common.Compressor;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Helpers.Combinatorics;
using WeihanLi.Common.Helpers.PeriodBatching;
using WeihanLi.Common.Otp;
using WeihanLi.Common.Services;

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
