import type { RequestEmployeeInfoData } from '@/features/shared/RequestEmployeeInfo'

export function EmployeeTableCell({ employee }: { employee: RequestEmployeeInfoData }) {
  return (
    <div>
      <p className="font-medium">{employee.employeeName}</p>
      <p className="text-xs text-muted-foreground">
        {[employee.employeeDepartment, employee.employeeUsername].filter(Boolean).join(' · ') || '—'}
      </p>
    </div>
  )
}
