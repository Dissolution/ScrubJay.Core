﻿namespace ScrubJay.Extensions;

/// <summary>
/// Extensions for <see cref="DateTimeOffset"/>
/// </summary>
public static class DateTimeOffsetExtensions
{
    /// <summary>
    /// How much time has elapsed since this <see cref="DateTimeOffset"/> occured?
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <returns></returns>
    public static TimeSpan ElapsedSince(this DateTimeOffset dateTimeOffset)
    {
        return DateTimeOffset.Now - dateTimeOffset;
    }

#region XStart + XEnd
    /// <summary>
    /// Get the start of the day portion of the specified DateTimeOffset
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <returns></returns>
    public static DateTimeOffset DayStart(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.Date;
    }

    /// <summary>
    /// Get the end of the day portion of the specified DateTimeOffset
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <returns></returns>
    public static DateTimeOffset DayEnd(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// Gets the start of the week containing this DateTimeOffset.
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <param name="startOfWeek"></param>
    /// <returns></returns>
    public static DateTimeOffset WeekStart(this DateTimeOffset dateTimeOffset, DayOfWeek startOfWeek = DayOfWeek.Sunday)
    {
        int diff = dateTimeOffset.DayOfWeek - startOfWeek;
        if (diff < 0)
            diff += 7;
        return dateTimeOffset.AddDays(-1 * diff).Date;
    }

    /// <summary>
    /// Gets the start of the week containing this DateTimeOffset.
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <param name="endOfWeek"></param>
    /// <returns></returns>
    public static DateTimeOffset WeekEnd(this DateTimeOffset dateTimeOffset, DayOfWeek endOfWeek = DayOfWeek.Saturday)
    {
        int diff = endOfWeek - dateTimeOffset.DayOfWeek;
        return dateTimeOffset.AddDays(diff).Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// Gets the start of the month containing this DateTimeOffset.
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <returns></returns>
    public static DateTimeOffset MonthStart(this DateTimeOffset dateTimeOffset)
    {
        return new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, 1, 0, 0, 0, dateTimeOffset.Offset).Date;
    }

    /// <summary>
    /// Gets the end of the month containing this DateTimeOffset.
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <returns></returns>
    public static DateTimeOffset MonthEnd(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.MonthStart().AddMonths(1).AddTicks(-1);
    }

    /// <summary>
    /// Gets the start of the year containing this DateTimeOffset.
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <returns></returns>
    public static DateTimeOffset YearStart(this DateTimeOffset dateTimeOffset)
    {
        return new DateTimeOffset(dateTimeOffset.Year, 1, 1, 0, 0, 0, dateTimeOffset.Offset).Date;
    }

    /// <summary>
    /// Gets the end of the year containing this DateTimeOffset.
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <returns></returns>
    public static DateTimeOffset YearEnd(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.YearStart().AddYears(1).AddTicks(-1);
    }
#endregion

#region Rounding
    /// <summary>
    /// Drop this <see cref="DateTimeOffset"/> to the earliest <see cref="TimeSpan"/> precision floor.
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static DateTimeOffset Floor(this DateTimeOffset dateTimeOffset, TimeSpan precision)
    {
        long delta = dateTimeOffset.Ticks % precision.Ticks;
        return dateTimeOffset.AddTicks(-delta);
    }

    /// <summary>
    /// Round this <see cref="DateTimeOffset"/> to the specified <see cref="TimeSpan"/> precision.
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static DateTimeOffset Round(this DateTimeOffset dateTimeOffset, TimeSpan precision)
    {
        long delta = dateTimeOffset.Ticks % precision.Ticks;
        bool shouldRoundUp = delta > precision.Ticks / 2L;
        long offset = shouldRoundUp ? precision.Ticks : 0L;
        return dateTimeOffset.AddTicks(offset - delta);
    }


    /// <summary>
    /// Raise this <see cref="DateTimeOffset"/> to the latest <see cref="TimeSpan"/> precision ceiling.
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static DateTimeOffset Ceiling(this DateTimeOffset dateTimeOffset, TimeSpan precision)
    {
        long delta = dateTimeOffset.Ticks % precision.Ticks;
        return dateTimeOffset.AddTicks(precision.Ticks - delta);
    }
#endregion
}