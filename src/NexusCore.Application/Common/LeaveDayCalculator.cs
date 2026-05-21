namespace NexusCore.Application.Common;

public static class LeaveDayCalculator
{
    public static int CountInclusiveDays(DateOnly start, DateOnly end) =>
        end.DayNumber - start.DayNumber + 1;
}
