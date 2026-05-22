namespace NexusCore.Application.Common;

/// <summary>
/// ผลลัพธ์มาตรฐานของ use case พร้อมสถานะสำเร็จ ข้อมูล ข้อความผิดพลาด และรหัส HTTP
/// </summary>
public record ServiceResult<T>(bool Success, T? Data, string? Error, int StatusCode = 400)
{
    /// <summary>สร้างผลลัพธ์สำเร็จพร้อมข้อมูล</summary>
    public static ServiceResult<T> Ok(T data) => new(true, data, null, 200);

    /// <summary>สร้างผลลัพธ์ล้มเหลวพร้อมข้อความและรหัสสถานะ</summary>
    public static ServiceResult<T> Fail(string error, int statusCode = 400) => new(false, default, error, statusCode);
}
