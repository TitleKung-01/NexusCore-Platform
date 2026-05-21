namespace NexusCore.Domain.Constants;

public static class UserRoles
{
    public const string Employee = "Employee";
    public const string Manager = "Manager";
    public const string Hr = "Hr";
    public const string Admin = "Admin";

    public static readonly string[] All = [Employee, Manager, Hr, Admin];
    public static readonly string[] Approvers = [Manager, Hr, Admin];
    public static readonly string[] HrOrAdmin = [Hr, Admin];
}
