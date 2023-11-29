﻿namespace ScrubJay.Extensions;

/// <summary>
/// Extensions for <see cref="DateTime"/>
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// How much time has elapsed since this <see cref="DateTime"/> occured?
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static TimeSpan ElapsedSince(this DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc)
            return DateTime.UtcNow - dateTime;
        return DateTime.Now - dateTime;
    }

    /// <summary>
    /// Creates a new <see cref="DateTime"/> that is this <see cref="DateTime"/> as the specified <see cref="DateTimeKind"/>
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    public static DateTime AsKind(this DateTime dateTime, DateTimeKind kind)
    {
        return DateTime.SpecifyKind(dateTime, kind);
    }

#region XStart + XEnd
    /// <summary>
    /// Get the start of the day portion of the specified DateTime
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime DayStart(this DateTime dateTime)
    {
        return dateTime.Date;
    }

    /// <summary>
    /// Get the end of the day portion of the specified DateTime
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime DayEnd(this DateTime dateTime)
    {
        return dateTime.Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// Gets the start of the week containing this DateTime.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="startOfWeek"></param>
    /// <returns></returns>
    public static DateTime WeekStart(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Sunday)
    {
        int diff = dateTime.DayOfWeek - startOfWeek;
        if (diff < 0)
            diff += 7;
        return dateTime.AddDays(-1 * diff).Date;
    }

    /// <summary>
    /// Gets the start of the week containing this DateTime.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="endOfWeek"></param>
    /// <returns></returns>
    public static DateTime WeekEnd(this DateTime dateTime, DayOfWeek endOfWeek = DayOfWeek.Saturday)
    {
        int diff = endOfWeek - dateTime.DayOfWeek;
        return dateTime.AddDays(diff).Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// Gets the start of the month containing this DateTime.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime MonthStart(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, 1).Date;
    }

    /// <summary>
    /// Gets the end of the month containing this DateTime.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime MonthEnd(this DateTime dateTime)
    {
        return dateTime.MonthStart().AddMonths(1).AddTicks(-1);
    }

    /// <summary>
    /// Gets the start of the year containing this DateTime.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime YearStart(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, 1, 1).Date;
    }

    /// <summary>
    /// Gets the end of the year containing this DateTime.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime YearEnd(this DateTime dateTime)
    {
        return dateTime.YearStart().AddYears(1).AddTicks(-1);
    }
#endregion

#region Rounding
    /// <summary>
    /// Drop this <see cref="DateTime"/> to the earliest <see cref="TimeSpan"/> precision floor.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static DateTime Floor(this DateTime dateTime, TimeSpan precision)
    {
        long delta = dateTime.Ticks % precision.Ticks;
        return dateTime.AddTicks(-delta);
    }

    /// <summary>
    /// Round this <see cref="DateTime"/> to the specified <see cref="TimeSpan"/> precision.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static DateTime Round(this DateTime dateTime, TimeSpan precision)
    {
        long delta = dateTime.Ticks % precision.Ticks;
        bool shouldRoundUp = delta > precision.Ticks / 2L;
        long offset = shouldRoundUp ? precision.Ticks : 0L;
        return dateTime.AddTicks(offset - delta);
    }

    /// <summary>
    /// Raise this <see cref="DateTime"/> to the latest <see cref="TimeSpan"/> precision ceiling.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static DateTime Ceiling(this DateTime dateTime, TimeSpan precision)
    {
        long delta = dateTime.Ticks % precision.Ticks;
        return dateTime.AddTicks(precision.Ticks - delta);
    }
#endregion
}