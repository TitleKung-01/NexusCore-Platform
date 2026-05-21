import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { WorkflowStatusBadge } from '@/features/shared/WorkflowStatusBadge'
import type { ExpenseClaim } from '@/types/api'

export function ExpenseDetailPage() {
  const { id } = useParams()
  const [item, setItem] = useState<ExpenseClaim | null>(null)
  const [loading, setLoading] = useState(true)

  const load = () => {
    if (!id) return
    api
      .get<ExpenseClaim>(`/api/expense-claims/${id}`)
      .then((r) => setItem(r.data))
      .catch(() => setItem(null))
      .finally(() => setLoading(false))
  }

  useEffect(load, [id])

  const cancel = async () => {
    try {
      await api.post(`/api/expense-claims/${id}/cancel`)
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
        <Link to="/expenses">← กลับ</Link>
      </Button>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>{item.title}</CardTitle>
          <WorkflowStatusBadge status={item.status} />
        </CardHeader>
        <CardContent className="space-y-4 text-sm">
          <p>
            <span className="text-muted-foreground">ยอดรวม:</span>{' '}
            {item.totalAmount.toLocaleString('th-TH')} บาท
          </p>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>รายละเอียด</TableHead>
                <TableHead className="text-right">จำนวน</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {item.lineItems.map((l) => (
                <TableRow key={l.id}>
                  <TableCell>{l.description}</TableCell>
                  <TableCell className="text-right">{l.amount.toLocaleString('th-TH')}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
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
