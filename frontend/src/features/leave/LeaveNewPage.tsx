import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Textarea } from '@/components/ui/textarea'
import type { LeaveBalance, LeaveRequest, LeaveType } from '@/types/api'
import { Calendar, ClipboardList, Sparkles, BookOpen, Clock, HeartPulse, CheckCircle2 } from 'lucide-react'

export function LeaveNewPage() {
  const navigate = useNavigate()
  const [types, setTypes] = useState<LeaveType[]>([])
  const [balances, setBalances] = useState<LeaveBalance[]>([])
  const [leaveTypeId, setLeaveTypeId] = useState('')
  const [startDate, setStartDate] = useState('')
  const [endDate, setEndDate] = useState('')
  const [reason, setReason] = useState('')
  const [submitting, setSubmitting] = useState(false)
  const [loadingTypes, setLoadingTypes] = useState(true)

  useEffect(() => {
    setLoadingTypes(true)
    Promise.all([
      api.get<LeaveType[]>('/api/leave-types'),
      api.get<LeaveBalance[]>('/api/leave-balances'),
    ])
      .then(([typesRes, balancesRes]) => {
        setTypes(typesRes.data)
        setBalances(balancesRes.data)
        if (typesRes.data[0]) setLeaveTypeId(typesRes.data[0].id)
      })
      .catch((err) => toast.error(formatApiError(err, 'โหลดข้อมูลไม่สำเร็จ')))
      .finally(() => setLoadingTypes(false))
  }, [])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setSubmitting(true)
    try {
      const { data } = await api.post<LeaveRequest>('/api/leave-requests', {
        leaveTypeId,
        startDate,
        endDate,
        reason,
      })
      await api.post(`/api/leave-requests/${data.id}/submit`)
      toast.success('ส่งคำขอลาแล้ว')
      navigate(`/leave/${data.id}`)
    } catch (err) {
      toast.error(formatApiError(err, 'ยื่นไม่สำเร็จ'))
    } finally {
      setSubmitting(false)
    }
  }

  // Get corresponding icon for leave type
  const getLeaveIcon = (name: string) => {
    const lowercaseName = name.toLowerCase()
    if (lowercaseName.includes('ป่วย') || lowercaseName.includes('sick')) {
      return <HeartPulse className="size-4 text-emerald-500" />
    }
    if (lowercaseName.includes('พักร้อน') || lowercaseName.includes('vacation') || lowercaseName.includes('annual')) {
      return <Sparkles className="size-4 text-amber-500" />
    }
    return <Clock className="size-4 text-blue-500" />
  }

  // Get corresponding theme color classes for progress bar based on leave type
  const getLeaveProgressBarColor = (name: string) => {
    const lowercaseName = name.toLowerCase()
    if (lowercaseName.includes('ป่วย') || lowercaseName.includes('sick')) {
      return 'bg-gradient-to-r from-emerald-500 to-teal-400'
    }
    if (lowercaseName.includes('พักร้อน') || lowercaseName.includes('vacation') || lowercaseName.includes('annual')) {
      return 'bg-gradient-to-r from-amber-500 to-orange-400'
    }
    return 'bg-gradient-to-r from-blue-500 to-indigo-500'
  }

  return (
    <div className="max-w-6xl mx-auto space-y-6">
      {/* Premium Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 border-b border-border/50 pb-5">
        <div>
          <h2 className="text-3xl font-extrabold tracking-tight text-foreground flex items-center gap-3">
            <span className="p-2.5 rounded-xl bg-blue-500/10 text-primary shadow-inner">
              <Calendar className="size-7" />
            </span>
            ยื่นคำขอลาพักงาน
          </h2>
          <p className="text-sm text-muted-foreground mt-1.5 font-medium">
            กรอกฟอร์มยื่นล่วงหน้า และตรวจสอบโควตาวันลาของคุณตามนโยบายบริษัท
          </p>
        </div>
        <Button variant="outline" size="sm" onClick={() => navigate('/leave')} className="rounded-xl border-border/60 hover:bg-muted font-bold transition-all">
          ประวัติการลา
        </Button>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 items-start">
        {/* Left Column: Form */}
        <div className="lg:col-span-2 space-y-6">
          <Card className="border border-blue-500/10 shadow-lg shadow-blue-500/[0.02] bg-card overflow-hidden">
            <div className="h-1 bg-gradient-to-r from-blue-500 via-indigo-500 to-sky-400" />
            <CardHeader className="border-b border-border/40 pb-4">
              <CardTitle className="text-lg font-bold text-primary flex items-center gap-2">
                <ClipboardList className="size-5" />
                แบบฟอร์มข้อมูลการลา
              </CardTitle>
              <CardDescription className="font-semibold text-xs text-muted-foreground">กรุณากรอกข้อมูลที่ถูกต้องเพื่อประกอบการพิจารณาอนุมัติจากผู้บังคับบัญชา</CardDescription>
            </CardHeader>
            <CardContent className="pt-6">
              <form onSubmit={handleSubmit} className="space-y-6">
                <div className="space-y-2">
                  <Label className="text-xs font-bold uppercase tracking-wider text-muted-foreground">ประเภทการลา</Label>
                  <Select value={leaveTypeId} onValueChange={setLeaveTypeId} disabled={loadingTypes || types.length === 0}>
                    <SelectTrigger className="h-11 rounded-xl bg-muted/20 border-border/80 focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all">
                      <SelectValue placeholder={loadingTypes ? 'กำลังโหลด...' : types.length === 0 ? 'ไม่มีประเภทลา' : 'เลือกประเภทการลา'} />
                    </SelectTrigger>
                    <SelectContent className="rounded-xl">
                      {types.map((t) => (
                        <SelectItem key={t.id} value={t.id} className="rounded-lg font-semibold text-sm">
                          <span className="flex items-center gap-2">
                            {getLeaveIcon(t.name)}
                            {t.name}
                          </span>
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label className="text-xs font-bold uppercase tracking-wider text-muted-foreground">วันเริ่มต้น</Label>
                    <Input 
                      type="date" 
                      value={startDate} 
                      onChange={(e) => setStartDate(e.target.value)} 
                      required 
                      className="h-11 rounded-xl bg-muted/20 border-border/80 focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all"
                    />
                  </div>
                  <div className="space-y-2">
                    <Label className="text-xs font-bold uppercase tracking-wider text-muted-foreground">วันสิ้นสุด</Label>
                    <Input 
                      type="date" 
                      value={endDate} 
                      onChange={(e) => setEndDate(e.target.value)} 
                      required 
                      className="h-11 rounded-xl bg-muted/20 border-border/80 focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all"
                    />
                  </div>
                </div>

                <div className="space-y-2">
                  <Label className="text-xs font-bold uppercase tracking-wider text-muted-foreground">เหตุผลการขอลา</Label>
                  <Textarea 
                    value={reason} 
                    onChange={(e) => setReason(e.target.value)} 
                    required 
                    placeholder="ระบุเหตุผลการลาอย่างชัดเจน เช่น เพื่อเข้าพบแพทย์ตามนัดหมาย หรือ ติดธุระครอบครัว..."
                    className="min-h-[110px] rounded-xl bg-muted/20 border-border/80 focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all p-3 font-semibold text-sm"
                  />
                </div>

                <div className="pt-2">
                  <Button 
                    type="submit" 
                    disabled={submitting || loadingTypes || !leaveTypeId}
                    className="w-full md:w-auto px-8 h-11 text-sm font-bold rounded-xl bg-primary text-primary-foreground shadow-md shadow-primary/20 hover:opacity-95 transition-all btn-premium shrink-0"
                  >
                    {submitting ? 'กำลังส่งคำขอ...' : 'ส่งคำขออนุมัติ'}
                  </Button>
                </div>
              </form>
            </CardContent>
          </Card>
        </div>

        {/* Right Column: Sidebar Info */}
        <div className="space-y-6">
          {/* Days Remaining balances */}
          {balances.length > 0 && (
            <Card className="border border-blue-500/10 shadow-md shadow-blue-500/[0.02]">
              <CardHeader className="pb-3 border-b border-border/40 bg-muted/10">
                <CardTitle className="text-xs font-extrabold uppercase tracking-wider text-primary flex items-center gap-2">
                  <Clock className="size-4" />
                  สิทธิ์การลาคงเหลือของคุณ
                </CardTitle>
              </CardHeader>
              <CardContent className="pt-4 space-y-4 bg-card">
                {balances.map((b) => {
                  const percent = b.daysAllowed > 0 ? Math.min(100, Math.max(0, (b.daysRemaining / b.daysAllowed) * 100)) : 0
                  return (
                    <div key={b.leaveTypeId} className="p-3.5 rounded-xl bg-muted/10 border border-border/30 hover:border-blue-500/15 transition-all">
                      <div className="flex justify-between items-center text-xs font-semibold text-foreground">
                        <span className="flex items-center gap-2 font-bold text-sm">
                          {getLeaveIcon(b.leaveTypeName)}
                          {b.leaveTypeName}
                        </span>
                        <span className="font-extrabold text-primary text-sm">
                          {b.daysRemaining} / {b.daysAllowed} <span className="text-xs text-muted-foreground font-semibold">วัน</span>
                        </span>
                      </div>
                      
                      {/* Visual progress bar */}
                      <div className="w-full bg-muted/60 dark:bg-muted/30 rounded-full h-2 overflow-hidden mt-3.5 border border-border/10">
                        <div 
                          className={`${getLeaveProgressBarColor(b.leaveTypeName)} h-2 rounded-full transition-all duration-700 ease-out`} 
                          style={{ width: `${percent}%` }}
                        />
                      </div>
                    </div>
                  )
                })}
              </CardContent>
            </Card>
          )}

          {/* Guidelines policy box */}
          <Card className="border border-blue-500/10 shadow-md bg-muted/5 glass-panel">
            <CardHeader className="pb-3 border-b border-border/40 bg-muted/20">
              <CardTitle className="text-xs font-extrabold uppercase tracking-wider text-muted-foreground flex items-center gap-2">
                <BookOpen className="size-4 text-blue-500" />
                ระเบียบปฏิบัติและเงื่อนไข
              </CardTitle>
            </CardHeader>
            <CardContent className="pt-4 text-xs font-semibold space-y-3.5 text-muted-foreground">
              <div className="flex items-start gap-2.5">
                <CheckCircle2 className="size-4 text-emerald-500 shrink-0 mt-0.5" />
                <p>การลากิจควรส่งล่วงหน้าอย่างน้อย 3 วันทำการเพื่อให้ผู้บังคับบัญชาจัดสรรทีมงานทดแทน</p>
              </div>
              <div className="flex items-start gap-2.5">
                <CheckCircle2 className="size-4 text-emerald-500 shrink-0 mt-0.5" />
                <p>การลาป่วยตั้งแต่ 3 วันทำการขึ้นไป จำเป็นต้องแนบใบรับรองแพทย์ในหน้ารายละเอียดหลังการยื่นขอ</p>
              </div>
              <div className="flex items-start gap-2.5">
                <CheckCircle2 className="size-4 text-emerald-500 shrink-0 mt-0.5" />
                <p>การลากิจและลาพักผ่อนประจำปีจะได้รับการคำนวณวันลาคงเหลือโดยอัตโนมัติเมื่อหัวหน้างานอนุมัติ</p>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  )
}