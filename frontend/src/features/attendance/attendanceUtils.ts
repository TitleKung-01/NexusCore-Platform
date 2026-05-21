import type { AttendanceRecord } from '@/types/api'

/** รองรับทั้ง camelCase และ PascalCase จาก API */
export function normalizeAttendance(raw: unknown): AttendanceRecord | null {
  if (!raw || typeof raw !== 'object') return null
  const r = raw as Record<string, unknown>
  const id = r.id ?? r.Id
  if (id == null) return null

  const checkInUtc = (r.checkInUtc ?? r.CheckInUtc ?? null) as string | null
  const checkOutUtc = (r.checkOutUtc ?? r.CheckOutUtc ?? null) as string | null
  const canCheckIn =
    typeof r.canCheckIn === 'boolean'
      ? r.canCheckIn
      : typeof r.CanCheckIn === 'boolean'
        ? r.CanCheckIn
        : !checkInUtc
  const canCheckOut =
    typeof r.canCheckOut === 'boolean'
      ? r.canCheckOut
      : typeof r.CanCheckOut === 'boolean'
        ? r.CanCheckOut
        : Boolean(checkInUtc) && !checkOutUtc

  return {
    id: String(id),
    employeeId: String(r.employeeId ?? r.EmployeeId ?? ''),
    employeeName: String(r.employeeName ?? r.EmployeeName ?? ''),
    workDate: String(r.workDate ?? r.WorkDate ?? ''),
    checkInUtc,
    checkOutUtc,
    checkInLocal: (r.checkInLocal ?? r.CheckInLocal ?? null) as string | null,
    checkOutLocal: (r.checkOutLocal ?? r.CheckOutLocal ?? null) as string | null,
    isLateCheckIn: Boolean(r.isLateCheckIn ?? r.IsLateCheckIn),
    lateMinutes: Number(r.lateMinutes ?? r.LateMinutes ?? 0),
    isEarlyCheckOut: Boolean(r.isEarlyCheckOut ?? r.IsEarlyCheckOut),
    statusLabel: String(r.statusLabel ?? r.StatusLabel ?? ''),
    workSummary: (r.workSummary ?? r.WorkSummary ?? null) as string | null,
    canCheckIn,
    canCheckOut,
  }
}
