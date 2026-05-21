import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { useAuth } from '@/features/auth/AuthContext'
import { LeaveStatusBadge } from '@/features/leave/statusBadge'
import type { LeaveRequest } from '@/types/api'

export function DashboardPage() {
  const { me, isApprover } = useAuth()
  const [mine, setMine] = useState<LeaveRequest[]>([])
  const [pendingCount, setPendingCount] = useState(0)

  useEffect(() => {
    api.get<LeaveRequest[]>('/api/leave-requests?scope=mine').then((r) => setMine(r.data.slice(0, 5)))
    if (isApprover) {
      api
        .get<LeaveRequest[]>('/api/leave-requests?scope=pending-approval')
        .then((r) => setPendingCount(r.data.length))
    }
  }, [isApprover])

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold">สวัสดี, {me?.fullName}</h2>
        <p className="text-muted-foreground">
          {me?.departmentName} · {me?.role}
        </p>
      </div>
      {isApprover && (
        <Card>
          <CardHeader>
            <CardTitle>รออนุมัติ</CardTitle>
          </CardHeader>
          <CardContent className="flex items-center justify-between">
            <p className="text-3xl font-bold">{pendingCount}</p>
            <Button asChild>
              <Link to="/approvals">ไปหน้าอนุมัติ</Link>
            </Button>
          </CardContent>
        </Card>
      )}
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>คำขอลาล่าสุด</CardTitle>
          <Button variant="outline" size="sm" asChild>
            <Link to="/leave">ดูทั้งหมด</Link>
          </Button>
        </CardHeader>
        <CardContent className="space-y-3">
          {mine.map((item) => (
            <div key={item.id} className="flex items-center justify-between border-b pb-2 last:border-0">
              <div>
                <p className="font-medium">{item.leaveTypeName}</p>
                <p className="text-sm text-muted-foreground">
                  {item.startDate} — {item.endDate}
                </p>
              </div>
              <LeaveStatusBadge status={item.status} />
            </div>
          ))}
          {mine.length === 0 && (
            <p className="text-muted-foreground text-sm">ยังไม่มีคำขอลา — <Link to="/leave/new" className="underline">ยื่นคำขอ</Link></p>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
