﻿namespace ScrubJay.Extensions;

/// <summary>
/// Extensions for <see cref="IComparable{T}"/> and <see cref="IComparable"/> values.
/// </summary>
public static class ComparableExtensions
{
    /// <summary>
    /// Limit this <see cref="IComparable{T}"/> value between a minimum and maximum value.
    /// </summary>
    /// <param name="value">The <see cref="IComparable{T}"/> value to limit.</param>
    /// <param name="minimum">The minimum inclusive value it can be.</param>
    /// <param name="maximum">The maximum inclusive value it can be.</param>
    public static void Clamp<T>(ref T value, T minimum, T maximum)
        where T : IComparable<T>
    {
        if (value.CompareTo(minimum) < 0)
        {
            value = minimum;
        }
        else if (value.CompareTo(maximum) > 0)
        {
            value = maximum;
        }
    }

    /// <summary>
    /// Limit this <see cref="IComparable{T}"/> value between a minimum and maximum value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The <see cref="IComparable{T}"/> value to limit.</param>
    /// <param name="minimum">The minimum inclusive value it can be.</param>
    /// <param name="maximum">The maximum inclusive value it can be.</param>
    /// <returns></returns>
    public static T Clamped<T>(this T value, T minimum, T maximum)
        where T : IComparable<T>
    {
        if (value.CompareTo(minimum) < 0) return minimum;
        if (value.CompareTo(maximum) > 0) return maximum;
        return value;
    }

    /// <summary>
    /// Is this <see cref="IComparable{T}"/> value between a minimum and maximum value?
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The <see cref="IComparable{T}"/> value to check.</param>
    /// <param name="minimum">The minimum inclusive value it can be.</param>
    /// <param name="maximum">The maximum inclusive value it can be.</param>
    /// <returns></returns>
    public static bool IsBetween<T>(this T value, T minimum, T maximum)
        where T : IComparable<T>
    {
        if (value.CompareTo(minimum) < 0) return false;
        if (value.CompareTo(maximum) > 0) return false;
        return true;
    }
}