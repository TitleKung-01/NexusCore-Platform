export interface MeResponse {
  userId: string
  username: string
  role: string
  fullName: string
  email: string
  phone: string | null
  departmentId: string
  departmentName: string
  managerId: string | null
  managerName: string | null
  canApprove: boolean
  isHrAccess: boolean
}

export interface RoleDefinition {
  id: string
  name: string
  description: string | null
  canApprove: boolean
  isHrAccess: boolean
  isBuiltIn: boolean
}

export interface CreateDepartmentRequest {
  name: string
  code: string
}

export interface CreateRoleRequest {
  name: string
  description?: string | null
  canApprove: boolean
  isHrAccess: boolean
}

export interface UpdateMeRequest {
  fullName: string
  email: string
  phone: string | null
}

export interface LoginResponse {
  token: string
  message: string
}

export interface ChangePasswordRequest {
  currentPassword: string
  newPassword: string
}

export interface Department {
  id: string
  name: string
  code: string
}

export interface LeaveType {
  id: string
  name: string
  code: string
}

export interface RequestEmployeeFields {
  employeeName: string
  employeeUsername?: string
  employeeEmail?: string
  employeeDepartment?: string
  employeeRole?: string
  employeeManagerName?: string | null
}

export interface LeaveRequest extends RequestEmployeeFields {
  id: string
  employeeId: string
  leaveTypeId: string
  leaveTypeName: string
  startDate: string
  endDate: string
  reason: string
  status: string
  submittedAtUtc: string | null
  decidedAtUtc: string | null
  managerComment: string | null
}

export interface CreateLeaveRequest {
  leaveTypeId: string
  startDate: string
  endDate: string
  reason: string
}

export interface LeaveBalance {
  leaveTypeId: string
  leaveTypeName: string
  year: number
  daysAllowed: number
  daysUsed: number
  daysRemaining: number
}

export interface LeaveCalendarEntry {
  id: string
  employeeId: string
  employeeName: string
  leaveTypeName: string
  startDate: string
  endDate: string
  status: string
}

export interface LeaveAttachment {
  id: string
  leaveRequestId: string
  fileName: string
  contentType: string
  sizeBytes: number
  uploadedAtUtc: string
}

export interface Notification {
  id: string
  eventType: string
  title: string
  body: string
  linkPath: string | null
  isRead: boolean
  createdAtUtc: string
}

export interface UnreadCountResponse {
  count: number
}

export interface Holiday {
  id: string
  date: string
  name: string
}

export interface CreateHolidayRequest {
  date: string
  name: string
}

export interface UpdateHolidayRequest {
  date: string
  name: string
}

export interface AttendanceRecord {
  id: string
  employeeId: string
  employeeName: string
  workDate: string
  checkInUtc: string | null
  checkOutUtc: string | null
  checkInLocal: string | null
  checkOutLocal: string | null
  isLateCheckIn: boolean
  lateMinutes: number
  isEarlyCheckOut: boolean
  statusLabel: string
  workSummary: string | null
  canCheckIn: boolean
  canCheckOut: boolean
}

export interface OvertimeRequest extends RequestEmployeeFields {
  id: string
  employeeId: string
  workDate: string
  hours: number
  reason: string
  status: string
  submittedAtUtc: string | null
  decidedAtUtc: string | null
  managerComment: string | null
}

export interface CreateOvertimeRequest {
  workDate: string
  hours: number
  reason: string
}

export interface ExpenseLineItem {
  id: string
  description: string
  amount: number
}

export interface ExpenseClaim extends RequestEmployeeFields {
  id: string
  employeeId: string
  title: string
  totalAmount: number
  status: string
  submittedAtUtc: string | null
  decidedAtUtc: string | null
  managerComment: string | null
  lineItems: ExpenseLineItem[]
}

export interface CreateExpenseLineItem {
  description: string
  amount: number
}

export interface CreateExpenseClaimRequest {
  title: string
  lineItems: CreateExpenseLineItem[]
}

export interface Payslip {
  id: string
  employeeId: string
  year: number
  month: number
  fileName: string
  publishedAtUtc: string
}

export interface EmployeeListItem {
  userId: string
  username: string
  role: string
  fullName: string
  email: string
  departmentName: string
  managerName: string | null
}

export interface UpdateEmployeeRequest {
  departmentId?: string | null
  managerId?: string | null
  role?: string | null
  fullName?: string | null
  email?: string | null
  isActive?: boolean | null
}

export interface EmployeeTransfer {
  id: string
  employeeId: string
  employeeName: string
  fromDepartment: string
  toDepartment: string
  effectiveDate: string
  note: string | null
  createdAtUtc: string
}

export interface Announcement {
  id: string
  title: string
  body: string
  authorId: string
  publishedAtUtc: string
  isActive: boolean
}

export interface CreateAnnouncementRequest {
  title: string
  body: string
}

export interface UpdateAnnouncementRequest {
  title: string
  body: string
  isActive: boolean
}

export interface OnboardingTemplateTask {
  id: string
  title: string
  sortOrder: number
}

export interface OnboardingTemplate {
  id: string
  name: string
  tasks: OnboardingTemplateTask[]
}

export interface EmployeeOnboardingTask {
  id: string
  employeeId: string
  templateId: string
  title: string
  isCompleted: boolean
  completedAtUtc: string | null
  sortOrder: number
}

export interface AssignOnboardingRequest {
  templateId: string
  employeeId: string
}
