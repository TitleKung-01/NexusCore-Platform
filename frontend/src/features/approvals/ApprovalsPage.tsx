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
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Textarea } from '@/components/ui/textarea'
import { WorkflowStatusBadge } from '@/features/shared/WorkflowStatusBadge'
import type { ExpenseClaim, LeaveRequest, OvertimeRequest } from '@/types/api'

type ApprovalKind = 'leave' | 'overtime' | 'expense'

export function ApprovalsPage() {
  const [mainTab, setMainTab] = useState('pending')
  const [kindTab, setKindTab] = useState<ApprovalKind>('leave')
  const [leaveItems, setLeaveItems] = useState<LeaveRequest[]>([])
  const [leaveHistory, setLeaveHistory] = useState<LeaveRequest[]>([])
  const [overtimeItems, setOvertimeItems] = useState<OvertimeRequest[]>([])
  const [expenseItems, setExpenseItems] = useState<ExpenseClaim[]>([])
  const [comment, setComment] = useState('')
  const [dialogOpen, setDialogOpen] = useState(false)
  const [activeId, setActiveId] = useState<string | null>(null)
  const [activeKind, setActiveKind] = useState<ApprovalKind>('leave')
  const [action, setAction] = useState<'approve' | 'reject'>('approve')

  const loadPending = () => {
    api.get<LeaveRequest[]>('/api/leave-requests?scope=pending-approval').then((r) => setLeaveItems(r.data)).catch(() => setLeaveItems([]))
    api.get<OvertimeRequest[]>('/api/overtime-requests?scope=pending-approval').then((r) => setOvertimeItems(r.data)).catch(() => setOvertimeItems([]))
    api.get<ExpenseClaim[]>('/api/expense-claims?scope=pending-approval').then((r) => setExpenseItems(r.data)).catch(() => setExpenseItems([]))
  }

  const loadHistory = () => {
    api
      .get<LeaveRequest[]>('/api/leave-requests?scope=approval-history')
      .then((r) => setLeaveHistory(r.data))
      .catch(() => setLeaveHistory([]))
  }

  useEffect(() => {
    if (mainTab === 'pending') loadPending()
    else loadHistory()
  }, [mainTab])

  const openDialog = (id: string, kind: ApprovalKind, act: 'approve' | 'reject') => {
    setActiveId(id)
    setActiveKind(kind)
    setAction(act)
    setComment('')
    setDialogOpen(true)
  }

  const submitDecision = async () => {
    if (!activeId) return
    const base =
      activeKind === 'leave'
        ? 'leave-requests'
        : activeKind === 'overtime'
          ? 'overtime-requests'
          : 'expense-claims'
    try {
      const path = action === 'approve' ? 'approve' : 'reject'
      await api.post(`/api/${base}/${activeId}/${path}`, { comment: comment || null })
      toast.success(action === 'approve' ? 'อนุมัติแล้ว' : 'ปฏิเสธแล้ว')
      setDialogOpen(false)
      if (mainTab === 'pending') loadPending()
      else loadHistory()
    } catch (err) {
      toast.error(formatApiError(err, 'ดำเนินการไม่สำเร็จ'))
    }
  }

  const detailLink = (kind: ApprovalKind, id: string) => {
    if (kind === 'leave') return `/leave/${id}`
    if (kind === 'overtime') return `/overtime/${id}`
    return `/expenses/${id}`
  }

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-semibold">อนุมัติคำขอ</h2>
      <Tabs value={mainTab} onValueChange={setMainTab}>
        <TabsList>
          <TabsTrigger value="pending">รออนุมัติ</TabsTrigger>
          <TabsTrigger value="history">ประวัติ (ลา)</TabsTrigger>
        </TabsList>
        <TabsContent value="pending">
          <div className="flex flex-wrap gap-2 mt-2">
            <Button
              size="sm"
              variant={kindTab === 'leave' ? 'default' : 'outline'}
              onClick={() => setKindTab('leave')}
            >
              ลา ({leaveItems.length})
            </Button>
            <Button
              size="sm"
              variant={kindTab === 'overtime' ? 'default' : 'outline'}
              onClick={() => setKindTab('overtime')}
            >
              ล่วงเวลา ({overtimeItems.length})
            </Button>
            <Button
              size="sm"
              variant={kindTab === 'expense' ? 'default' : 'outline'}
              onClick={() => setKindTab('expense')}
            >
              เบิกค่าใช้จ่าย ({expenseItems.length})
            </Button>
          </div>
          {kindTab === 'leave' && (
            <ApprovalLeaveTable
              items={leaveItems}
              onApprove={(id) => openDialog(id, 'leave', 'approve')}
              onReject={(id) => openDialog(id, 'leave', 'reject')}
              detailLink={(id) => detailLink('leave', id)}
            />
          )}
          {kindTab === 'overtime' && (
            <ApprovalOvertimeTable
              items={overtimeItems}
              onApprove={(id) => openDialog(id, 'overtime', 'approve')}
              onReject={(id) => openDialog(id, 'overtime', 'reject')}
              detailLink={(id) => detailLink('overtime', id)}
            />
          )}
          {kindTab === 'expense' && (
            <ApprovalExpenseTable
              items={expenseItems}
              onApprove={(id) => openDialog(id, 'expense', 'approve')}
              onReject={(id) => openDialog(id, 'expense', 'reject')}
              detailLink={(id) => detailLink('expense', id)}
            />
          )}
        </TabsContent>
        <TabsContent value="history">
          <ApprovalLeaveTable
            items={leaveHistory}
            onApprove={(id) => openDialog(id, 'leave', 'approve')}
            onReject={(id) => openDialog(id, 'leave', 'reject')}
            detailLink={(id) => detailLink('leave', id)}
            readOnly
          />
        </TabsContent>
      </Tabs>

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

function ApprovalLeaveTable({
  items,
  onApprove,
  onReject,
  detailLink,
  readOnly,
}: {
  items: LeaveRequest[]
  onApprove: (id: string) => void
  onReject: (id: string) => void
  detailLink: (id: string) => string
  readOnly?: boolean
}) {
  return (
    <Card className="mt-4">
      <CardHeader>
        <CardTitle>คำขอลา ({items.length})</CardTitle>
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
                  <WorkflowStatusBadge status={item.status} />
                </TableCell>
                <TableCell className="space-x-2">
                  {!readOnly && item.status === 'Pending' && (
                    <>
                      <Button size="sm" onClick={() => onApprove(item.id)}>
                        อนุมัติ
                      </Button>
                      <Button size="sm" variant="destructive" onClick={() => onReject(item.id)}>
                        ปฏิเสธ
                      </Button>
                    </>
                  )}
                  <Button variant="link" size="sm" asChild>
                    <Link to={detailLink(item.id)}>ดู</Link>
                  </Button>
                </TableCell>
              </TableRow>
            ))}
            {items.length === 0 && (
              <TableRow>
                <TableCell colSpan={5} className="text-center text-muted-foreground">
                  ไม่มีรายการ
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </CardContent>
    </Card>
  )
}

