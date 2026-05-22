using FluentValidation;
using NexusCore.Application.Leave;

namespace NexusCore.Application.Validators;

/// <summary>
/// ตรวจสอบคำขอสร้างใบลา (ประเภท วันที่ เหตุผล)
/// </summary>
public class CreateLeaveRequestValidator : AbstractValidator<CreateLeaveRequest>
{
    /// <summary>กำหนดกฎฟิลด์ใบลา</summary>
    public CreateLeaveRequestValidator()
    {
        RuleFor(x => x.LeaveTypeId).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}
