import { useEffect, useRef, useState } from 'react'
import { Link, useParams, useSearchParams, useNavigate } from 'react-router-dom'
import { toast } from 'sonner'
import { api, formatApiError, getStoredToken } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { LeaveStatusBadge } from '@/features/leave/statusBadge'
import { RequestEmployeeInfo } from '@/features/shared/RequestEmployeeInfo'
import type { LeaveAttachment, LeaveRequest } from '@/types/api'
import { 
  ArrowLeft, 
  Paperclip, 
  FileText, 
  Download, 
  Trash2, 
  Calendar, 
  MessageSquare, 
  Upload, 
  HelpCircle,
  FileText as FileIcon
} from 'lucide-react'

export function LeaveDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const fromApprovals = searchParams.get('from') === 'approvals'
  const fileRef = useRef<HTMLInputElement>(null)
  const [item, setItem] = useState<LeaveRequest | null>(null)
  const [attachments, setAttachments] = useState<LeaveAttachment[]>([])
  const [loading, setLoading] = useState(true)
  const [uploading, setUploading] = useState(false)

  const load = () => {
    if (!id) return
    setLoading(true)
    Promise.all([
      api.get<LeaveRequest>(`/api/leave-requests/${id}`),
      api.get<LeaveAttachment[]>(`/api/leave-requests/${id}/attachments`),
    ])
      .then(([req, att]) => {
        setItem(req.data)
        setAttachments(att.data)
      })
      .catch(() => setItem(null))
      .finally(() => setLoading(false))
  }

  useEffect(load, [id])

  const cancel = async () => {
    try {
      await api.post(`/api/leave-requests/${id}/cancel`)
      toast.success('ยกเลิกคำขอลาสำเร็จ')
      load()
    } catch (err) {
      toast.error(formatApiError(err, 'ยกเลิกไม่สำเร็จ'))
    }
  }

  const uploadFile = async (file: File) => {
    setUploading(true)
    try {
      const form = new FormData()
      form.append('file', file)
      await api.post(`/api/leave-requests/${id}/attachments`, form)
      toast.success('อัปโหลดไฟล์เรียบร้อยแล้ว')
      load()
    } catch (err) {
      toast.error(formatApiError(err, 'อัปโหลดไม่สำเร็จ'))
    } finally {
      setUploading(false)
    }
  }

  const downloadAttachment = async (attachmentId: string, fileName: string) => {
    try {
      const token = getStoredToken()
      const base = import.meta.env.VITE_API_URL ?? ''
      const res = await fetch(`${base}/api/leave-requests/attachments/${attachmentId}/download`, {
        headers: token ? { Authorization: `Bearer ${token}` } : {},
      })
      if (!res.ok) throw new Error('failed')
      const blob = await res.blob()
      const url = URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = fileName
      a.click()
      URL.revokeObjectURL(url)
    } catch {
      toast.error('ดาวน์โหลดไม่สำเร็จ')
    }
  }

  if (loading) return (
    <div className="flex flex-col items-center justify-center min-h-[300px] space-y-3">
      <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
      <p className="text-sm text-muted-foreground font-semibold">กำลังโหลดข้อมูลคำขอ...</p>
    </div>
  )
  
  if (!item) return (
    <div className="flex flex-col items-center justify-center min-h-[300px] text-center space-y-4">
      <div className="p-3 bg-destructive/10 rounded-full text-destructive">
        <HelpCircle className="size-8" />
      </div>
      <div>
        <h3 className="text-lg font-bold text-foreground">ไม่พบคำขอลาพักงาน</h3>
        <p className="text-sm text-muted-foreground mt-1">ลิ้งก์ที่คุณใช้อาจไม่ถูกต้อง หรือข้อมูลอาจถูกลบไปแล้ว</p>
      </div>
      <Button variant="outline" onClick={() => navigate('/leave')} className="rounded-xl">
        กลับไปหน้าหลัก
      </Button>
    </div>
  )

  const canCancel = item.status === 'Draft' || item.status === 'Pending'
  const canUpload = item.status === 'Draft' || item.status === 'Pending'

  return (
    <div className="max-w-4xl mx-auto space-y-6">
      {/* Back & Title Header */}
      <div className="flex flex-col gap-2">
        <div className="flex items-center gap-3">
          <Button variant="ghost" size="sm" asChild className="rounded-xl shrink-0 group border border-border/10">
            <Link to={fromApprovals ? '/approvals' : '/leave'} className="flex items-center gap-1.5">
              <ArrowLeft className="size-4 group-hover:-translate-x-1 transition-transform" />
              <span>ย้อนกลับ</span>
            </Link>
          </Button>
          <span className="text-xs font-bold text-muted-foreground uppercase tracking-widest bg-muted/60 px-3 py-1 rounded-full border border-border/10">
            Leave Request Details
          </span>
        </div>
        
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mt-2">
          <div>
            <h2 className="text-2xl font-black text-foreground tracking-tight flex items-center gap-2">
              คำขอ: <span className="text-primary">{item.leaveTypeName}</span>
            </h2>
            <p className="text-xs text-muted-foreground mt-1">เลขที่เอกสารอ้างอิง: #{item.id.slice(0, 8)}...</p>
          </div>
          <div className="flex items-center gap-3 shrink-0">
            <LeaveStatusBadge status={item.status} />
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
                รายละเอียดการลาพักงาน
              </CardTitle>
            </CardHeader>
            <CardContent className="pt-5 space-y-6">
              {/* Date Matrix display */}
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 p-4 rounded-2xl bg-muted/20 border border-border/30">
                <div className="flex items-center gap-3">
                  <div className="size-10 rounded-xl bg-blue-500/10 flex items-center justify-center text-primary shrink-0">
                    <Calendar className="size-5" />
                  </div>
                  <div>
                    <p className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground">วันเริ่มต้น</p>
                    <p className="text-sm font-extrabold text-foreground mt-0.5">{item.startDate}</p>
                  </div>
                </div>

                <div className="flex items-center gap-3">
                  <div className="size-10 rounded-xl bg-indigo-500/10 flex items-center justify-center text-indigo-500 shrink-0">
                    <Calendar className="size-5" />
                  </div>
                  <div>
                    <p className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground">วันสิ้นสุด</p>
                    <p className="text-sm font-extrabold text-foreground mt-0.5">{item.endDate}</p>
                  </div>
                </div>
              </div>

              {/* Reason description section */}
              <div className="space-y-2">
                <h4 className="text-xs font-bold uppercase tracking-wider text-muted-foreground">เหตุผลการขอลา</h4>
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
                    ยกเลิกคำขอลา
                  </Button>
                </div>
              )}
            </CardContent>
          </Card>
        </div>

        {/* Right Column Content - Attachments */}
        <div className="space-y-6">
          <Card className="border border-blue-500/10 shadow-lg shadow-blue-500/[0.02]">
            <CardHeader className="pb-3 border-b border-border/40">
              <CardTitle className="text-sm font-extrabold uppercase tracking-wider text-primary flex items-center gap-2">
                <Paperclip className="size-4" />
                เอกสาร / ไฟล์แนบ
              </CardTitle>
            </CardHeader>
            <CardContent className="pt-4 space-y-4">
              <ul className="space-y-2">
                {attachments.map((a) => (
                  <li 
                    key={a.id} 
                    className="flex justify-between items-center p-3 rounded-xl bg-muted/10 border border-border/30 hover:border-blue-500/15 transition-all gap-2"
                  >
                    <div className="flex items-center gap-2.5 min-w-0">
                      <div className="size-8 rounded-lg bg-blue-50 dark:bg-blue-950/20 border border-blue-500/5 flex items-center justify-center text-primary shrink-0">
                        <FileIcon className="size-4" />
                      </div>
                      <span className="text-xs font-bold text-foreground truncate min-w-0 pr-1">{a.fileName}</span>
                    </div>
                    <Button 
                      size="sm" 
                      variant="ghost" 
                      onClick={() => downloadAttachment(a.id, a.fileName)}
                      className="rounded-lg h-8 px-2 text-primary shrink-0 hover:bg-primary/10"
                    >
                      <Download className="size-3.5" />
                    </Button>
                  </li>
                ))}
                
                {attachments.length === 0 && (
                  <div className="flex flex-col items-center justify-center p-6 text-center bg-muted/10 rounded-2xl border border-dashed border-border/80">
                    <Paperclip className="size-6 text-muted-foreground/60" />
                    <p className="text-[11px] text-muted-foreground font-bold mt-2">ยังไม่มีการแนบเอกสารใดๆ</p>
                  </div>
                )}
              </ul>

              {/* Upload field zone */}
              {canUpload && (
                <div className="pt-2 border-t border-border/30">
                  <input
                    ref={fileRef}
                    type="file"
                    className="hidden"
                    onChange={(e) => {
                      const f = e.target.files?.[0]
                      if (f) void uploadFile(f)
                      e.target.value = ''
                    }}
                  />
                  <div 
                    onClick={() => !uploading && fileRef.current?.click()}
                    className="flex flex-col items-center justify-center p-4 rounded-xl border border-dashed border-blue-500/20 hover:border-primary/50 bg-blue-500/[0.01] hover:bg-blue-500/[0.03] transition-all text-center cursor-pointer group"
                  >
                    <Upload className="size-5 text-blue-500/70 group-hover:scale-105 transition-transform" />
                    <p className="text-[11px] text-primary font-bold mt-2">
                      {uploading ? 'กำลังอัปโหลดเอกสาร...' : 'คลิกที่นี่เพื่อแนบไฟล์เพิ่มเติม'}
                    </p>
                    <p className="text-[9px] text-muted-foreground mt-0.5 font-semibold">รองรับไฟล์ใบรับรองแพทย์ PDF, JPG, PNG</p>
                  </div>
                </div>
              )}
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  )
}
