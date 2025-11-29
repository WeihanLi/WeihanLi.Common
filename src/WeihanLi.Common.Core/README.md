# WeihanLi.Common.Core

AOT-compatible core library with extensions, helpers, and utilities.

This library contains code that is fully compatible with Native AOT compilation and trimming. It provides fundamental utilities that can be safely used in AOT scenarios.

## Features

- Guard utilities for parameter validation
- Base encoding helpers (Base32, Base64Url)
- String utilities
- Hash helpers
- Result models
- Profiling utilities
- Service abstractions

## Usage

```csharp
using WeihanLi.Common;
using WeihanLi.Common.Helpers;

// Guard utilities
Guard.NotNull(value);
Guard.NotNullOrEmpty(str);

// Hash helpers
var hash = HashHelper.GetHashedString(HashType.SHA256, "input");

// Base64Url encoding
var encoded = Base64UrlEncodeHelper.Encode("data");
```

## AOT Compatibility

This library is designed to be fully compatible with Native AOT compilation. It avoids:
- Dynamic code generation
- Unreferenced code paths
- Reflection without proper annotations

For code that requires reflection or dynamic code generation, use `WeihanLi.Common` which includes additional functionality but may not be fully AOT-compatible.
