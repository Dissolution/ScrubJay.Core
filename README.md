# ScrubJay.Core
The core `ScrubJay` library, this contains all the code used by `ScrubJay` namespaces.




### `Result`
In Rust, it is possible to return from a match statement, so they can more easily consume a `Result`
e.g.:
```rust
// Early return on error
let mut file = match File::create("my_best_friends.txt") {
    Err(e) => return Err(e),
    Ok(f) => f,
};
```
That is hard to do in C#:
```csharp
// Does not compile:
int i = int.TryParse("???").Match(
    ok => ok,
    err => return err);

// Does not compile:
Result<int, Exception> result = int.TryParse("???");
int i;
result.Match(
    ok => i = ok,
    err => return err);

// Verbose:
Result<int, Exception> result = int.TryParse("???");
if (result.IsErr(out var ex))
    return result; // return ex;
int i = result.Unwrap();    // has a redundant check for isOk that we already know is true

// Best:
Result<int, Exception> result = int.TryParse("???");
if (!result.IsOk(out var ok))
    return result; // have to return Result and not TError (unless we call IsError or UnwrapErr)
```
Thus, the methods `IsSuccess` and `IsFailure` were added to extract the ok and error values at the same time:
```csharp
Result<int, Exception> result = int.TryParse("???");
if (result.IsFailure(out var error, out var ok))
{
    return error;    
}       
/* or */
if (!result.IsSuccess(out var ok, out var error))
{
    return error;
}
/* or just use ok and/or error and beware that one of them will be default() */
```