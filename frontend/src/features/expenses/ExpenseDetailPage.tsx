import { useEffect, useState } from 'react'
import { Link, useParams, useSearchParams, useNavigate } from 'react-router-dom'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { RequestEmployeeInfo } from '@/features/shared/RequestEmployeeInfo'
import { WorkflowStatusBadge } from '@/features/shared/WorkflowStatusBadge'
import type { ExpenseClaim } from '@/types/api'
import { 
  ArrowLeft, 
  Trash2, 
  MessageSquare, 
  Info,
  Coins,
  BadgeAlert,
  FileSpreadsheet
} from 'lucide-react'

export function ExpenseDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const fromApprovals = searchParams.get('from') === 'approvals'
  const [item, setItem] = useState<ExpenseClaim | null>(null)
  const [loading, setLoading] = useState(true)

  const load = () => {
    if (!id) return
    setLoading(true)
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
      toast.success('ยกเลิกใบเบิกสำเร็จแล้ว')
      load()
    } catch (err) {
      toast.error(formatApiError(err, 'ยกเลิกไม่สำเร็จ'))
    }
  }

  if (loading) return (
    <div className="flex flex-col items-center justify-center min-h-[300px] space-y-3">
      <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
      <p className="text-sm text-muted-foreground font-semibold">กำลังโหลดข้อมูลใบเบิกค่าใช้จ่าย...</p>
    </div>
  )
  
  if (!item) return (
    <div className="flex flex-col items-center justify-center min-h-[300px] text-center space-y-4">
      <div className="p-3 bg-destructive/10 rounded-full text-destructive">
        <BadgeAlert className="size-8" />
      </div>
      <div>
        <h3 className="text-lg font-bold text-foreground">ไม่พบคำขอเบิกค่าใช้จ่าย</h3>
        <p className="text-sm text-muted-foreground mt-1">ลิ้งก์ที่คุณใช้อาจไม่ถูกต้อง หรือข้อมูลอาจถูกลบไปแล้ว</p>
      </div>
      <Button variant="outline" onClick={() => navigate('/expenses')} className="rounded-xl">
        กลับไปหน้าหลัก
      </Button>
    </div>
  )

  const canCancel = item.status === 'Draft' || item.status === 'Pending'

  return (
    <div className="max-w-4xl mx-auto space-y-6">
      {/* Back & Title Header */}
      <div className="flex flex-col gap-2">
        <div className="flex items-center gap-3">
          <Button variant="ghost" size="sm" asChild className="rounded-xl shrink-0 group border border-border/10">
            <Link to={fromApprovals ? '/approvals' : '/expenses'} className="flex items-center gap-1.5">
              <ArrowLeft className="size-4 group-hover:-translate-x-1 transition-transform" />
              <span>ย้อนกลับ</span>
            </Link>
          </Button>
          <span className="text-xs font-bold text-muted-foreground uppercase tracking-widest bg-muted/60 px-3 py-1 rounded-full border border-border/10">
            Expense Claim Details
          </span>
        </div>
        
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mt-2">
          <div>
            <h2 className="text-2xl font-black text-foreground tracking-tight flex items-center gap-2">
              คำขอ: <span className="text-primary">{item.title}</span>
            </h2>
            <p className="text-xs text-muted-foreground mt-1">เลขที่เอกสารเบิกจ่าย: #{item.id.slice(0, 8)}...</p>
          </div>
          <div className="flex items-center gap-3 shrink-0">
            <WorkflowStatusBadge status={item.status} />
          </div>
        </div>
      </div>

      {/* Corporate ID Badge Header */}
      <RequestEmployeeInfo employee={item} />

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 items-start">
        {/* Left / Center Main Content */}
        <div className="md:col-span-2 space-y-6">
          <Card className="border border-blue-500/10 shadow-lg shadow-blue-500/[0.02]">
            <CardHeader className="pb-3 border-b border-border/40">
              <CardTitle className="text-base font-extrabold uppercase tracking-wider text-primary flex items-center gap-2">
                <FileSpreadsheet className="size-4" />
                รายละเอียดรายการเบิก (Invoice Details)
              </CardTitle>
            </CardHeader>
            <CardContent className="pt-5 space-y-6">
              {/* Itemized Table */}
              <div className="border border-border/60 rounded-xl overflow-hidden shadow-sm bg-card">
                <Table>
                  <TableHeader className="bg-muted/50">
                    <TableRow>
                      <TableHead className="font-extrabold text-[10px] uppercase tracking-wider text-foreground pl-4">รายละเอียดสินค้า / บริการ</TableHead>
                      <TableHead className="text-right font-extrabold text-[10px] uppercase tracking-wider text-foreground pr-4">จำนวนเงินที่เบิก (บาท)</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {item.lineItems.map((l) => (
                      <TableRow key={l.id} className="hover:bg-muted/10 transition-colors">
                        <TableCell className="font-semibold text-xs py-3.5 pl-4 text-foreground">{l.description}</TableCell>
                        <TableCell className="text-right font-bold text-xs py-3.5 pr-4 text-foreground font-mono">
                          {l.amount.toLocaleString('th-TH', { minimumFractionDigits: 2 })}
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </div>

              {/* Manager Feedback Quote Bubble */}
              {item.managerComment && (
                <div className="p-4 rounded-2xl bg-amber-500/5 border border-amber-500/10 relative mt-4">
                  <div className="absolute -top-3 left-5 px-2.5 py-0.5 rounded-full bg-amber-500 text-white text-[9px] font-bold uppercase tracking-wider flex items-center gap-1 shadow-sm">
                    <MessageSquare className="size-3" /> ความเห็นผู้อนุมัติ
                  </div>
                  <p className="text-sm font-semibold text-foreground italic mt-1.5 pl-1 leading-relaxed">
                    "{item.managerComment}"
                  </p>
                </div>
              )}

              {/* Submit/Cancel Buttons bar */}
              {canCancel && (
                <div className="pt-2 border-t border-border/30 flex justify-end">
                  <Button 
                    variant="destructive" 
                    size="sm" 
                    onClick={cancel}
                    className="rounded-xl px-5 py-2 font-bold shadow-md shadow-destructive/10 hover:opacity-95 transition-all text-xs"
                  >
                    <Trash2 className="size-3.5 mr-1.5" />
                    ยกเลิกใบเบิกเงิน
                  </Button>
                </div>
              )}
            </CardContent>
          </Card>
        </div>

        {/* Right Column Content - Total Summary Box */}
        <div className="space-y-6">
          {/* Total amount summary card */}
          <Card className="bg-gradient-to-br from-blue-600 via-indigo-600 to-indigo-700 text-white shadow-xl shadow-blue-500/20 border-0 rounded-2xl relative overflow-hidden">
            {/* Ambient Background Glow Orbs */}
            <div className="absolute -right-10 -top-10 size-32 bg-white/10 rounded-full blur-xl" />
            
            <CardContent className="p-6 space-y-4 relative">
              <div className="flex items-center gap-2 text-white/80">
                <Coins className="size-4" />
                <span className="text-[10px] font-extrabold uppercase tracking-widest">สรุปยอดรวมใบเบิก</span>
              </div>
              
              <div>
                <span className="text-[10px] text-white/70 font-semibold block">ยอดเบิกทั้งสิ้น (Total Paid Out)</span>
                <h3 className="text-3xl font-black mt-2 font-mono flex items-baseline gap-1 tracking-tight select-all">
                  {item.totalAmount.toLocaleString('th-TH', { minimumFractionDigits: 2 })}
                  <span className="text-sm font-bold text-white/95 ml-1">บาท</span>
                </h3>
              </div>
            </CardContent>
          </Card>

          {/* Status info step guides */}
          <Card className="border border-blue-500/10 shadow-lg shadow-blue-500/[0.02]">
            <CardHeader className="pb-3 border-b border-border/40">
              <CardTitle className="text-sm font-extrabold uppercase tracking-wider text-primary flex items-center gap-2">
                <Info className="size-4" />
                สถานะใบสำคัญจ่าย
              </CardTitle>
            </CardHeader>
            <CardContent className="pt-4 text-xs font-semibold space-y-3.5 text-muted-foreground">
              <div className="p-3 bg-muted/20 border border-border/20 rounded-xl">
                <p className="text-foreground font-bold">ขั้นตอนถัดไป:</p>
                {item.status === 'Pending' ? (
                  <p className="mt-1 font-medium leading-relaxed">ใบเบิกจ่ายกำลังตรวจสอบยอดโดยฝ่ายการเงินและรอบัญชีตั้งเบิก โดยปกติระบบจะใช้เวลา 3-5 วันทำการในการโอนเงินเข้าบัญชีเงินเดือนที่ผูกไว้</p>
                ) : item.status === 'Approved' ? (
                  <p className="mt-1 font-medium leading-relaxed text-emerald-600 dark:text-emerald-400">ใบเบิกจ่ายได้รับการอนุมัติเรียบร้อยแล้ว ยอดเงินนี้จะโอนคืนพร้อมกับรอบการจ่ายเงินเดือนรอบถัดไป</p>
                ) : item.status === 'Rejected' ? (
                  <p className="mt-1 font-medium leading-relaxed text-destructive">คำขอถูกปฏิเสธเนื่องจากความเห็นของผู้อนุมัติหรือหลักฐานเอกสารไม่ถูกต้อง กรุณาติดต่อฝ่ายการเงินองค์กร</p>
                ) : (
                  <p className="mt-1 font-medium leading-relaxed">คำขอยังเป็นแบบร่าง กรุณาตรวจสอบความถูกต้องและกดส่งคำขออีกครั้ง</p>
                )}
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  )
}
