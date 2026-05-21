namespace NexusCore.Application.Holidays;

public record HolidayResponse(Guid Id, string Date, string Name);

public record CreateHolidayRequest(string Date, string Name);

public record UpdateHolidayRequest(string Date, string Name);
