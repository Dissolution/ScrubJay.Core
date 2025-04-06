// using System.Runtime.Serialization;
//
// namespace ScrubJay.Collections;
//
// /// <summary>
// /// a <b>C</b>ircular, <b>D</b>oubly-<b>L</b>inked <b>L</b>ist
// /// </summary>
// /// <typeparam name="T"></typeparam>
// /// <see href="https://en.wikipedia.org/wiki/Doubly_linked_list"/>
// [DebuggerDisplay("Count = {Count}")]
// public class CDLL<T> : ICollection<T>, ICollection, IReadOnlyCollection<T>
// {
//     private Node? _headNode;
//     private int _count;
//     private int _version;
//
//     public int Count => _count;
//
//     public Node? First => _headNode;
//
//     public Node? Last => _headNode?._previousNode;
//
//     public CDLL() { }
//
//     public CDLL(IEnumerable<T> collection)
//     {
//         ArgumentNullException.ThrowIfNull(collection);
//
//         foreach (T item in collection)
//         {
//             AddLast(item);
//         }
//     }
//
//     private void AddNodeWhenEmpty(Node node)
//     {
//         Debug.Assert((_headNode == null) && (_count == 0));
//
//         _version++;
//         node._nextNode = node;
//         node._previousNode = node;
//         _headNode = node;
//         _count++;
//     }
//
//     private void InsertNodeBefore(Node targetNode, Node newNode)
//     {
//         _version++;
//         newNode._nextNode = targetNode;
//         newNode._previousNode = targetNode._previousNode;
//         targetNode._previousNode!._nextNode = newNode;
//         targetNode._previousNode = newNode;
//         _count++;
//     }
//
//     internal void DeleteNode(Node node)
//     {
//         Debug.Assert(node._linkedList == this, "Deleting the node from another list!");
//         Debug.Assert(_headNode != null, "This method shouldn't be called on empty list!");
//         if (node._nextNode == node)
//         {
//             Debug.Assert(_count == 1 && _headNode == node, "this should only be true for a list with only one node");
//             _headNode = null;
//         }
//         else
//         {
//             node._nextNode!._previousNode = node._previousNode;
//             node._previousNode!._nextNode = node._nextNode;
//             if (_headNode == node)
//             {
//                 _headNode = node._nextNode;
//             }
//         }
//         node.Invalidate();
//         _count--;
//         _version++;
//     }
//
//
//     void ICollection<T>.Add(T value) => AddLast(value);
//
//     public Node AddLast(T value)
//     {
//         Node node = new Node(this, value);
//         if (_headNode == null)
//         {
//             AddNodeWhenEmpty(node);
//         }
//         else
//         {
//             // circular, so insert before head is insert after last
//             InsertNodeBefore(_headNode, node);
//         }
//         return node;
//     }
//
//     public Node AddFirst(T value)
//     {
//         Node node = new Node(this, value);
//         if (_headNode == null)
//         {
//             AddNodeWhenEmpty(node);
//         }
//         else
//         {
//             // insert before head, then shift that to be head
//             InsertNodeBefore(_headNode, node);
//             _headNode = node;
//         }
//         return node;
//     }
//
//     public void Clear()
//     {
//         _version++;
//         Node? current = _headNode;
//         Node temp;
//         while (current != null)
//         {
//             temp = current;
//             current = current.Next;
//             temp.Dispose();
//         }
//
//         _headNode = null;
//         _count = 0;
//     }
//
//
//     private Option<Node> TryFindFirst(T value, IEqualityComparer<T>? valueComparer = null)
//     {
//         Node? node = _headNode;
//         var comparer = valueComparer ?? EqualityComparer<T>.Default;
//         if (node is not null)
//         {
//             do
//             {
//                 if (comparer.Equals(node!.Value!, value!))
//                 {
//                     return Some(node);
//                 }
//                 node = node._nextNode;
//             } while (node != _headNode);
//         }
//         return None<Node>();
//     }
//
//     public Option<Node> TryFindLast(T value, IEqualityComparer<T>? valueComparer = null)
//     {
//         if (_headNode == null)
//             return None();
//
//         Node? last = _headNode._previousNode;
//         Node? node = last;
//         var comparer = valueComparer ?? EqualityComparer<T>.Default;
//         if (node is not null)
//         {
//             do
//             {
//                 if (comparer.Equals(node!.Value, value!))
//                 {
//                     return Some(node);
//                 }
//
//                 node = node._previousNode;
//             } while (node != last);
//         }
//         return None();
//     }
//
//     public Option<Node> TryFind(
//         T value,
//         bool firstToLast = true,
//         IEqualityComparer<T>? valueComparer = null)
//     {
//         if (firstToLast)
//             return TryFindFirst(value, valueComparer);
//         return TryFindLast(value, valueComparer);
//     }
//
//     public bool Contains(T value, IEqualityComparer<T>? valueComparer = null)
//         => TryFindFirst(value, valueComparer).IsSome();
//
//     void ICollection<T>.CopyTo(T[] array, int index)
//     {
//         ArgumentNullException.ThrowIfNull(array);
//
//         ArgumentOutOfRangeException.ThrowIfNegative(index);
//
//         if (index > array.Length)
//         {
//             throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_BiggerThanCollection);
//         }
//
//         if (array.Length - index < Count)
//         {
//             throw new ArgumentException(SR.Arg_InsufficientSpace);
//         }
//
//         Node? node = _headNode;
//         if (node != null)
//         {
//             do
//             {
//                 array[index++] = node!._item;
//                 node = node._nextNode;
//             } while (node != _headNode);
//         }
//     }
//
//     public Result<int> TryCopyTo(Span<T> span)
//     {
//         if (Validate.CanCopyTo(span, _count).IsError(out var ex))
//             return ex;
//
//         Node? node = _headNode;
//         int i = 0;
//
//         if (node is not null)
//         {
//             do
//             {
//                 span[i++] = node!.Value;
//                 node = node._nextNode;
//             } while (node != _headNode);
//         }
//
//         return _count;
//     }
//
//
//     public bool Remove(T value)
//     {
//         Node? node = TryFindFirst(value);
//         if (node != null)
//         {
//             DeleteNode(node);
//             return true;
//         }
//         return false;
//     }
//
//     public void Remove(Node node)
//     {
//         ValidateNode(node);
//         DeleteNode(node);
//     }
//
//     public void RemoveFirst()
//     {
//         if (_headNode == null) { throw new InvalidOperationException(SR.LinkedListEmpty); }
//         DeleteNode(_headNode);
//     }
//
//     public void RemoveLast()
//     {
//         if (_headNode == null) { throw new InvalidOperationException(SR.LinkedListEmpty); }
//         DeleteNode(_headNode._previousNode!);
//     }
//
//
//
//
//     internal static void ValidateNewNode(Node node)
//     {
//         ArgumentNullException.ThrowIfNull(node);
//
//         if (node._linkedList != null)
//         {
//             throw new InvalidOperationException(SR.LinkedListNodeIsAttached);
//         }
//     }
//
//     internal void ValidateNode(Node node)
//     {
//         ArgumentNullException.ThrowIfNull(node);
//
//         if (node._linkedList != this)
//         {
//             throw new InvalidOperationException(SR.ExternalLinkedListNode);
//         }
//     }
//
//
//     IEnumerator IEnumerable.GetEnumerator() => _count == 0 ? Enumerator.Empty<T>() : GetEnumerator();
//     IEnumerator<T> IEnumerable<T>.GetEnumerator() => _count == 0 ? Enumerator.Empty<T>() : GetEnumerator();
//     public CDLLEnumerator GetEnumerator() => new CDLLEnumerator(this);
//
//     [MustDisposeResource(false)]
//     public sealed class CDLLEnumerator : IEnumerator<T>, IEnumerator, IDisposable
//     {
//         private readonly CDLL<T> _cdlList;
//
//         private readonly int _count;
//         private readonly int _version;
//
//         private Node? _currentNode;
//         private int _index;
//
//         object? IEnumerator.Current => Current;
//
//         public T Current
//         {
//             get
//             {
//                 Throw.IfBadEnumerationState(_index < 0, _index >= _count);
//                 Debug.Assert(_currentNode is not null);
//                 return _currentNode!.Value;
//             }
//         }
//
//         internal CDLLEnumerator(CDLL<T> cdlList)
//         {
//             _cdlList = cdlList;
//             _count = cdlList.Count;
//             _version = cdlList._version;
//             _currentNode = null;
//             _index = -1;
//         }
//
//         void IDisposable.Dispose() { }
//
//         public bool MoveNext()
//         {
//             Throw.IfEnumerationSourceHasChanged(_version != _cdlList._version);
//
//             int nextIndex = _index + 1;
//
//             // If we are at the start, emit the head node
//             if (nextIndex == 0)
//             {
//                 _currentNode = _cdlList._headNode;
//                 _index = nextIndex;
//                 return true;
//             }
//             // if we are after the end, fail
//             else if (nextIndex >= _count)
//             {
//                 Debug.Assert(_currentNode is null || _currentNode.IsLast);
//                 _currentNode = null;
//                 return false;
//             }
//             // otherwise, the New Node is the Current Node's Next Node (!?)
//             else
//             {
//                 Debug.Assert(_currentNode is not null);
//                 _currentNode = _currentNode!._nextNode;
//                 _index = nextIndex;
//                 return true;
//             }
//         }
//
//         public void Reset()
//         {
//             Throw.IfEnumerationSourceHasChanged(_version != _cdlList._version);
//             _currentNode = null;
//             _index = -1;
//         }
//     }
//
//
//     [MustDisposeResource(false)]
//     public sealed class Node :
// #if NET7_0_OR_GREATER
//         IEqualityOperators<Node, Node, bool>,
// #endif
//         IEquatable<Node>,
//         IDisposable
//     {
//         public static bool operator ==(Node? left, Node? right) => ReferenceEquals(left, right);
//         public static bool operator !=(Node? left, Node? right) => !ReferenceEquals(left, right);
//
//         private CDLL<T>? _linkedList;
//         private T _value;
//         internal Node? _nextNode;
//         internal Node? _previousNode;
//
//         public Node? Next
//         {
//             get
//             {
//                 if (_nextNode is null || (_nextNode == _linkedList?._headNode)) // if our next node is the head node, we're lastreturn _nextNode;
//                     return null;
//                 else
//                     return _nextNode;
//             }
//         }
//
//         public Node? Previous
//         {
//             get
//             {
//                 if (_previousNode is null || (this == _linkedList?._headNode)) // if we're the head, our previous is actually the last
//                     return null;
//                 else
//                     return _previousNode;
//             }
//         }
//
//         public bool IsFirst => this == _linkedList?._headNode;
//
//         public bool IsLast => _nextNode == _linkedList?._headNode;
//
//         public T Value
//         {
//             get => _value;
//             set => _value = value;
//         }
//
//         public ref T RefValue => ref _value;
//
//         internal Node(CDLL<T> linkedList, T value)
//         {
//             _linkedList = linkedList;
//             _value = value;
//         }
//
//         public Node AddAfter(T value)
//         {
//             Throw.IfDisposed(_linkedList is null, this);
//             Node newNode = new Node(_linkedList!, value);
//             _linkedList.InsertNodeBefore(_nextNode!, newNode);
//             return newNode;
//         }
//
//         public Node AddBefore(T value)
//         {
//             Throw.IfDisposed(_linkedList is null, this);
//             if (this == _linkedList._headNode)
//             {
//                 return _linkedList.AddFirst(value);
//             }
//             else
//             {
//                 Node newNode = new Node(_linkedList!, value);
//                 _linkedList.InsertNodeBefore(this, newNode);
//                 return newNode;
//             }
//         }
//
//         public void Dispose()
//         {
//             _linkedList = null;
//             _value = default!;
//             _nextNode = null;
//             _previousNode = null;
//         }
//
//         public bool Equals(Node? node)
//             => ReferenceEquals(this, node);
//
//         public override bool Equals([NotNullWhen(true)] object? obj)
//             => obj is Node node && Equals(node);
//
//         public override int GetHashCode()
//             => Throw.NotSupported<int>($"A {typeof(Node).NameOf()} should only be stored in a {typeof(CDLL<T>).NameOf()}");
//
//         public override string ToString()
//         {
//             return TextBuilder.New
//                 .AppendIf(IsFirst, "(", "…)·(")
//                 .Append(_value)
//                 .AppendIf(IsLast, ")", ")·(…")
//                 .ToStringAndDispose();
//         }
//     }
// }
