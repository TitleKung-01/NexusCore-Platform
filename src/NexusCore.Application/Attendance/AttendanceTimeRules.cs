namespace NexusCore.Application.Attendance;

/// <summary>
/// กฎเวลาเข้างานมาตรฐาน (09:00–18:00 กรุงเทพ) และป้ายสถานะภาษาไทย
/// </summary>
public static class AttendanceTimeRules
{
    /// <summary>เวลาเข้างานตามกำหนด</summary>
    public static readonly TimeOnly ScheduledCheckIn = new(9, 0);

    /// <summary>เวลาออกงานตามกำหนด</summary>
    public static readonly TimeOnly ScheduledCheckOut = new(18, 0);

    private static readonly TimeZoneInfo Bangkok = ResolveBangkokTimeZone();

    /// <summary>แปลง UTC เป็นเวลาท้องถิ่นกรุงเทพ</summary>
    public static DateTime ToBangkokLocal(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Bangkok);

    /// <summary>จัดรูปแบบเวลาท้องถิ่น HH:mm</summary>
    public static string FormatLocalTime(DateTime utc) =>
        ToBangkokLocal(utc).ToString("HH:mm");

    /// <summary>ตรวจว่าลงเวลาเข้าสายหรือไม่</summary>
    public static bool IsLateCheckIn(DateTime checkInUtc) =>
        TimeOnly.FromDateTime(ToBangkokLocal(checkInUtc)) > ScheduledCheckIn;

    /// <summary>คำนวณนาทีที่สายจากเวลาเข้างาน</summary>
    public static int LateMinutes(DateTime checkInUtc)
    {
        var local = TimeOnly.FromDateTime(ToBangkokLocal(checkInUtc));
        if (local <= ScheduledCheckIn)
            return 0;
        return (int)Math.Round((local.ToTimeSpan() - ScheduledCheckIn.ToTimeSpan()).TotalMinutes);
    }

    /// <summary>ตรวจว่าลงเวลาออกก่อนเวลาหรือไม่</summary>
    public static bool IsEarlyCheckOut(DateTime checkOutUtc) =>
        TimeOnly.FromDateTime(ToBangkokLocal(checkOutUtc)) < ScheduledCheckOut;

    /// <summary>สร้างป้ายสถานะภาษาไทยจากเวลาเข้า-ออก</summary>
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
