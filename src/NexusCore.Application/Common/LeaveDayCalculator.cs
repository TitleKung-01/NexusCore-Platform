namespace NexusCore.Application.Common;

/// <summary>
/// คำนวณจำนวนวันลาแบบรวมวันเริ่มและวันสิ้นสุด (inclusive)
/// </summary>
public static class LeaveDayCalculator
{
    /// <summary>นับจำนวนวันลารวมทั้งวันแรกและวันสุดท้าย</summary>
    public static int CountInclusiveDays(DateOnly start, DateOnly end) =>
        end.DayNumber - start.DayNumber + 1;
}
