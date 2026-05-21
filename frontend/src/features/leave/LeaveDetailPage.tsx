import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { LeaveStatusBadge } from '@/features/leave/statusBadge'
import type { LeaveRequest } from '@/types/api'

export function LeaveDetailPage() {
  const { id } = useParams()
  const [item, setItem] = useState<LeaveRequest | null>(null)
  const [loading, setLoading] = useState(true)

  const load = () => {
    if (!id) return
    api
      .get<LeaveRequest>(`/api/leave-requests/${id}`)
      .then((r) => setItem(r.data))
      .catch(() => setItem(null))
      .finally(() => setLoading(false))
  }

  useEffect(load, [id])

  const cancel = async () => {
    try {
      await api.post(`/api/leave-requests/${id}/cancel`)
      toast.success('ยกเลิกแล้ว')
      load()
    } catch (err) {
      toast.error(formatApiError(err, 'ยกเลิกไม่สำเร็จ'))
    }
  }

  if (loading) return <p className="text-muted-foreground">กำลังโหลด...</p>
  if (!item) return <p className="text-destructive">ไม่พบคำขอ</p>

  const canCancel = item.status === 'Draft' || item.status === 'Pending'

  return (
    <div className="max-w-lg space-y-4">
      <Button variant="ghost" size="sm" asChild>
        <Link to="/leave">← กลับ</Link>
      </Button>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>{item.leaveTypeName}</CardTitle>
          <LeaveStatusBadge status={item.status} />
        </CardHeader>
        <CardContent className="space-y-2 text-sm">
          <p>
            <span className="text-muted-foreground">ช่วงวัน:</span> {item.startDate} — {item.endDate}
          </p>
          <p>
            <span className="text-muted-foreground">เหตุผล:</span> {item.reason}
          </p>
          {item.managerComment && (
            <p>
              <span className="text-muted-foreground">ความเห็นผู้อนุมัติ:</span> {item.managerComment}
            </p>
          )}
          {canCancel && (
            <Button variant="destructive" size="sm" onClick={cancel}>
              ยกเลิกคำขอ
            </Button>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
