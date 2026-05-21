namespace NexusCore.Application.Attendance;

public record AttendanceResponse(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    string WorkDate,
    string? CheckInUtc,
    string? CheckOutUtc);

public record CheckInRequest(string WorkDate);

public record CheckOutRequest(string WorkDate);
