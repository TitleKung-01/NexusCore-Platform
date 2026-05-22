namespace NexusCore.Application.Holidays;

/// <summary>วันหยุดบริษัทสำหรับแสดงในรายการ</summary>
public record HolidayResponse(Guid Id, string Date, string Name);

/// <summary>คำขอเพิ่มวันหยุดบริษัท</summary>
public record CreateHolidayRequest(string Date, string Name);

/// <summary>คำขอแก้ไขวันหยุดบริษัท</summary>
public record UpdateHolidayRequest(string Date, string Name);