function ApprovalOvertimeTable({
  items,
  onApprove,
  onReject,
  detailLink,
}: {
  items: OvertimeRequest[]
  onApprove: (id: string) => void
  onReject: (id: string) => void
  detailLink: (id: string) => string
}) {
  return (
    <Card className="mt-4">
      <CardHeader>
        <CardTitle>ล่วงเวลา ({items.length})</CardTitle>
      </CardHeader>
      <CardContent>
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>พนักงาน</TableHead>
              <TableHead>วันที่</TableHead>
              <TableHead>ชั่วโมง</TableHead>
              <TableHead>สถานะ</TableHead>
              <TableHead></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {items.map((item) => (
              <TableRow key={item.id}>
                <TableCell>{item.employeeName}</TableCell>
                <TableCell>{item.workDate}</TableCell>
                <TableCell>{item.hours}</TableCell>
                <TableCell>
                  <WorkflowStatusBadge status={item.status} />
                </TableCell>
                <TableCell className="space-x-2">
                  {item.status === 'Pending' && (
                    <>
                      <Button size="sm" onClick={() => onApprove(item.id)}>
                        อนุมัติ
                      </Button>
                      <Button size="sm" variant="destructive" onClick={() => onReject(item.id)}>
                        ปฏิเสธ
                      </Button>
                    </>
                  )}
                  <Button variant="link" size="sm" asChild>
                    <Link to={detailLink(item.id)}>ดู</Link>
                  </Button>
                </TableCell>
              </TableRow>
            ))}
            {items.length === 0 && (
              <TableRow>
                <TableCell colSpan={5} className="text-center text-muted-foreground">
                  ไม่มีรายการ
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </CardContent>
    </Card>
  )
}

function ApprovalExpenseTable({
  items,
  onApprove,
  onReject,
  detailLink,
}: {
  items: ExpenseClaim[]
  onApprove: (id: string) => void
  onReject: (id: string) => void
  detailLink: (id: string) => string
}) {
  return (
    <Card className="mt-4">
      <CardHeader>
        <CardTitle>เบิกค่าใช้จ่าย ({items.length})</CardTitle>
      </CardHeader>
      <CardContent>
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>พนักงาน</TableHead>
              <TableHead>หัวข้อ</TableHead>
              <TableHead>ยอด</TableHead>
              <TableHead>สถานะ</TableHead>
              <TableHead></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {items.map((item) => (
              <TableRow key={item.id}>
                <TableCell>{item.employeeName}</TableCell>
                <TableCell>{item.title}</TableCell>
                <TableCell>{item.totalAmount.toLocaleString('th-TH')}</TableCell>
                <TableCell>
                  <WorkflowStatusBadge status={item.status} />
                </TableCell>
                <TableCell className="space-x-2">
                  {item.status === 'Pending' && (
                    <>
                      <Button size="sm" onClick={() => onApprove(item.id)}>
                        อนุมัติ
                      </Button>
                      <Button size="sm" variant="destructive" onClick={() => onReject(item.id)}>
                        ปฏิเสธ
                      </Button>
                    </>
                  )}
                  <Button variant="link" size="sm" asChild>
                    <Link to={detailLink(item.id)}>ดู</Link>
                  </Button>
                </TableCell>
              </TableRow>
            ))}
            {items.length === 0 && (
              <TableRow>
                <TableCell colSpan={5} className="text-center text-muted-foreground">
                  ไม่มีรายการ
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </CardContent>
    </Card>
  )
}
