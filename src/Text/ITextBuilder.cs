namespace ScrubJay.Text;

public interface ITextBuilder<B> : IBuilder<B>
    where B : ITextBuilder<B>
{
    ref char this[Index index] { get; }

    Span<char> this[Range range] { get; }

    /// <summary>
    /// Gets the total length of text written to this Builder
    /// </summary>
    int Length { get; }

    /// <summary>
    /// Gets the current capacity to store items<br/>
    /// This will be automatically increased if required or by calling <see cref="Grow"/>
    /// </summary>
    int Capacity { get; }

    /// <summary>
    /// Append a <see cref="char">character</see>
    /// </summary>
    /// <param name="ch">The <see cref="char"/> to append</param>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    B Append(char ch);

    /// <summary>
    /// Append a <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see>
    /// </summary>
    /// <param name="text">The <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> to append</param>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    B Append(scoped text text);

    B Append(char[]? chars);

    /// <summary>
    /// Append a <see cref="string"/>
    /// </summary>
    /// <param name="str">The <see cref="string"/> to append</param>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    B Append(string? str);

    /// <summary>
    /// Appends text using an <see cref="InterpolatedTextBuilder{B}"/>
    /// </summary>
    /// <param name="interpolatedTextBuilder"></param>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    B Append([InterpolatedStringHandlerArgument("")]
        ref InterpolatedTextBuilder<B> interpolatedTextBuilder);

    /// <summary>
    /// Append a <typeparamref name="T"/> <paramref name="value"/> with no formatting
    /// </summary>
    /// <param name="value">The value to append the textual representation of</param>
    /// <typeparam name="T">The <see cref="Type"/> of value to append</typeparam>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    B Append<T>(T? value);

    /// <summary>
    /// Append a <typeparamref name="T"/> <paramref name="value"/> with a format
    /// </summary>
    /// <param name="value">The value to append the textual representation of</param>
    /// <param name="format">The <see cref="string"/> format</param>
    /// <param name="provider">The optional <see cref="IFormatProvider"/></param>
    /// <typeparam name="T">The <see cref="Type"/> of value to format and append</typeparam>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    B Append<T>(T? value, string? format, IFormatProvider? provider);

    /// <summary>
    /// Append a <typeparamref name="T"/> <paramref name="value"/> with a format
    /// </summary>
    /// <param name="value">The value to append the textual representation of</param>
    /// <param name="format">The <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> format</param>
    /// <param name="provider">The optional <see cref="IFormatProvider"/></param>
    /// <typeparam name="T">The <see cref="Type"/> of value to format and append</typeparam>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    B Append<T>(T? value, scoped text format, IFormatProvider? provider);

    /// <summary>
    /// Appends a <see cref="Environment.NewLine"/>
    /// </summary>
    /// <returns></returns>
    B NewLine();

    B AppendLine(char ch);
    B AppendLine(scoped text text);
    B AppendLine(char[]? chars);
    B AppendLine(string? str);
    B AppendLine<T>(T? value);
    B AppendLine<T>(T? value, scoped text format, IFormatProvider? provider);
    B AppendLine<T>(T? value, string? format, IFormatProvider? provider);

    B AppendLine(
        [InterpolatedStringHandlerArgument("")]
        ref InterpolatedTextBuilder<B> interpolatedTextBuilder);

    B Align(
        char ch,
        int width,
        char paddingChar = ' ',
        Alignment alignment = Alignment.None);

    B Align(
        scoped text text,
        int width,
        char paddingChar = ' ',
        Alignment alignment = Alignment.None);

    B AlignFormat<T>(
        T? value,
        int width,
        string? format,
        char paddingChar = ' ',
        Alignment alignment = Alignment.None);

    B EnumerateAppend<T>(ReadOnlySpan<T> values);
    B EnumerateAppend<T>(IEnumerable<T> values);

    B Delimit<T>(char delimiter, ReadOnlySpan<T> values, Action<B, T> onBuilderValue);
    B Delimit<T>(string delimiter, ReadOnlySpan<T> values, Action<B, T> onBuilderValue);
    B Delimit<T>(scoped text delimiter, ReadOnlySpan<T> values, Action<B, T> onBuilderValue);
    B Delimit<T>(Action<B> onDelimit, ReadOnlySpan<T> values, Action<B, T> onBuilderValue);
    B Delimit<T>(char delimiter, T[]? values, Action<B, T> onBuilderValue);
    B Delimit<T>(string delimiter, T[]? values, Action<B, T> onBuilderValue);
    B Delimit<T>(scoped text delimiter, T[]? values, Action<B, T> onBuilderValue);
    B Delimit<T>(Action<B> onDelimit, T[]? values, Action<B, T> onBuilderValue);
    B Delimit<T>(char delimiter, IEnumerable<T>? values, Action<B, T> onBuilderValue);
    B Delimit<T>(string delimiter, IEnumerable<T>? values, Action<B, T> onBuilderValue);
    B Delimit<T>(scoped text delimiter, IEnumerable<T>? values, Action<B, T> onBuilderValue);
    B Delimit<T>(Action<B> onDelimit, IEnumerable<T>? values, Action<B, T> onBuilderValue);

    B LineDelimit<T>(ReadOnlySpan<T> values, Action<B, T> buildValue);
    B LineDelimit<T>(T[]? values, Action<B, T> buildValue);
    B LineDelimit<T>(IEnumerable<T>? values, Action<B, T> onBuilderValue);
    B DelimitAppend<T>(char delimiter, ReadOnlySpan<T> values);
    B DelimitAppend<T>(string delimiter, ReadOnlySpan<T> values);
    B DelimitAppend<T>(scoped text delimiter, ReadOnlySpan<T> values);
    B DelimitAppend<T>(Action<B> onDelimit, ReadOnlySpan<T> values);
    B DelimitAppend<T>(char delimiter, T[]? values, string? format, IFormatProvider? provider);
    B DelimitAppend<T>(string delimiter, T[]? values, string? format, IFormatProvider? provider);
    B DelimitAppend<T>(scoped text delimiter, T[]? values, string? format, IFormatProvider? provider);
    B DelimitAppend<T>(Action<B> onDelimit, T[]? values, string? format, IFormatProvider? provider);
    B DelimitAppend<T>(char delimiter, IEnumerable<T>? values);
    B DelimitAppend<T>(string delimiter, IEnumerable<T>? values);
    B DelimitAppend<T>(scoped text delimiter, IEnumerable<T>? values);
    B DelimitAppend<T>(Action<B> onDelimit, IEnumerable<T>? values);
    B LineDelimitAppend<T>(ReadOnlySpan<T> values);
    B LineDelimitAppend<T>(T[]? values);
    B LineDelimitAppend<T>(IEnumerable<T>? values);
    B Insert(Index index, char ch);
    B Insert(Index index, scoped text text);
    B Insert(Index index, string? str);
    B Allocate(int count, SpanDelegates.ActionS<char> write);

    /// <summary>
    /// Allocates a <see cref="Span{T}"/> of the given <paramref name="length"/>
    /// </summary>
    /// <param name="length">
    /// The total number of items to allocate a <see cref="Span{T}"/> for
    /// </param>
    /// <returns>
    /// A <see cref="Span{T}"/> over the allocated items
    /// </returns>
    Span<Char> Allocate(int length);

    B Repeat(int count, char ch);
    B Repeat(int count, scoped text text);
    B RepeatFormat<T>(int count, T? value, string? format);

    Option<int> TryFindIndex(char ch, bool firstToLast, Index? offset,
        IEqualityComparer<char>? charComparer);

    Option<int> TryFindIndex(
        scoped text text,
        bool firstToLast,
        Index? offset,
        IEqualityComparer<char>? charComparer);

    bool TryRemoveAt(Index index);
    bool TryRemoveMany(Range range);
    B RemoveLast(int count);
    B Clear();
    bool TryCopyTo(Span<char> span);
    text AsText();
    void Dispose();
    string ToStringAndDispose();
    string ToString();
    bool Equals([NotNullWhen(true)] object? obj);
    int GetHashCode();
    void Grow();
    void GrowBy(int adding);
    void GrowTo(int minCapacity);

    /// <summary>
    /// Adds the given <paramref name="items"/> to this <see cref="PooledList{T}"/>
    /// </summary>
    void AddMany(ReadOnlySpan<Char> items);

    /// <summary>
    /// Adds the given <paramref name="items"/> to this <see cref="PooledList{T}"/>
    /// </summary>
    void AddMany(params Char[]? items);

    /// <summary>
    /// Adds the given <paramref name="items"/> to this <see cref="PooledList{T}"/>
    /// </summary>
    void AddMany(IEnumerable<Char>? items);

    /// <summary>
    /// Try to insert an <paramref name="item"/> into this <see cref="Buffer{T}"/> at <paramref name="index"/>
    /// </summary>
    /// <param name="index">The <see cref="Index"/> to insert the <paramref name="item"/></param>
    /// <param name="item">The item to insert</param>
    /// <returns>
    /// A <see cref="Result{O,E}"/> that contains the <c>int</c> offset the item was inserted at or an <see cref="Exception"/> that describes why insertion failed
    /// </returns>
    Result<int> TryInsert(Index index, Char item);

    /// <summary>
    /// Try to insert multiple <paramref name="items"/> into this <see cref="Buffer{T}"/> at <paramref name="index"/>
    /// </summary>
    /// <param name="index">The <see cref="Index"/> to insert the <paramref name="items"/></param>
    /// <param name="items">The <see cref="ReadOnlySpan{T}"/> of items to insert</param>
    /// <returns>
    /// A <see cref="Result{O,E}"/> that contains the <c>int</c> offset the items were inserted at or an <see cref="Exception"/> that describes why insertion failed
    /// </returns>
    Result<int> TryInsertMany(Index index, scoped ReadOnlySpan<Char> items);

    /// <summary>
    /// Try to insert multiple <paramref name="items"/> into this <see cref="Buffer{T}"/> at <paramref name="index"/>
    /// </summary>
    /// <param name="index">The <see cref="Index"/> to insert the <paramref name="items"/></param>
    /// <param name="items">The <see cref="Array">T[]</see> of items to insert</param>
    /// <returns>
    /// A <see cref="Result{O,E}"/> that contains the <c>int</c> offset the items were inserted at or an <see cref="Exception"/> that describes why insertion failed
    /// </returns>
    void TryInsertMany(Index index, params Char[]? items);

    /// <summary>
    /// Try to insert multiple <paramref name="items"/> into this <see cref="Buffer{T}"/> at <paramref name="index"/>
    /// </summary>
    /// <param name="index">The <see cref="Index"/> to insert the <paramref name="items"/></param>
    /// <param name="items">The <see cref="IEnumerable{T}"/> of items to insert</param>
    /// <returns>
    /// A <see cref="Result{O,E}"/> that contains the <c>int</c> offset the items were inserted at or an <see cref="Exception"/> that describes why insertion failed
    /// </returns>
    Result<int> TryInsertMany(Index index, IEnumerable<Char>? items);

    /// <summary>
    /// Sorts the items in this <see cref="PooledList{T}"/> using an optional <see cref="IComparer{T}"/>
    /// </summary>
    /// <param name="itemComparer">
    /// An optional <see cref="IComparer{T}"/> used to sort the items, defaults to <see cref="Comparer{T}"/>.<see cref="Comparer{T}.Default"/>
    /// </param>
    void Sort(IComparer<Char>? itemComparer);

    /// <summary>
    /// Sorts the items in this <see cref="PooledList{T}"/> using a <see cref="Comparison{T}"/> delegate
    /// </summary>
    /// <param name="itemComparison">
    /// The <see cref="Comparison{T}"/> delegate used to sort the items
    /// </param>
    void Sort(Comparison<Char> itemComparison);

    /// <summary>
    /// Reverses the order of the items in this <see cref="PooledList{T}"/>
    /// </summary>
    void Reverse();

    /// <summary>
    /// Try to find the Index and Item that match an <paramref name="itemPredicate"/>
    /// </summary>
    /// <param name="itemPredicate">
    /// The <see cref="Predicate{T}"/> used to determine if a valid item has been found
    /// </param>
    /// <param name="firstToLast"><b>default: true</b><br/>
    /// <c>true</c>: Search from low to high indices<br/>
    /// <c>false</c>: Search from high to low indices
    /// </param>
    /// <param name="offset">
    /// The <see cref="Index"/> offset to start the search from
    /// </param>
    /// <returns>
    /// An <see cref="Option{T}"/> that might contain the first matching Index + Item
    /// </returns>
    Option<(int Index, Char Item)> TryFindItemIndex(
        Func<Char, bool>? itemPredicate,
        bool firstToLast = true,
        Index? offset = default);

    Result<Char> TryGetAt(Index index);
    Result<Char[]> TryGetMany(Range range);
    Result<Unit> TryGetManyTo(Range range, Span<Char> destination);
    Result<Unit> TrySetAt(Index index, Char item);
    Result<Unit> TrySetMany(Range range, scoped ReadOnlySpan<Char> items);

    /// <summary>
    /// Try to remove and return the item at the given <see cref="Index"/>
    /// </summary>
    /// <param name="index">
    /// The <see cref="Index"/> of the item to remove
    /// </param>
    /// <returns>
    /// An <see cref="Option{T}"/> that contains the removed value
    /// </returns>
    Result<Char> TryRemoveAndGetAt(Index index);

    /// <summary>
    /// Try to remove and return the items at the given <see cref="Range"/>
    /// </summary>
    /// <param name="range">
    /// The <see cref="Range"/> of items to remove
    /// </param>
    /// <returns>
    /// An <see cref="Option{T}"/> containing an <see cref="Array">T[]</see> of removed items
    /// </returns>
    Result<Char[]> TryRemoveAndGetMany(Range range);

    /// <summary>
    /// Remove all the items in this <see cref="PooledList{T}"/> that match an <paramref name="itemPredicate"/>
    /// </summary>
    /// <param name="itemPredicate">
    /// The <see cref="Predicate{T}"/> to determine if an item is to be removed
    /// </param>
    /// <returns>
    /// The total number of items removed
    /// </returns>
    int RemoveWhere(Func<Char, bool>? itemPredicate);

    /// <summary>
    /// Try to use the available capacity of this <see cref="PooledList{T}"/> using a <see cref="SpanDelegates.FuncS{T1,R}"/> delegate
    /// </summary>
    /// <param name="useAvailable">
    /// <see cref="SpanDelegates.FuncS{T1,R}"/> to apply to any currently available space
    /// </param>
    /// <returns>
    /// <c>true</c> if the <see cref="SpanDelegates.FuncS{T1,R}"/> operation succeeded<br/>
    /// <c>false</c> if it did not
    /// </returns>
    bool TryUseAvailable(SpanDelegates.FuncS<Char, int> useAvailable);

    /// <summary>
    /// Performs a <see cref="ActionRef{T}"/> operation on each item in this <see cref="PooledList{T}"/>
    /// </summary>
    /// <param name="perItem">
    /// The <see cref="ActionRef{T}"/> delegate that can mutate items
    /// </param>
    void ForEach(ActionRef<Char> perItem);

    /// <summary>
    /// Performs a <see cref="ActionRef{T,I}"/> operation on each item in this <see cref="PooledList{T}"/>
    /// </summary>
    /// <param name="perItem">
    /// The <see cref="ActionRef{T,I}"/> delegate that can mutate items
    /// </param>
    void ForEach(ActionRef<Char, int> perItem);

    /// <summary>
    /// Get the <see cref="Span{T}"/> of written items in this <see cref="PooledList{T}"/>
    /// </summary>
    Span<Char> AsSpan();

    /// <summary>
    /// Gets a <see cref="Span{T}"/> slice of written items starting at the given <paramref name="index"/>
    /// </summary>
    Span<Char> Slice(int index);

    /// <summary>
    /// Gets a <see cref="Span{T}"/> slice of written items starting at the given <see cref="Index"/>
    /// </summary>
    Span<Char> Slice(Index index);

    /// <summary>
    /// Gets a <see cref="Span{T}"/> slice of written items from <paramref name="index"/> for <paramref name="count"/>
    /// </summary>
    Span<Char> Slice(int index, int count);

    /// <summary>
    /// Gets a <see cref="Span{T}"/> slice of written items from <paramref name="index"/> for <paramref name="count"/>
    /// </summary>
    Span<Char> Slice(Index index, int count);

    /// <summary>
    /// Gets a <see cref="Span{T}"/> slice of written items for a <see cref="Range"/>
    /// </summary>
    Span<Char> Slice(Range range);

    /// <summary>
    /// Copy the items in this <see cref="PooledList{T}"/> to a new <c>T[]</c>
    /// </summary>
    Char[] ToArray();

    /// <summary>
    /// Convert this <see cref="PooledList{T}"/> to a <see cref="List{T}"/> containing the same items
    /// </summary>
    List<Char> ToList();

    bool SequenceEqual(ReadOnlySpan<Char> items);
    bool SequenceEqual(params Char[]? items);
    bool SequenceEqual(IEnumerable<Char>? items);
}