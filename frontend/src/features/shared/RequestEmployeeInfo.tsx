import { Building2, Mail, User, UserCheck } from 'lucide-react'
import { Card, CardContent } from '@/components/ui/card'

export interface RequestEmployeeInfoData {
  employeeName: string
  employeeUsername?: string | null
  employeeEmail?: string | null
  employeeDepartment?: string | null
  employeeRole?: string | null
  employeeManagerName?: string | null
}

export function RequestEmployeeInfo({ employee }: { employee: RequestEmployeeInfoData }) {
  // Get initials for the badge avatar
  const getInitials = (name: string) => {
    return name.split(' ').map(n => n[0]).slice(0, 2).join('').toUpperCase()
  }

  return (
    <Card className="overflow-hidden border border-blue-500/10 shadow-md shadow-blue-500/[0.02] relative bg-card">
      {/* Premium left-side accent gradient line */}
      <div className="absolute left-0 top-0 bottom-0 w-1.5 bg-gradient-to-b from-blue-500 via-indigo-500 to-sky-400" />
      
      <CardContent className="p-5">
        <div className="flex flex-col md:flex-row md:items-center gap-5">
          {/* Avatar and Main Info Group */}
          <div className="flex items-center gap-4 shrink-0 pb-3 md:pb-0 md:border-r border-blue-500/5 md:pr-6">
            <div className="size-14 rounded-2xl bg-gradient-to-br from-blue-500 to-indigo-600 flex items-center justify-center text-white text-lg font-bold shadow-md shadow-blue-500/20 shrink-0 border border-white/10">
              {getInitials(employee.employeeName)}
            </div>
            <div className="min-w-0">
              <span className="text-[10px] font-bold text-blue-600 dark:text-blue-400 uppercase tracking-wider bg-blue-50 dark:bg-blue-950/40 px-2.5 py-1 rounded-full border border-blue-500/5">
                เจ้าของคำขอ (Requester)
              </span>
              <h4 className="text-lg font-extrabold text-foreground truncate mt-1.5 leading-snug">
                {employee.employeeName}
              </h4>
            </div>
          </div>
          
          {/* Badge Specs Grid (Details) */}
          <div className="flex-1 grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 text-xs font-semibold">
            {/* Department */}
            {employee.employeeDepartment && (
              <div className="flex items-start gap-2.5">
                <div className="size-8 rounded-lg bg-blue-50 dark:bg-blue-950/20 border border-blue-500/5 flex items-center justify-center text-blue-500 shrink-0">
                  <Building2 className="size-3.5" />
                </div>
                <div>
                  <p className="text-[10px] uppercase tracking-wider text-muted-foreground/80 font-bold">แผนก</p>
                  <p className="text-foreground text-sm font-bold mt-0.5">{employee.employeeDepartment}</p>
                </div>
              </div>
            )}
            
            {/* Role */}
            {employee.employeeRole && (
              <div className="flex items-start gap-2.5">
                <div className="size-8 rounded-lg bg-indigo-50 dark:bg-indigo-950/20 border border-indigo-500/5 flex items-center justify-center text-indigo-500 shrink-0">
                  <User className="size-3.5" />
                </div>
                <div>
                  <p className="text-[10px] uppercase tracking-wider text-muted-foreground/80 font-bold">ตำแหน่ง</p>
                  <p className="text-foreground text-sm font-bold mt-0.5">{employee.employeeRole}</p>
                </div>
              </div>
            )}
            
            {/* Email */}
            {employee.employeeEmail && (
              <div className="flex items-start gap-2.5 min-w-0">
                <div className="size-8 rounded-lg bg-teal-50 dark:bg-teal-950/20 border border-teal-500/5 flex items-center justify-center text-teal-500 shrink-0">
                  <Mail className="size-3.5" />
                </div>
                <div className="min-w-0 flex-1">
                  <p className="text-[10px] uppercase tracking-wider text-muted-foreground/80 font-bold">อีเมลพนักงาน</p>
                  <p className="text-foreground text-sm font-bold mt-0.5 truncate">{employee.employeeEmail}</p>
                </div>
              </div>
            )}
            
            {/* Manager */}
            {employee.employeeManagerName && (
              <div className="flex items-start gap-2.5">
                <div className="size-8 rounded-lg bg-amber-50 dark:bg-amber-950/20 border border-amber-500/5 flex items-center justify-center text-amber-500 shrink-0">
                  <UserCheck className="size-3.5" />
                </div>
                <div>
                  <p className="text-[10px] uppercase tracking-wider text-muted-foreground/80 font-bold">หัวหน้าผู้อนุมัติ</p>
                  <p className="text-foreground text-sm font-bold mt-0.5">{employee.employeeManagerName}</p>
                </div>
              </div>
            )}
          </div>
        </div>
      </CardContent>
    </Card>
  )
}
