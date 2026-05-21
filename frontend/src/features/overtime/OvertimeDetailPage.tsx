import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { WorkflowStatusBadge } from '@/features/shared/WorkflowStatusBadge'
import type { OvertimeRequest } from '@/types/api'

export function OvertimeDetailPage() {
  const { id } = useParams()
  const [item, setItem] = useState<OvertimeRequest | null>(null)
  const [loading, setLoading] = useState(true)

  const load = () => {
    if (!id) return
    api
      .get<OvertimeRequest>(`/api/overtime-requests/${id}`)
      .then((r) => setItem(r.data))
      .catch(() => setItem(null))
      .finally(() => setLoading(false))
  }

  useEffect(load, [id])

  const cancel = async () => {
    try {
      await api.post(`/api/overtime-requests/${id}/cancel`)
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
        <Link to="/overtime">← กลับ</Link>
      </Button>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>ล่วงเวลา {item.workDate}</CardTitle>
          <WorkflowStatusBadge status={item.status} />
        </CardHeader>
        <CardContent className="space-y-2 text-sm">
          <p>
            <span className="text-muted-foreground">ชั่วโมง:</span> {item.hours}
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
