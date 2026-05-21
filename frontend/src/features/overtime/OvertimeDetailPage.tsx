import { useEffect, useState } from 'react'
import { Link, useParams, useSearchParams, useNavigate } from 'react-router-dom'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { RequestEmployeeInfo } from '@/features/shared/RequestEmployeeInfo'
import { WorkflowStatusBadge } from '@/features/shared/WorkflowStatusBadge'
import type { OvertimeRequest } from '@/types/api'
import { 
  ArrowLeft, 
  Clock, 
  Calendar, 
  FileText, 
  Trash2, 
  MessageSquare, 
  Info,
  BadgeAlert
} from 'lucide-react'

export function OvertimeDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const fromApprovals = searchParams.get('from') === 'approvals'
  const [item, setItem] = useState<OvertimeRequest | null>(null)
  const [loading, setLoading] = useState(true)

  const load = () => {
    if (!id) return
    setLoading(true)
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
      toast.success('ยกเลิกคำขอ OT สำเร็จ')
      load()
    } catch (err) {
      toast.error(formatApiError(err, 'ยกเลิกไม่สำเร็จ'))
    }
  }

  if (loading) return (
    <div className="flex flex-col items-center justify-center min-h-[300px] space-y-3">
      <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
      <p className="text-sm text-muted-foreground font-semibold">กำลังโหลดข้อมูลคำขอ OT...</p>
    </div>
  )
  
  if (!item) return (
    <div className="flex flex-col items-center justify-center min-h-[300px] text-center space-y-4">
      <div className="p-3 bg-destructive/10 rounded-full text-destructive">
        <BadgeAlert className="size-8" />
      </div>
      <div>
        <h3 className="text-lg font-bold text-foreground">ไม่พบคำขอทำงานล่วงเวลา</h3>
        <p className="text-sm text-muted-foreground mt-1">ลิ้งก์ที่คุณใช้อาจไม่ถูกต้อง หรือข้อมูลอาจถูกลบไปแล้ว</p>
      </div>
      <Button variant="outline" onClick={() => navigate('/overtime')} className="rounded-xl">
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
            <Link to={fromApprovals ? '/approvals' : '/overtime'} className="flex items-center gap-1.5">
              <ArrowLeft className="size-4 group-hover:-translate-x-1 transition-transform" />
              <span>ย้อนกลับ</span>
            </Link>
          </Button>
          <span className="text-xs font-bold text-muted-foreground uppercase tracking-widest bg-muted/60 px-3 py-1 rounded-full border border-border/10">
            Overtime Claim Details
          </span>
        </div>
        
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mt-2">
          <div>
            <h2 className="text-2xl font-black text-foreground tracking-tight flex items-center gap-2">
              คำขอ: <span className="text-primary">ล่วงเวลา (OT) #{item.id.slice(0, 8)}...</span>
            </h2>
            <p className="text-xs text-muted-foreground mt-1">ยื่นปฏิบัติงานเมื่อ: {item.workDate}</p>
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
                <FileText className="size-4" />
                รายละเอียดคำขอล่วงเวลา
              </CardTitle>
            </CardHeader>
            <CardContent className="pt-5 space-y-6">
              {/* Date & Hours Grid */}
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 p-4 rounded-2xl bg-muted/20 border border-border/30">
                <div className="flex items-center gap-3">
                  <div className="size-10 rounded-xl bg-blue-500/10 flex items-center justify-center text-primary shrink-0">
                    <Calendar className="size-5" />
                  </div>
                  <div>
                    <p className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground">วันที่ปฏิบัติงานจริง</p>
                    <p className="text-sm font-extrabold text-foreground mt-0.5">{item.workDate}</p>
                  </div>
                </div>

                <div className="flex items-center gap-3">
                  <div className="size-10 rounded-xl bg-indigo-500/10 flex items-center justify-center text-indigo-500 shrink-0">
                    <Clock className="size-5" />
                  </div>
                  <div>
                    <p className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground">จำนวนชั่วโมงล่วงเวลา</p>
                    <p className="text-sm font-extrabold text-foreground mt-0.5">
                      {item.hours} <span className="text-xs text-muted-foreground font-normal">ชั่วโมง</span>
                    </p>
                  </div>
                </div>
              </div>

              {/* Reason description section */}
              <div className="space-y-2">
                <h4 className="text-xs font-bold uppercase tracking-wider text-muted-foreground">เหตุผลและความจำเป็น</h4>
                <div className="p-4 rounded-xl bg-card border border-border/50 text-sm font-medium leading-relaxed text-foreground shadow-sm">
                  {item.reason}
                </div>
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
                    ยกเลิกคำขอ OT
                  </Button>
                </div>
              )}
            </CardContent>
          </Card>
        </div>

        {/* Right Column Content - Additional Info / Next Steps */}
        <div className="space-y-6">
          <Card className="border border-blue-500/10 shadow-lg shadow-blue-500/[0.02]">
            <CardHeader className="pb-3 border-b border-border/40">
              <CardTitle className="text-sm font-extrabold uppercase tracking-wider text-primary flex items-center gap-2">
                <Info className="size-4" />
                สถานะการพิจารณา
              </CardTitle>
            </CardHeader>
            <CardContent className="pt-4 text-xs font-semibold space-y-3.5 text-muted-foreground">
              <div className="p-3 bg-muted/20 border border-border/20 rounded-xl">
                <p className="text-foreground font-bold">ขั้นตอนถัดไป:</p>
                {item.status === 'Pending' ? (
                  <p className="mt-1 font-medium leading-relaxed">คำขอของคุณอยู่ระหว่างการพิจารณาตรวจสอบชั่วโมงโดยหัวหน้าสายงานโดยตรง ระบบจะส่งอีเมลแจ้งทันทีที่มีผลลัพธ์</p>
                ) : item.status === 'Approved' ? (
                  <p className="mt-1 font-medium leading-relaxed text-emerald-600 dark:text-emerald-400">ได้รับการอนุมัติแล้ว ชั่วโมงทำงานล่วงเวลานี้จะถูกโอนไปประมวลผลจ่ายเบี้ยเลี้ยงรอบถัดไป</p>
                ) : item.status === 'Rejected' ? (
                  <p className="mt-1 font-medium leading-relaxed text-destructive">คำขอถูกปฏิเสธ หากคุณมีข้อโต้แย้งโปรดติดต่อแผนกบุคคล หรือหัวหน้าสายงานเพื่อชี้แจงเพิ่มเติม</p>
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
