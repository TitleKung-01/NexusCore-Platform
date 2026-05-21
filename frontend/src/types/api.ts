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
}

export interface LeaveType {
  id: string
  name: string
  code: string
}

export interface LeaveRequest {
  id: string
  employeeId: string
  employeeName: string
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

export interface LoginResponse {
  token: string
  message: string
}
