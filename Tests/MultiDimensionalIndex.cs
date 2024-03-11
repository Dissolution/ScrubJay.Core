using System.Diagnostics.CodeAnalysis;

namespace ScrubJay.Tests;

internal sealed class CombinationIndex
{
    private readonly int[] _indices;
    private readonly int _argumentCount;
    private readonly int _lastItemIndex;
    
    public CombinationIndex(int argumentCount, int itemCount)
    {
        Debug.Assert(argumentCount > 0);
        Debug.Assert(itemCount > 0);
        _argumentCount = argumentCount;
        _lastItemIndex = itemCount - 1;
        _indices = new int[argumentCount];
        Reset();
    }

    private bool TryIncrementAt(int arg)
    {
        if (arg < 0 || arg >= _argumentCount) return false;

        var indices = _indices;
        var argItemIndex = indices[arg];
        
        // is this arg
        if (argItemIndex >= _lastItemIndex)
        {
            // Try to increment the next argument (right to left)
            if (!TryIncrementAt(arg - 1))
            {
                // Nothing can increment
                return false;
            }
            // Someone else incremented, reset this arg index
            indices[arg] = 0;
            return true;
        }
        
        // can increment
        indices[arg] = argItemIndex + 1;
        return true;
    }
    
    public void Reset()
    {
        _indices[^1] = -1;
        for (var i = _argumentCount - 2; i >= 0; i--)
        {
            _indices[i] = 0;
        }
    }

    public bool TryGetNext([NotNullWhen(true)] out int[]? indices)
    {
        // right to left
        if (TryIncrementAt(_argumentCount - 1))
        {
            indices = _indices;
            return true;
        }
        indices = null;
        return false;
    }
}