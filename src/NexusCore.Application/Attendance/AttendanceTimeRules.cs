namespace NexusCore.Application.Attendance;

public static class AttendanceTimeRules
{
    public static readonly TimeOnly ScheduledCheckIn = new(9, 0);
    public static readonly TimeOnly ScheduledCheckOut = new(18, 0);

    private static readonly TimeZoneInfo Bangkok = ResolveBangkokTimeZone();

    public static DateTime ToBangkokLocal(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Bangkok);

    public static string FormatLocalTime(DateTime utc) =>
        ToBangkokLocal(utc).ToString("HH:mm");

    public static bool IsLateCheckIn(DateTime checkInUtc) =>
        TimeOnly.FromDateTime(ToBangkokLocal(checkInUtc)) > ScheduledCheckIn;

    public static int LateMinutes(DateTime checkInUtc)
    {
        var local = TimeOnly.FromDateTime(ToBangkokLocal(checkInUtc));
        if (local <= ScheduledCheckIn)
            return 0;
        return (int)Math.Round((local.ToTimeSpan() - ScheduledCheckIn.ToTimeSpan()).TotalMinutes);
    }

    public static bool IsEarlyCheckOut(DateTime checkOutUtc) =>
        TimeOnly.FromDateTime(ToBangkokLocal(checkOutUtc)) < ScheduledCheckOut;

    public static string BuildStatusLabel(DateTime? checkInUtc, DateTime? checkOutUtc)
    {
        if (checkInUtc is null)
            return "ยังไม่ลงเวลาเข้า";
        if (checkOutUtc is null)
            return "พร้อมลงเวลาออก";

        var late = IsLateCheckIn(checkInUtc.Value);
        var early = IsEarlyCheckOut(checkOutUtc.Value);
        if (late && early)
            return "เข้าสาย · ออกก่อนเวลา";
        if (late)
            return "เข้าสาย";
        if (early)
            return "ออกก่อนเวลา";
        return "ตรงเวลา";
    }

    private static TimeZoneInfo ResolveBangkokTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.CreateCustomTimeZone("Asia/Bangkok", TimeSpan.FromHours(7), "Bangkok", "Bangkok");
        }
    }
}
