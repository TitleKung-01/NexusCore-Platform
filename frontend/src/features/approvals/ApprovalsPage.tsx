import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Textarea } from '@/components/ui/textarea'
import { LeaveStatusBadge } from '@/features/leave/statusBadge'
import type { LeaveRequest } from '@/types/api'

export function ApprovalsPage() {
  const [items, setItems] = useState<LeaveRequest[]>([])
  const [comment, setComment] = useState('')
  const [dialogOpen, setDialogOpen] = useState(false)
  const [activeId, setActiveId] = useState<string | null>(null)
  const [action, setAction] = useState<'approve' | 'reject'>('approve')

  const load = () => {
    api
      .get<LeaveRequest[]>('/api/leave-requests?scope=pending-approval')
      .then((r) => setItems(r.data))
      .catch(() => setItems([]))
  }

  useEffect(load, [])

  const openDialog = (id: string, act: 'approve' | 'reject') => {
    setActiveId(id)
    setAction(act)
    setComment('')
    setDialogOpen(true)
  }

  const submitDecision = async () => {
    if (!activeId) return
    try {
      const path = action === 'approve' ? 'approve' : 'reject'
      await api.post(`/api/leave-requests/${activeId}/${path}`, { comment: comment || null })
      toast.success(action === 'approve' ? 'อนุมัติแล้ว' : 'ปฏิเสธแล้ว')
      setDialogOpen(false)
      load()
    } catch (err) {
      toast.error(formatApiError(err, 'ดำเนินการไม่สำเร็จ'))
    }
  }

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-semibold">รออนุมัติ</h2>
      <Card>
        <CardHeader>
          <CardTitle>คำขอที่รอดำเนินการ ({items.length})</CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>พนักงาน</TableHead>
                <TableHead>ประเภท</TableHead>
                <TableHead>ช่วงวัน</TableHead>
                <TableHead>สถานะ</TableHead>
                <TableHead></TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {items.map((item) => (
                <TableRow key={item.id}>
                  <TableCell>{item.employeeName}</TableCell>
                  <TableCell>{item.leaveTypeName}</TableCell>
                  <TableCell>
                    {item.startDate} — {item.endDate}
                  </TableCell>
                  <TableCell>
                    <LeaveStatusBadge status={item.status} />
                  </TableCell>
                  <TableCell className="space-x-2">
                    <Button size="sm" onClick={() => openDialog(item.id, 'approve')}>
                      อนุมัติ
                    </Button>
                    <Button size="sm" variant="destructive" onClick={() => openDialog(item.id, 'reject')}>
                      ปฏิเสธ
                    </Button>
                    <Button variant="link" size="sm" asChild>
                      <Link to={`/leave/${item.id}`}>ดู</Link>
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
              {items.length === 0 && (
                <TableRow>
                  <TableCell colSpan={5} className="text-center text-muted-foreground">
                    ไม่มีคำขอรออนุมัติ
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </CardContent>
      </Card>

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{action === 'approve' ? 'อนุมัติคำขอ' : 'ปฏิเสธคำขอ'}</DialogTitle>
          </DialogHeader>
          <Textarea
            placeholder="ความเห็น (ไม่บังคับ)"
            value={comment}
            onChange={(e) => setComment(e.target.value)}
          />
          <DialogFooter>
            <Button variant="outline" onClick={() => setDialogOpen(false)}>
              ยกเลิก
            </Button>
            <Button onClick={submitDecision}>ยืนยัน</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}
