﻿- `T CompareExchange<T>(ref T location, T newValue, T expectedValue)`
    - If the value in `location` == `expectedValue` (using Reference equality), then `newValue` is stored in `location`.
    - Either way, the original value in `location` (before exchange) is returned.