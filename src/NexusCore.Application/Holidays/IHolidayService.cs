using NexusCore.Application.Common;

namespace NexusCore.Application.Holidays;

/// <summary>
/// บริการจัดการปฏิทินวันหยุดบริษัท (เฉพาะ HR/Admin สร้าง/แก้/ลบ)
/// </summary>
public interface IHolidayService
{
    /// <summary>ดึงรายการวันหยุด (กรองตามปีได้)</summary>
    Task<IReadOnlyList<HolidayResponse>> ListAsync(int? year, CancellationToken cancellationToken = default);

    /// <summary>เพิ่มวันหยุดใหม่</summary>
    Task<ServiceResult<HolidayResponse>> CreateAsync(CreateHolidayRequest request, CancellationToken cancellationToken = default);

    /// <summary>แก้ไขวันหยุด</summary>
    Task<ServiceResult<HolidayResponse>> UpdateAsync(Guid id, UpdateHolidayRequest request, CancellationToken cancellationToken = default);

    /// <summary>ลบวันหยุด</summary>
    Task<ServiceResult<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
