# `ScrubJay.Functional`
Functional programming inspired types

## `Option<T>`
🚧

## `Result<T>` and `Result<T, E>`
### Links
- [Andrew Lock - Working with the Result Pattern](https://andrewlock.net/series/working-with-the-result-pattern/)
- [Milan Jovanović - Functional Error Handling with the Result Pattern](https://www.milanjovanovic.tech/blog/functional-error-handling-in-dotnet-with-the-result-pattern)
- [Ben Witt - Result Pattern](https://medium.com/@wgyxxbf/result-pattern-a01729f42f8c)

### The need for `IsOkWithError` and `IsErrorWithOk`

#### [Rust](https://www.rust-lang.org/)
In Rust, a `match` statement works inline, which supports early `return` from methods:
```rust
fn write_cool_text(path: &str) -> Result<()> {
    let mut file = File::create(path)?;
    file.write(b"Sphinx of black quartz, judge my vow!")?;
    dbg!("Wrote text to file");
    Ok(())
}
```

#### C#
This behavior is hard to mimic:
```csharp
// Given this supporting method that wraps `int.TryParse()`
Result<int> TryParse(string? str);

// Given this surrounding method
public Result<int> ParseAndWriteI32(string text)
{
    {CODE}
    Console.WriteLine(i);
    return Result<int>.Ok(i);
}

// For {CODE}:

// this does not compile
int i = TryParse("???").Match(
    ok => ok,
    err => return err); <- we're inside a delegate, cannot return from this method


// verbose
Result<int> result = TryParse("???");
if (!result)                // Result<T> implicitly converts to bool
    return result;          // return failed result
int i = result.OkOrThrow(); // Has a redundant check on _isOk that we already know is true

// cleaner
Result<int> result = TryParse("???");
if (!result.IsOk(out int i))    // Check for ok direcly
    return result;              // return failed result
// now we can use i
// but this does not work if the Result<> for the return has a different type
// then we have to extract the error
```

The methods `IsOkWithError` and `IsErrorWithOk` were added to extract the ok and error values at the same time:
```csharp
Result<int> result = TryParse("???");
if (result.IsOkWithError(out T ok, out E error))
    return error;
// continue to use ok
```

### `Ok<T>` and `Error<T>`
🚧
