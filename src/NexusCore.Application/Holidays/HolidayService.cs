using NexusCore.Application.Common;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Holidays;

/// <summary>
/// จัดการปฏิทินวันหยุดบริษัทสำหรับ HR
/// </summary>
public class HolidayService(
    ICurrentUserService currentUser,
    ICompanyHolidayRepository holidays) : IHolidayService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<HolidayResponse>> ListAsync(int? year, CancellationToken cancellationToken = default)
    {
        var list = await holidays.ListAsync(year, cancellationToken);
        return list.Select(Map).ToList();
    }

    /// <inheritdoc />
    public async Task<ServiceResult<HolidayResponse>> CreateAsync(CreateHolidayRequest request, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return ServiceResult<HolidayResponse>.Fail("Forbidden.", 403);

        if (!DateOnly.TryParse(request.Date, out var date))
            return ServiceResult<HolidayResponse>.Fail("Invalid date format. Use yyyy-MM-dd.", 400);

        var entity = new CompanyHoliday
        {
            Id = Guid.NewGuid(),
            Date = date,
            Name = request.Name.Trim()
        };
        await holidays.AddAsync(entity, cancellationToken);
        await holidays.SaveChangesAsync(cancellationToken);
        return ServiceResult<HolidayResponse>.Ok(Map(entity));
    }

    /// <inheritdoc />
    public async Task<ServiceResult<HolidayResponse>> UpdateAsync(Guid id, UpdateHolidayRequest request, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return ServiceResult<HolidayResponse>.Fail("Forbidden.", 403);

        var entity = await holidays.FindByIdTrackedAsync(id, cancellationToken);
        if (entity is null)
            return ServiceResult<HolidayResponse>.Fail("Holiday not found.", 404);

        if (!DateOnly.TryParse(request.Date, out var date))
            return ServiceResult<HolidayResponse>.Fail("Invalid date format. Use yyyy-MM-dd.", 400);

        entity.Date = date;
        entity.Name = request.Name.Trim();
        await holidays.SaveChangesAsync(cancellationToken);
        return ServiceResult<HolidayResponse>.Ok(Map(entity));
    }

    /// <inheritdoc />
    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return ServiceResult<bool>.Fail("Forbidden.", 403);

        var entity = await holidays.FindByIdTrackedAsync(id, cancellationToken);
        if (entity is null)
            return ServiceResult<bool>.Fail("Holiday not found.", 404);

        holidays.Remove(entity);
        await holidays.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Ok(true);
    }

    private static HolidayResponse Map(CompanyHoliday h) =>
        new(h.Id, h.Date.ToString("yyyy-MM-dd"), h.Name);
}