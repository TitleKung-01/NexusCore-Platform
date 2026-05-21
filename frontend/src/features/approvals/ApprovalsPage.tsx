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
import { EmployeeTableCell } from '@/features/shared/EmployeeTableCell'
import { RequestEmployeeInfo, type RequestEmployeeInfoData } from '@/features/shared/RequestEmployeeInfo'
import { WorkflowStatusBadge } from '@/features/shared/WorkflowStatusBadge'
import type { ExpenseClaim, LeaveRequest, OvertimeRequest } from '@/types/api'
import { 
  CheckSquare, 
  Check, 
  X, 
  Eye, 
  Calendar, 
  Clock, 
  Receipt,
  AlertCircle
} from 'lucide-react'

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
  const [activeEmployee, setActiveEmployee] = useState<RequestEmployeeInfoData | null>(null)
  const [activeSummary, setActiveSummary] = useState('')
  const [action, setAction] = useState<'approve' | 'reject'>('approve')
  const [loading, setLoading] = useState(false)

  const loadPending = () => {
    setLoading(true)
    Promise.all([
      api.get<LeaveRequest[]>('/api/leave-requests?scope=pending-approval').catch(() => ({ data: [] })),
      api.get<OvertimeRequest[]>('/api/overtime-requests?scope=pending-approval').catch(() => ({ data: [] })),
      api.get<ExpenseClaim[]>('/api/expense-claims?scope=pending-approval').catch(() => ({ data: [] }))
    ])
      .then(([leaveRes, otRes, expRes]) => {
        setLeaveItems(leaveRes.data)
        setOvertimeItems(otRes.data)
        setExpenseItems(expRes.data)
      })
      .finally(() => setLoading(false))
  }

  const loadHistory = () => {
    setLoading(true)
    api
      .get<LeaveRequest[]>('/api/leave-requests?scope=approval-history')
      .then((r) => setLeaveHistory(r.data))
      .catch(() => setLeaveHistory([]))
      .finally(() => setLoading(false))
  }

  useEffect(() => {
    if (mainTab === 'pending') loadPending()
    else loadHistory()
  }, [mainTab])

  const openDialog = (
    employee: RequestEmployeeInfoData,
    id: string,
    kind: ApprovalKind,
    act: 'approve' | 'reject',
    summary: string
  ) => {
    setActiveEmployee(employee)
    setActiveSummary(summary)
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
      toast.success(action === 'approve' ? 'อนุมัติคำขอสำเร็จแล้ว' : 'ปฏิเสธคำขอสำเร็จแล้ว')
      setDialogOpen(false)
      if (mainTab === 'pending') loadPending()
      else loadHistory()
    } catch (err) {
      toast.error(formatApiError(err, 'ดำเนินการไม่สำเร็จ'))
    }
  }

  const detailLink = (kind: ApprovalKind, id: string) => {
    const q = '?from=approvals'
    if (kind === 'leave') return `/leave/${id}${q}`
    if (kind === 'overtime') return `/overtime/${id}${q}`
    return `/expenses/${id}${q}`
  }

  return (
    <div className="max-w-6xl mx-auto space-y-6">
      {/* Premium Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 border-b border-border/50 pb-5">
        <div>
          <h2 className="text-3xl font-extrabold tracking-tight text-foreground flex items-center gap-3">
            <span className="p-2.5 rounded-xl bg-blue-500/10 text-primary shadow-inner">
              <CheckSquare className="size-7" />
            </span>
            ศูนย์อนุมัติคำขอ (Approvals Center)
          </h2>
          <p className="text-sm text-muted-foreground mt-1.5 font-medium">
            พิจารณาตรวจสอบการยื่นลา ทำงานล่วงเวลา และเบิกค่าใช้จ่ายของพนักงานในทีม
          </p>
        </div>
      </div>

      <Tabs value={mainTab} onValueChange={setMainTab} className="w-full">
        <TabsList className="bg-muted/60 p-1 rounded-xl h-11 border border-border/40">
          <TabsTrigger value="pending" className="rounded-lg font-bold text-xs px-5">
            รอพิจารณาอนุมัติ
          </TabsTrigger>
          <TabsTrigger value="history" className="rounded-lg font-bold text-xs px-5">
            ประวัติการอนุมัติ (เฉพาะลา)
          </TabsTrigger>
        </TabsList>

        <TabsContent value="pending" className="mt-4 space-y-4 outline-none">
          {/* Sub-tabs pills */}
          <div className="flex flex-wrap gap-2 p-1.5 rounded-xl bg-muted/20 border border-border/30 w-fit">
            <Button
              size="sm"
              variant={kindTab === 'leave' ? 'default' : 'ghost'}
              onClick={() => setKindTab('leave')}
              className={`rounded-lg font-bold text-xs transition-all ${
                kindTab === 'leave' 
                  ? 'bg-primary text-primary-foreground shadow-xs' 
                  : 'text-muted-foreground hover:bg-muted/50 hover:text-foreground'
              }`}
            >
              <Calendar className="size-3.5 mr-1.5" />
              คำขอลา ({leaveItems.length})
            </Button>
            <Button
              size="sm"
              variant={kindTab === 'overtime' ? 'default' : 'ghost'}
              onClick={() => setKindTab('overtime')}
              className={`rounded-lg font-bold text-xs transition-all ${
                kindTab === 'overtime' 
                  ? 'bg-primary text-primary-foreground shadow-xs' 
                  : 'text-muted-foreground hover:bg-muted/50 hover:text-foreground'
              }`}
            >
              <Clock className="size-3.5 mr-1.5" />
              ล่วงเวลา ({overtimeItems.length})
            </Button>
            <Button
              size="sm"
              variant={kindTab === 'expense' ? 'default' : 'ghost'}
              onClick={() => setKindTab('expense')}
              className={`rounded-lg font-bold text-xs transition-all ${
                kindTab === 'expense' 
                  ? 'bg-primary text-primary-foreground shadow-xs' 
                  : 'text-muted-foreground hover:bg-muted/50 hover:text-foreground'
              }`}
            >
              <Receipt className="size-3.5 mr-1.5" />
              เบิกค่าใช้จ่าย ({expenseItems.length})
            </Button>
          </div>

          {kindTab === 'leave' && (
            <ApprovalLeaveTable
              items={leaveItems}
              loading={loading}
              onApprove={(item) =>
                openDialog(item, item.id, 'leave', 'approve', `${item.leaveTypeName}: ${item.startDate} ถึง ${item.endDate}`)
              }
              onReject={(item) =>
                openDialog(item, item.id, 'leave', 'reject', `${item.leaveTypeName}: ${item.startDate} ถึง ${item.endDate}`)
              }
              detailLink={(id) => detailLink('leave', id)}
            />
          )}
          {kindTab === 'overtime' && (
            <ApprovalOvertimeTable
              items={overtimeItems}
              loading={loading}
              onApprove={(item) =>
                openDialog(item, item.id, 'overtime', 'approve', `ล่วงเวลาวันที่ ${item.workDate} · จำนวน ${item.hours} ชั่วโมง`)
              }
              onReject={(item) =>
                openDialog(item, item.id, 'overtime', 'reject', `ล่วงเวลาวันที่ ${item.workDate} · จำนวน ${item.hours} ชั่วโมง`)
              }
              detailLink={(id) => detailLink('overtime', id)}
            />
          )}
          {kindTab === 'expense' && (
            <ApprovalExpenseTable
              items={expenseItems}
              loading={loading}
              onApprove={(item) =>
                openDialog(
                  item,
                  item.id,
                  'expense',
                  'approve',
                  `${item.title} · ยอดเบิก ${item.totalAmount.toLocaleString('th-TH')} บาท`
                )
              }
              onReject={(item) =>
                openDialog(
                  item,
                  item.id,
                  'expense',
                  'reject',
                  `${item.title} · ยอดเบิก ${item.totalAmount.toLocaleString('th-TH')} บาท`
                )
              }
              detailLink={(id) => detailLink('expense', id)}
            />
          )}
        </TabsContent>

        <TabsContent value="history" className="mt-4 outline-none">
          <ApprovalLeaveTable
            items={leaveHistory}
            loading={loading}
            onApprove={(item) =>
              openDialog(item, item.id, 'leave', 'approve', `${item.leaveTypeName}: ${item.startDate} ถึง ${item.endDate}`)
            }
            onReject={(item) =>
              openDialog(item, item.id, 'leave', 'reject', `${item.leaveTypeName}: ${item.startDate} ถึง ${item.endDate}`)
            }
            detailLink={(id) => detailLink('leave', id)}
            readOnly
          />
        </TabsContent>
      </Tabs>

      {/* Decision Dialog */}
      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent className="max-w-lg rounded-2xl border border-blue-500/10 shadow-2xl">
          <DialogHeader className="pb-3 border-b border-border/40">
            <DialogTitle className="text-lg font-extrabold text-foreground flex items-center gap-2">
              {action === 'approve' ? (
                <>
                  <span className="p-1 rounded bg-emerald-500/10 text-emerald-500 shrink-0">
                    <Check className="size-4.5" />
                  </span>
                  <span>อนุมัติคำขอของพนักงาน</span>
                </>
              ) : (
                <>
                  <span className="p-1 rounded bg-destructive/10 text-destructive shrink-0">
                    <X className="size-4.5" />
                  </span>
                  <span>ปฏิเสธคำขอของพนักงาน</span>
                </>
              )}
            </DialogTitle>
          </DialogHeader>

          <div className="space-y-4 pt-3">
            {activeEmployee && (
              <div className="border border-border/30 rounded-xl overflow-hidden bg-muted/10 p-1">
                <RequestEmployeeInfo employee={activeEmployee} />
              </div>
            )}
            
            {activeSummary && (
              <div className="text-xs font-bold text-foreground rounded-xl border border-blue-500/10 bg-blue-500/[0.02] px-3.5 py-2.5 flex items-start gap-2">
                <AlertCircle className="size-4 text-primary shrink-0 mt-0.5" />
                <div>
                  <p className="text-[10px] text-muted-foreground uppercase tracking-wider font-semibold">สรุปรายละเอียดคำขอ</p>
                  <p className="mt-0.5">{activeSummary}</p>
                </div>
              </div>
            )}
            
            <div className="space-y-2">
              <label className="text-[10px] font-extrabold uppercase tracking-widest text-muted-foreground">ความคิดเห็นพิจารณา (ความคิดเห็นเพิ่มเติม)</label>
              <Textarea
                placeholder="ระบุข้อความชี้แจงความเห็นเพิ่มเติมเพื่อให้พนักงานรับทราบ..."
                value={comment}
                onChange={(e) => setComment(e.target.value)}
                className="min-h-[90px] rounded-xl bg-muted/20 border-border/80 focus:ring-2 focus:ring-primary/20 focus:border-primary text-sm p-3 font-medium"
              />
            </div>
          </div>

          <DialogFooter className="pt-3 border-t border-border/40 gap-2 sm:gap-0 mt-2">
            <Button variant="outline" onClick={() => setDialogOpen(false)} className="rounded-xl font-bold text-xs">
              ยกเลิก
            </Button>
            <Button 
              onClick={submitDecision}
              className={`rounded-xl font-bold text-xs ${
                action === 'approve' 
                  ? 'bg-emerald-600 hover:bg-emerald-500 text-white shadow-md shadow-emerald-500/10' 
                  : 'bg-destructive hover:bg-destructive/90 text-destructive-foreground shadow-md shadow-destructive/10'
              }`}
            >
              {action === 'approve' ? 'อนุมัติคำขอ' : 'ปฏิเสธคำขอ'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}

function ApprovalLeaveTable({
  items,
  loading,
  onApprove,
  onReject,
  detailLink,
  readOnly,
}: {
  items: LeaveRequest[]
  loading: boolean
  onApprove: (item: LeaveRequest) => void
  onReject: (item: LeaveRequest) => void
  detailLink: (id: string) => string
  readOnly?: boolean
}) {
  return (
    <Card className="border border-blue-500/10 shadow-lg shadow-blue-500/[0.02] bg-card overflow-hidden">
      <CardHeader className="border-b border-border/40 pb-4">
        <CardTitle className="text-base font-extrabold text-primary flex items-center gap-2">
          <Calendar className="size-5" />
          รายการพิจารณาอนุมัติคำขอลาพักงาน ({items.length})
        </CardTitle>
      </CardHeader>
      <CardContent className="p-0">
        <div className="overflow-x-auto">
          <Table>
            <TableHeader className="bg-muted/40">
              <TableRow>
                <TableHead className="font-extrabold text-[10px] uppercase tracking-wider text-foreground pl-6 py-3">พนักงานผู้ยื่นขอ</TableHead>
                <TableHead className="font-extrabold text-[10px] uppercase tracking-wider text-foreground py-3">ประเภทการลา</TableHead>
                <TableHead className="font-extrabold text-[10px] uppercase tracking-wider text-foreground py-3">ช่วงวันที่ขอลา</TableHead>
                <TableHead className="font-extrabold text-[10px] uppercase tracking-wider text-foreground py-3">สถานะ</TableHead>
                <TableHead className="py-3 text-right pr-6"></TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {items.map((item) => (
                <TableRow key={item.id} className="hover:bg-muted/10 transition-colors">
                  <TableCell className="py-3.5 pl-6">
                    <EmployeeTableCell employee={item} />
                  </TableCell>
                  <TableCell className="font-bold text-xs py-3.5 text-foreground">{item.leaveTypeName}</TableCell>
                  <TableCell className="font-semibold text-xs py-3.5 text-foreground">
                    <div className="flex items-center gap-1.5 text-muted-foreground">
                      <span className="text-foreground font-bold">{item.startDate}</span>
                      <span>ถึง</span>
                      <span className="text-foreground font-bold">{item.endDate}</span>
                    </div>
                  </TableCell>
                  <TableCell className="py-3.5">
                    <WorkflowStatusBadge status={item.status} />
                  </TableCell>
                  <TableCell className="py-3.5 text-right pr-6 space-x-1.5 shrink-0 whitespace-nowrap">
                    {!readOnly && item.status === 'Pending' && (
                      <>
                        <Button 
                          size="sm" 
                          onClick={() => onApprove(item)} 
                          className="rounded-lg h-8 text-[11px] font-bold bg-emerald-600 hover:bg-emerald-500 text-white shadow-xs"
                        >
                          <Check className="size-3.5 mr-1" /> อนุมัติ
                        </Button>
                        <Button 
                          size="sm" 
                          variant="destructive" 
                          onClick={() => onReject(item)} 
                          className="rounded-lg h-8 text-[11px] font-bold shadow-xs"
                        >
                          <X className="size-3.5 mr-1" /> ปฏิเสธ
                        </Button>
                      </>
                    )}
                    <Button variant="ghost" size="sm" asChild className="rounded-lg h-8 text-primary hover:bg-primary/10 hover:text-primary font-bold">
                      <Link to={detailLink(item.id)}>
                        <Eye className="size-3.5 mr-1" />
                        <span>เปิดดู</span>
                      </Link>
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
              
              {items.length === 0 && !loading && (
                <TableRow>
                  <TableCell colSpan={5} className="py-10 text-center text-xs text-muted-foreground font-semibold">
                    ไม่มีรายการคำขอลาพักงานค้างพิจารณา
                  </TableCell>
                </TableRow>
              )}

              {loading && (
                <TableRow>
                  <TableCell colSpan={5} className="py-10 text-center">
                    <div className="flex items-center justify-center gap-2">
                      <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-primary"></div>
                      <span className="text-xs text-muted-foreground font-semibold">กำลังโหลดข้อมูล...</span>
                    </div>
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </div>
      </CardContent>
    </Card>
  )
}

function ApprovalOvertimeTable({
  items,
  loading,
  onApprove,
  onReject,
  detailLink,
}: {
  items: OvertimeRequest[]
  loading: boolean
  onApprove: (item: OvertimeRequest) => void
  onReject: (item: OvertimeRequest) => void
  detailLink: (id: string) => string
}) {
  return (
    <Card className="border border-blue-500/10 shadow-lg shadow-blue-500/[0.02] bg-card overflow-hidden">
      <CardHeader className="border-b border-border/40 pb-4">
        <CardTitle className="text-base font-extrabold text-primary flex items-center gap-2">
          <Clock className="size-5" />
          รายการพิจารณาคำขอล่วงเวลา (OT) ({items.length})
        </CardTitle>
      </CardHeader>
      <CardContent className="p-0">
        <div className="overflow-x-auto">
          <Table>
            <TableHeader className="bg-muted/40">
              <TableRow>
                <TableHead className="font-extrabold text-[10px] uppercase tracking-wider text-foreground pl-6 py-3">พนักงานผู้ยื่นขอ</TableHead>
                <TableHead className="font-extrabold text-[10px] uppercase tracking-wider text-foreground py-3">วันที่ทำงาน</TableHead>
                <TableHead className="font-extrabold text-[10px] uppercase tracking-wider text-foreground py-3">จำนวนชั่วโมง</TableHead>
                <TableHead className="font-extrabold text-[10px] uppercase tracking-wider text-foreground py-3">สถานะ</TableHead>
                <TableHead className="py-3 text-right pr-6"></TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {items.map((item) => (
                <TableRow key={item.id} className="hover:bg-muted/10 transition-colors">
                  <TableCell className="py-3.5 pl-6">
                    <EmployeeTableCell employee={item} />
                  </TableCell>
                  <TableCell className="font-bold text-xs py-3.5 text-foreground">{item.workDate}</TableCell>
                  <TableCell className="font-bold text-xs py-3.5 text-foreground">
                    <span className="font-black text-indigo-600 dark:text-indigo-400">{item.hours}</span> ชั่วโมง
                  </TableCell>
                  <TableCell className="py-3.5">
                    <WorkflowStatusBadge status={item.status} />
                  </TableCell>
                  <TableCell className="py-3.5 text-right pr-6 space-x-1.5 shrink-0 whitespace-nowrap">
                    {item.status === 'Pending' && (
                      <>
                        <Button 
                          size="sm" 
                          onClick={() => onApprove(item)} 
                          className="rounded-lg h-8 text-[11px] font-bold bg-emerald-600 hover:bg-emerald-500 text-white shadow-xs"
                        >
                          <Check className="size-3.5 mr-1" /> อนุมัติ
                        </Button>
                        <Button 
                          size="sm" 
                          variant="destructive" 
                          onClick={() => onReject(item)} 
                          className="rounded-lg h-8 text-[11px] font-bold shadow-xs"
                        >
                          <X className="size-3.5 mr-1" /> ปฏิเสธ
                        </Button>
                      </>
                    )}
                    <Button variant="ghost" size="sm" asChild className="rounded-lg h-8 text-primary hover:bg-primary/10 hover:text-primary font-bold">
                      <Link to={detailLink(item.id)}>
                        <Eye className="size-3.5 mr-1" />
                        <span>เปิดดู</span>
                      </Link>
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
              
              {items.length === 0 && !loading && (
                <TableRow>
                  <TableCell colSpan={5} className="py-10 text-center text-xs text-muted-foreground font-semibold">
                    ไม่มีรายการคำขอ OT ค้างพิจารณา
                  </TableCell>
                </TableRow>
              )}

              {loading && (
                <TableRow>
                  <TableCell colSpan={5} className="py-10 text-center">
                    <div className="flex items-center justify-center gap-2">
                      <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-primary"></div>
                      <span className="text-xs text-muted-foreground font-semibold">กำลังโหลดข้อมูล...</span>
                    </div>
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </div>
      </CardContent>
    </Card>
  )
}

function ApprovalExpenseTable({
  items,
  loading,
  onApprove,
  onReject,
  detailLink,
}: {
  items: ExpenseClaim[]
  loading: boolean
  onApprove: (item: ExpenseClaim) => void
  onReject: (item: ExpenseClaim) => void
  detailLink: (id: string) => string
}) {
  return (
    <Card className="border border-blue-500/10 shadow-lg shadow-blue-500/[0.02] bg-card overflow-hidden">
      <CardHeader className="border-b border-border/40 pb-4">
        <CardTitle className="text-base font-extrabold text-primary flex items-center gap-2">
          <Receipt className="size-5" />
          รายการพิจารณาใบเบิกค่าใช้จ่าย ({items.length})
        </CardTitle>
      </CardHeader>
      <CardContent className="p-0">
        <div className="overflow-x-auto">
          <Table>
            <TableHeader className="bg-muted/40">
              <TableRow>
                <TableHead className="font-extrabold text-[10px] uppercase tracking-wider text-foreground pl-6 py-3">พนักงานผู้ยื่นขอ</TableHead>
                <TableHead className="font-extrabold text-[10px] uppercase tracking-wider text-foreground py-3">หัวเรื่องการเบิกเงิน</TableHead>
                <TableHead className="font-extrabold text-[10px] uppercase tracking-wider text-foreground py-3">จำนวนเงินเบิก</TableHead>
                <TableHead className="font-extrabold text-[10px] uppercase tracking-wider text-foreground py-3">สถานะ</TableHead>
                <TableHead className="py-3 text-right pr-6"></TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {items.map((item) => (
                <TableRow key={item.id} className="hover:bg-muted/10 transition-colors">
                  <TableCell className="py-3.5 pl-6">
                    <EmployeeTableCell employee={item} />
                  </TableCell>
                  <TableCell className="font-bold text-xs py-3.5 text-foreground">{item.title}</TableCell>
                  <TableCell className="font-bold text-xs py-3.5 text-foreground font-mono">
                    <span className="font-black text-emerald-600 dark:text-emerald-400">{item.totalAmount.toLocaleString('th-TH')}</span> บาท
                  </TableCell>
                  <TableCell className="py-3.5">
                    <WorkflowStatusBadge status={item.status} />
                  </TableCell>
                  <TableCell className="py-3.5 text-right pr-6 space-x-1.5 shrink-0 whitespace-nowrap">
                    {item.status === 'Pending' && (
                      <>
                        <Button 
                          size="sm" 
                          onClick={() => onApprove(item)} 
                          className="rounded-lg h-8 text-[11px] font-bold bg-emerald-600 hover:bg-emerald-500 text-white shadow-xs"
                        >
                          <Check className="size-3.5 mr-1" /> อนุมัติ
                        </Button>
                        <Button 
                          size="sm" 
                          variant="destructive" 
                          onClick={() => onReject(item)} 
                          className="rounded-lg h-8 text-[11px] font-bold shadow-xs"
                        >
                          <X className="size-3.5 mr-1" /> ปฏิเสธ
                        </Button>
                      </>
                    )}
                    <Button variant="ghost" size="sm" asChild className="rounded-lg h-8 text-primary hover:bg-primary/10 hover:text-primary font-bold">
                      <Link to={detailLink(item.id)}>
                        <Eye className="size-3.5 mr-1" />
                        <span>เปิดดู</span>
                      </Link>
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
              
              {items.length === 0 && !loading && (
                <TableRow>
                  <TableCell colSpan={5} className="py-10 text-center text-xs text-muted-foreground font-semibold">
                    ไม่มีรายการคำขอเบิกเงินค้างพิจารณา
                  </TableCell>
                </TableRow>
              )}

              {loading && (
                <TableRow>
                  <TableCell colSpan={5} className="py-10 text-center">
                    <div className="flex items-center justify-center gap-2">
                      <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-primary"></div>
                      <span className="text-xs text-muted-foreground font-semibold">กำลังโหลดข้อมูล...</span>
                    </div>
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </div>
      </CardContent>
    </Card>
  )
}

