import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { useAuth } from '@/features/auth/AuthContext'
import { LeaveStatusBadge } from '@/features/leave/statusBadge'
import type { Announcement, ExpenseClaim, LeaveBalance, LeaveRequest, OvertimeRequest } from '@/types/api'

export function DashboardPage() {
  const { me, isApprover } = useAuth()
  const [mine, setMine] = useState<LeaveRequest[]>([])
  const [balances, setBalances] = useState<LeaveBalance[]>([])
  const [announcements, setAnnouncements] = useState<Announcement[]>([])
  const [pendingCount, setPendingCount] = useState(0)

  useEffect(() => {
    api.get<LeaveRequest[]>('/api/leave-requests?scope=mine').then((r) => setMine(r.data.slice(0, 5)))
    api.get<LeaveBalance[]>('/api/leave-balances').then((r) => setBalances(r.data)).catch(() => setBalances([]))
    api
      .get<Announcement[]>('/api/announcements')
      .then((r) => setAnnouncements(r.data.filter((a) => a.isActive).slice(0, 3)))
      .catch(() => setAnnouncements([]))
    if (isApprover) {
      Promise.all([
        api.get<LeaveRequest[]>('/api/leave-requests?scope=pending-approval'),
        api.get<OvertimeRequest[]>('/api/overtime-requests?scope=pending-approval'),
        api.get<ExpenseClaim[]>('/api/expense-claims?scope=pending-approval'),
      ]).then(([leave, ot, exp]) => {
        setPendingCount(leave.data.length + ot.data.length + exp.data.length)
      }).catch(() => setPendingCount(0))
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
      {announcements.length > 0 && (
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle>ประกาศล่าสุด</CardTitle>
            <Button variant="outline" size="sm" asChild>
              <Link to="/announcements">ดูทั้งหมด</Link>
            </Button>
          </CardHeader>
          <CardContent className="space-y-3">
            {announcements.map((a) => (
              <div key={a.id} className="border-b pb-2 last:border-0">
                <p className="font-medium">{a.title}</p>
                <p className="text-sm text-muted-foreground line-clamp-2">{a.body}</p>
              </div>
            ))}
          </CardContent>
        </Card>
      )}
      {balances.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle>วันลาคงเหลือ</CardTitle>
          </CardHeader>
          <CardContent className="grid gap-2 sm:grid-cols-2">
            {balances.map((b) => (
              <div key={b.leaveTypeId} className="flex justify-between text-sm rounded border p-3">
                <span>{b.leaveTypeName}</span>
                <span className="font-semibold">
                  {b.daysRemaining} วัน
                </span>
              </div>
            ))}
          </CardContent>
        </Card>
      )}
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
            <p className="text-muted-foreground text-sm">
              ยังไม่มีคำขอลา —{' '}
              <Link to="/leave/new" className="underline">
                ยื่นคำขอ
              </Link>
            </p>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
