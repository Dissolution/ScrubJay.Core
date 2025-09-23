// using System.Collections;
// using ScrubJay.Collections;
// using ScrubJay.Utilities;
//
// namespace ScrubJay.Tests.CollectionsTests;
//
// public class OrderedListTests
// {
// #region Constructor Tests
//
//     [Test]
//     public async Task Constructor_WithDefaultParameters_CreatesEmptyList()
//     {
//         var list = new OrderedList<int>();
//
//         await Assert.That(list.Count).IsEqualTo(0);
//         await Assert.That(list.Comparer).IsEqualTo(Comparer<int>.Default);
//         await Assert.That(list.NewestFirst).IsFalse();
//     }
//
//     [Test]
//     public async Task Constructor_WithCustomComparer_UsesCustomComparer()
//     {
//         var customComparer = Comparer<int>.Create((x, y) => y.CompareTo(x)); // Descending
//         var list = new OrderedList<int>(customComparer);
//
//         await Assert.That(list.Comparer).IsEqualTo(customComparer);
//         await Assert.That(list.NewestFirst).IsFalse();
//     }
//
//     [Test]
//     public async Task Constructor_WithNewestFirstTrue_SetsNewestFirstProperty()
//     {
//         var list = new OrderedList<int>(newestFirst: true);
//
//         await Assert.That(list.NewestFirst).IsTrue();
//     }
//
//     [Test]
//     public async Task Constructor_WithNullComparer_UsesDefaultComparer()
//     {
//         var list = new OrderedList<int>(null, true);
//
//         await Assert.That(list.Comparer).IsEqualTo(Comparer<int>.Default);
//         await Assert.That(list.NewestFirst).IsTrue();
//     }
//
// #endregion
//
// #region Count Property Tests
//
//     [Test]
//     public async Task Count_EmptyList_ReturnsZero()
//     {
//         var list = new OrderedList<int>();
//
//         await Assert.That(list.Count).IsEqualTo(0);
//     }
//
//     [Test]
//     public async Task Count_AfterAddingItems_ReturnsCorrectCount()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1);
//         list.Add(2);
//         list.Add(1);
//
//         await Assert.That(list.Count).IsEqualTo(3);
//     }
//
//     [Test]
//     public async Task Count_AfterRemovingItems_ReturnsCorrectCount()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1);
//         list.Add(2);
//         list.RemoveFirst(1);
//
//         await Assert.That(list.Count).IsEqualTo(1);
//     }
//
// #endregion
//
// #region Add Method Tests
//
//     [Test]
//     public async Task Add_AddsToList()
//     {
//         var list = new OrderedList<int>();
//
//         list.Add(1);
//
//         await Assert.That(list.Count).IsEqualTo(1);
//         var items = list.ToList();
//         await Assert.That(items[0]).IsEqualTo(1);
//     }
//
//     [Test]
//     public async Task Add_MultipleItems_MaintainsSortOrder()
//     {
//         var list = new OrderedList<int>();
//
//         list.Add(3);
//         list.Add(1);
//         list.Add(2);
//
//         var items = list.ToList();
//         await Assert.That(items[0]).IsEqualTo(1);
//         await Assert.That(items[1]).IsEqualTo(2);
//         await Assert.That(items[2]).IsEqualTo(3);
//     }
//
//     [Test]
//     public async Task Add_DuplicateKeysWithNewestFirstFalse_AddsAtEnd()
//     {
//         var list = new OrderedList<IdName>(comparer: IdNameComparer.Default, newestFirst: false);
//
//         list.Add(new(1, "first"));
//         list.Add(new(1, "second"));
//         list.Add(new(1, "third"));
//
//         var items = list.ToList();
//         await Assert.That(items[0].Name).IsEqualTo("first");
//         await Assert.That(items[1].Name).IsEqualTo("second");
//         await Assert.That(items[2].Name).IsEqualTo("third");
//     }
//
//     [Test]
//     public async Task Add_DuplicateKeysWithNewestFirstTrue_AddsAtBeginning()
//     {
//         var list = new OrderedList<IdName>(comparer: IdNameComparer.Default, newestFirst: true);
//
//         list.Add(new(1, "first"));
//         list.Add(new(1, "second"));
//         list.Add(new(1, "third"));
//
//         var items = list.ToList();
//         await Assert.That(items[0].Name).IsEqualTo("third");
//         await Assert.That(items[1].Name).IsEqualTo("second");
//         await Assert.That(items[2].Name).IsEqualTo("first");
//     }
//
//     [Test]
//     public async Task Add_WithCustomComparer_UsesDifferentSortOrder()
//     {
//         var descendingComparer = Comparer<int>.Create((x, y) => y.CompareTo(x));
//         var list = new OrderedList<int>(descendingComparer);
//
//         list.Add(1);
//         list.Add(3);
//         list.Add(2);
//
//         var items = list.ToList();
//         await Assert.That(items[0]).IsEqualTo(3);
//         await Assert.That(items[1]).IsEqualTo(2);
//         await Assert.That(items[2]).IsEqualTo(1);
//     }
//
// #endregion
//
// #region Clear Method Tests
//
//     [Test]
//     public async Task Clear_EmptyList_RemainsEmpty()
//     {
//         var list = new OrderedList<int>();
//
//         list.Clear();
//
//         await Assert.That(list.Count).IsEqualTo(0);
//     }
//
//     [Test]
//     public async Task Clear_ListWithItems_RemovesAllItems()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1);
//         list.Add(2);
//         list.Add(3);
//
//         list.Clear();
//
//         await Assert.That(list.Count).IsEqualTo(0);
//     }
//
// #endregion
//
// #region Contains Method Tests
//
//     [Test]
//     public async Task Contains_ExistingKey_ReturnsTrue()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1);
//         list.Add(2);
//
//         var result = list.Contains(1);
//
//         await Assert.That(result).IsTrue();
//     }
//
//     [Test]
//     public async Task Contains_NonExistingKey_ReturnsFalse()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1);
//         list.Add(2);
//
//         var result = list.Contains(3);
//
//         await Assert.That(result).IsFalse();
//     }
//
//     [Test]
//     public async Task Contains_EmptyList_ReturnsFalse()
//     {
//         var list = new OrderedList<int>();
//
//         var result = list.Contains(1);
//
//         await Assert.That(result).IsFalse();
//     }
//
//     [Test]
//     public async Task Contains_DuplicateKeys_ReturnsTrue()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1);
//         list.Add(1);
//
//         var result = list.Contains(1);
//
//         await Assert.That(result).IsTrue();
//     }
//
// #endregion
//
//
// #region FindFirstIndex Method Tests
//
//     [Test]
//     public async Task FindFirstIndex_ExistingKey_ReturnsCorrectIndex()
//     {
//         var list = new OrderedList<int>();
//         list.Add(3);
//         list.Add(1);
//         list.Add(2);
//
//         var index = list.FindFirstIndex(2);
//
//         await Assert.That(index).IsEqualTo(1); // Should be at index 1 after sorting
//     }
//
//     [Test]
//     public async Task FindFirstIndex_NonExistingKey_ReturnsMinusOne()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1);
//         list.Add(2);
//
//         var index = list.FindFirstIndex(3);
//
//         await Assert.That(index).IsEqualTo(-1);
//     }
//
//     [Test]
//     public async Task FindFirstIndex_DuplicateKeys_ReturnsFirstOccurrence()
//     {
//         var list = new OrderedList<IdName>(comparer: IdNameComparer.Default);
//         list.Add(new(1, "first"));
//         list.Add(new(1, "second"));
//         list.Add(new(1, "third"));
//
//         var index = list.FindFirstIndex();
//
//         await Assert.That(index).IsEqualTo(0);
//         var items = list.ToList();
//         await Assert.That(items[index].Value).IsEqualTo("first");
//     }
//
// #endregion
//
// #region FindLastIndex Method Tests
//
//     [Test]
//     public async Task FindLastIndex_ExistingKey_ReturnsCorrectIndex()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1, "first");
//         list.Add(1, "second");
//         list.Add(1, "third");
//         list.Add(2);
//
//         var index = list.FindLastIndex(1);
//
//         await Assert.That(index).IsEqualTo(2); // Last occurrence of key 1
//     }
//
//     [Test]
//     public async Task FindLastIndex_NonExistingKey_ReturnsMinusOne()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1);
//         list.Add(2);
//
//         var index = list.FindLastIndex(3);
//
//         await Assert.That(index).IsEqualTo(-1);
//     }
//
// #endregion
//
// #region GetValues Method Tests
//
//     [Test]
//     public async Task GetValues_ExistingKey_ReturnsAllValues()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1, "first");
//         list.Add(2, "other");
//         list.Add(1, "second");
//         list.Add(1, "third");
//
//         var results = list.GetValues(1);
//
//         await Assert.That(results.Count).IsEqualTo(3);
//         await Assert.That(results.Contains("first")).IsTrue();
//         await Assert.That(results.Contains("second")).IsTrue();
//         await Assert.That(results.Contains("third")).IsTrue();
//     }
//
//     [Test]
//     public async Task GetValues_NonExistingKey_ReturnsEmptyList()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1);
//         list.Add(2);
//
//         var results = list.GetValues(3);
//
//         await Assert.That(results.Count).IsEqualTo(0);
//     }
//
//     [Test]
//     public async Task GetValues_EmptyList_ReturnsEmptyList()
//     {
//         var list = new OrderedList<int>();
//
//         var results = list.GetValues(1);
//
//         await Assert.That(results.Count).IsEqualTo(0);
//     }
//
// #endregion
//
// #region GetEnumerator Method Tests
//
//     [Test]
//     public async Task GetEnumerator_EmptyList_ReturnsEmptyEnumerator()
//     {
//         var list = new OrderedList<int>();
//
//         var enumerator = list.GetEnumerator();
//         var hasNext = enumerator.MoveNext();
//
//         await Assert.That(hasNext).IsFalse();
//     }
//
//     [Test]
//     public async Task GetEnumerator_WithItems_IteratesInSortedOrder()
//     {
//         var list = new OrderedList<int>();
//         list.Add(3);
//         list.Add(1);
//         list.Add(2);
//
//         var items = new List<Pair<int>>();
//         foreach (var item in list)
//         {
//             items.Add(item);
//         }
//
//         await Assert.That(items.Count).IsEqualTo(3);
//         await Assert.That(items[0].Key).IsEqualTo(1);
//         await Assert.That(items[1].Key).IsEqualTo(2);
//         await Assert.That(items[2].Key).IsEqualTo(3);
//     }
//
//     [Test]
//     public async Task GetEnumerator_NonGeneric_WorksCorrectly()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1);
//         list.Add(2);
//
//         IEnumerable enumerable = list;
//         var enumerator = enumerable.GetEnumerator();
//         var items = new List<object>();
//
//         while (enumerator.MoveNext())
//         {
//             items.Add(enumerator.Current);
//         }
//
//         await Assert.That(items.Count).IsEqualTo(2);
//         await Assert.That(items[0]).IsTypeOf<Pair<int>>();
//     }
//
// #endregion
//
// #region RemoveFirst Method Tests
//
//     [Test]
//     public async Task RemoveFirst_ExistingKey_RemovesFirstOccurrenceAndReturnsTrue()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1, "first");
//         list.Add(2);
//         list.Add(1, "second");
//
//         var result = list.RemoveFirst(1);
//
//         await Assert.That(result).IsTrue();
//         await Assert.That(list.Count).IsEqualTo(2);
//         var items = list.ToList();
//         await Assert.That(items[0].Value).IsEqualTo("second"); // First occurrence removed
//     }
//
//     [Test]
//     public async Task RemoveFirst_NonExistingKey_ReturnsFalse()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1);
//         list.Add(2);
//
//         var result = list.RemoveFirst(3);
//
//         await Assert.That(result).IsFalse();
//         await Assert.That(list.Count).IsEqualTo(2);
//     }
//
//     [Test]
//     public async Task RemoveFirst_EmptyList_ReturnsFalse()
//     {
//         var list = new OrderedList<int>();
//
//         var result = list.RemoveFirst(1);
//
//         await Assert.That(result).IsFalse();
//         await Assert.That(list.Count).IsEqualTo(0);
//     }
//
// #endregion
//
// #region RemoveLast Method Tests
//
//     [Test]
//     public async Task RemoveLast_ExistingKey_RemovesLastOccurrenceAndReturnsTrue()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1, "first");
//         list.Add(2);
//         list.Add(1, "second");
//
//         var result = list.RemoveLast(1);
//
//         await Assert.That(result).IsTrue();
//         await Assert.That(list.Count).IsEqualTo(2);
//         var items = list.ToList();
//         await Assert.That(items[0].Value).IsEqualTo("first"); // Last occurrence removed
//     }
//
//     [Test]
//     public async Task RemoveLast_NonExistingKey_ReturnsFalse()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1);
//         list.Add(2);
//
//         var result = list.RemoveLast(3);
//
//         await Assert.That(result).IsFalse();
//         await Assert.That(list.Count).IsEqualTo(2);
//     }
//
// #endregion
//
// #region RemoveAll Method Tests
//
//     [Test]
//     public async Task RemoveAll_Key_ExistingKey_RemovesAllOccurrencesAndReturnsCount()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1, "first");
//         list.Add(2);
//         list.Add(1, "second");
//         list.Add(3);
//         list.Add(1, "third");
//
//         var removed = list.RemoveAll(1);
//
//         await Assert.That(removed).IsEqualTo(3);
//         await Assert.That(list.Count).IsEqualTo(2);
//         var items = list.ToList();
//         await Assert.That(items[0].Key).IsEqualTo(2);
//         await Assert.That(items[1].Key).IsEqualTo(3);
//     }
//
//     [Test]
//     public async Task RemoveAll_Key_NonExistingKey_ReturnsZero()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1);
//         list.Add(2);
//
//         var removed = list.RemoveAll(3);
//
//         await Assert.That(removed).IsEqualTo(0);
//         await Assert.That(list.Count).IsEqualTo(2);
//     }
//
//     [Test]
//     public async Task RemoveAll_Pair_ExistingPair_RemovesAllMatchingPairsAndReturnsCount()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1, "first");
//         list.Add(1, "second");
//         list.Add(1, "first"); // Duplicate pair
//         list.Add(2);
//
//         var pair = new Pair<int>(1, "first");
//         var removed = list.RemoveAll(pair);
//
//         await Assert.That(removed).IsEqualTo(2);
//         await Assert.That(list.Count).IsEqualTo(2);
//         var items = list.ToList();
//         await Assert.That(items.Any(item => item.Value == "second")).IsTrue();
//         await Assert.That(items.Any(item => item.Value == "two")).IsTrue();
//     }
//
//     [Test]
//     public async Task RemoveAll_Pair_NonExistingPair_ReturnsZero()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1);
//         list.Add(2);
//
//         var pair = new Pair<int>(1, "different");
//         var removed = list.RemoveAll(pair);
//
//         await Assert.That(removed).IsEqualTo(0);
//         await Assert.That(list.Count).IsEqualTo(2);
//     }
//
// #endregion
//
// #region CopyTo Method Tests
//
//     [Test]
//     public async Task CopyTo_ValidArray_CopiesAllItems()
//     {
//         var list = new OrderedList<int>();
//         list.Add(2);
//         list.Add(1);
//         list.Add(3);
//
//         var array = new Pair<int>[3];
//         list.CopyTo(array);
//
//         await Assert.That(array[0].Key).IsEqualTo(1);
//         await Assert.That(array[1].Key).IsEqualTo(2);
//         await Assert.That(array[2].Key).IsEqualTo(3);
//     }
//
//     [Test]
//     public async Task CopyTo_WithArrayIndex_CopiesAtCorrectPosition()
//     {
//         var list = new OrderedList<int>();
//         list.Add(1);
//         list.Add(2);
//
//         var array = new Pair<int>[4];
//         list.CopyTo(array, 1);
//
//         await Assert.That(array[0]).IsEqualTo(default(Pair<int>));
//         await Assert.That(array[1].Key).IsEqualTo(1);
//         await Assert.That(array[2].Key).IsEqualTo(2);
//         await Assert.That(array[3]).IsEqualTo(default(Pair<int>));
//     }
//
// #endregion
//
// #region Integration Tests
//
//     [Test]
//     public async Task ComplexScenario_MixedOperations_WorksCorrectly()
//     {
//         var list = new OrderedList<int>(newestFirst: true);
//
//         // Add items
//         list.Add(3, "three-1");
//         list.Add(1, "one-1");
//         list.Add(2, "two-1");
//         list.Add(1, "one-2"); // Should be first among 1s
//         list.Add(3, "three-2"); // Should be first among 3s
//
//         // Verify order
//         await Assert.That(list.Count).IsEqualTo(5);
//         var items = list.ToList();
//         await Assert.That(items[0].Value).IsEqualTo("one-2");
//         await Assert.That(items[1].Value).IsEqualTo("one-1");
//         await Assert.That(items[2].Value).IsEqualTo("two-1");
//         await Assert.That(items[3].Value).IsEqualTo("three-2");
//         await Assert.That(items[4].Value).IsEqualTo("three-1");
//
//         // Remove one occurrence
//         list.RemoveFirst(1);
//         await Assert.That(list.Count).IsEqualTo(4);
//         await Assert.That(list.GetValues(1).Count).IsEqualTo(1);
//
//         // Remove all of key 3
//         var removedCount = list.RemoveAll(3);
//         await Assert.That(removedCount).IsEqualTo(2);
//         await Assert.That(list.Count).IsEqualTo(2);
//         await Assert.That(list.Contains(3)).IsFalse();
//
//         // Clear
//         list.Clear();
//         await Assert.That(list.Count).IsEqualTo(0);
//     }
//
// #endregion
// }