using NexusCore.Application.Common;

namespace NexusCore.Application.Holidays;

public interface IHolidayService
{
    Task<IReadOnlyList<HolidayResponse>> ListAsync(int? year, CancellationToken cancellationToken = default);
    Task<ServiceResult<HolidayResponse>> CreateAsync(CreateHolidayRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<HolidayResponse>> UpdateAsync(Guid id, UpdateHolidayRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
