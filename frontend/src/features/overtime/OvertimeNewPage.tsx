import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import type { OvertimeRequest } from '@/types/api'
import { Clock, ClipboardList, BookOpen, CheckCircle2, BadgePercent } from 'lucide-react'

export function OvertimeNewPage() {
  const navigate = useNavigate()
  const [workDate, setWorkDate] = useState('')
  const [hours, setHours] = useState('1')
  const [reason, setReason] = useState('')
  const [submitting, setSubmitting] = useState(false)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setSubmitting(true)
    try {
      const { data } = await api.post<OvertimeRequest>('/api/overtime-requests', {
        workDate,
        hours: Number(hours),
        reason,
      })
      await api.post(`/api/overtime-requests/${data.id}/submit`)
      toast.success('ส่งคำขอล่วงเวลาสำเร็จแล้ว')
      navigate(`/overtime/${data.id}`)
    } catch (err) {
      toast.error(formatApiError(err, 'ยื่นไม่สำเร็จ'))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="max-w-6xl mx-auto space-y-6">
      {/* Premium Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 border-b border-border/50 pb-5">
        <div>
          <h2 className="text-3xl font-extrabold tracking-tight text-foreground flex items-center gap-3">
            <span className="p-2.5 rounded-xl bg-blue-500/10 text-primary shadow-inner">
              <Clock className="size-7" />
            </span>
            ยื่นคำขอทำงานล่วงเวลา (OT)
          </h2>
          <p className="text-sm text-muted-foreground mt-1.5 font-medium">
            กรอกวันที่ทำงานและระยะเวลาตามจริง เพื่อส่งพิจารณาและคำนวณเบี้ยเลี้ยงล่วงเวลา
          </p>
        </div>
        <Button variant="outline" size="sm" onClick={() => navigate('/overtime')} className="rounded-xl border-border/60 hover:bg-muted font-bold transition-all">
          ประวัติคำขอ OT
        </Button>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 items-start">
        {/* Left Column: Form Card */}
        <div className="lg:col-span-2 space-y-6">
          <Card className="border border-blue-500/10 shadow-lg shadow-blue-500/[0.02] bg-card overflow-hidden">
            <div className="h-1 bg-gradient-to-r from-blue-500 via-indigo-500 to-sky-400" />
            <CardHeader className="border-b border-border/40 pb-4">
              <CardTitle className="text-lg font-bold text-primary flex items-center gap-2">
                <ClipboardList className="size-5" />
                รายละเอียดการทำงานล่วงเวลา
              </CardTitle>
              <CardDescription className="font-semibold text-xs text-muted-foreground">กรอกข้อมูลวันที่ปฏิบัติงานและจำนวนชั่วโมงล่วงเวลาให้ถูกต้อง</CardDescription>
            </CardHeader>
            <CardContent className="pt-6">
              <form onSubmit={handleSubmit} className="space-y-6">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label className="text-xs font-bold uppercase tracking-wider text-muted-foreground">วันที่ทำงาน</Label>
                    <Input 
                      type="date" 
                      value={workDate} 
                      onChange={(e) => setWorkDate(e.target.value)} 
                      required 
                      className="h-11 rounded-xl bg-muted/20 border-border/80 focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all pl-3"
                    />
                  </div>
                  
                  <div className="space-y-2">
                    <Label className="text-xs font-bold uppercase tracking-wider text-muted-foreground">จำนวนชั่วโมงปฏิบัติงาน</Label>
                    <Input
                      type="number"
                      min={0.5}
                      step={0.5}
                      value={hours}
                      onChange={(e) => setHours(e.target.value)}
                      required
                      className="h-11 rounded-xl bg-muted/20 border-border/80 focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all"
                    />
                  </div>
                </div>

                <div className="space-y-2">
                  <Label className="text-xs font-bold uppercase tracking-wider text-muted-foreground">เหตุผลและความจำเป็น</Label>
                  <Textarea 
                    value={reason} 
                    onChange={(e) => setReason(e.target.value)} 
                    required 
                    placeholder="ระบุรายละเอียดงานที่ปฏิบัติล่วงเวลาอย่างชัดเจน เช่น แก้ไขระบบคลาวด์ล่มภายนอกเวลาทำการ หรือ เตรียมข้อมูลนำเสนอการประชุมไตรมาส..."
                    className="min-h-[110px] rounded-xl bg-muted/20 border-border/80 focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all p-3 font-semibold text-sm"
                  />
                </div>

                <div className="pt-2">
                  <Button 
                    type="submit" 
                    disabled={submitting}
                    className="w-full md:w-auto px-8 h-11 text-sm font-bold rounded-xl bg-primary text-primary-foreground shadow-md shadow-primary/20 hover:opacity-95 transition-all btn-premium shrink-0"
                  >
                    {submitting ? 'กำลังส่งคำขอ...' : 'ส่งคำขออนุมัติ OT'}
                  </Button>
                </div>
              </form>
            </CardContent>
          </Card>
        </div>

        {/* Right Column: Policies & Multipliers Info */}
        <div className="space-y-6">
          {/* Compensation Rates Info */}
          <Card className="border border-blue-500/10 shadow-md">
            <CardHeader className="pb-3 border-b border-border/40 bg-muted/10">
              <CardTitle className="text-xs font-extrabold uppercase tracking-wider text-primary flex items-center gap-2">
                <BadgePercent className="size-4" />
                อัตราการจ่ายค่าล่วงเวลา (OT Rates)
              </CardTitle>
            </CardHeader>
            <CardContent className="pt-4 space-y-3 bg-card">
              <div className="flex justify-between items-center p-3 rounded-xl bg-muted/10 border border-border/30 hover:border-blue-500/15 transition-all">
                <span className="text-sm font-bold text-foreground">วันทำงานปกติ (Weekday OT)</span>
                <span className="text-sm font-extrabold text-blue-600 dark:text-blue-400">1.5 เท่า</span>
              </div>
              <div className="flex justify-between items-center p-3 rounded-xl bg-muted/10 border border-border/30 hover:border-blue-500/15 transition-all">
                <span className="text-sm font-bold text-foreground">วันหยุดราชการ/เสาร์-อาทิตย์</span>
                <span className="text-sm font-extrabold text-indigo-600 dark:text-indigo-400">3.0 เท่า</span>
              </div>
              <div className="p-3.5 rounded-xl bg-amber-500/5 border border-amber-500/10 text-amber-600 dark:text-amber-400 text-xs font-semibold leading-relaxed">
                *อัตราการจ่ายจะเป็นไปตามกฎกระทรวงแรงงานและโครงสร้างของระดับพนักงานที่บริษัทกำหนด
              </div>
            </CardContent>
          </Card>

          {/* Guidelines info */}
          <Card className="border border-blue-500/10 shadow-md bg-muted/5 glass-panel">
            <CardHeader className="pb-3 border-b border-border/40 bg-muted/20">
              <CardTitle className="text-xs font-extrabold uppercase tracking-wider text-muted-foreground flex items-center gap-2">
                <BookOpen className="size-4 text-blue-500" />
                นโยบายและระเบียบปฏิบัติ
              </CardTitle>
            </CardHeader>
            <CardContent className="pt-4 text-xs font-semibold space-y-3.5 text-muted-foreground">
              <div className="flex items-start gap-2.5">
                <CheckCircle2 className="size-4 text-emerald-500 shrink-0 mt-0.5" />
                <p>พนักงานต้องปฏิบัติงานล่วงเวลาไม่ต่ำกว่า 1 ชั่วโมงจึงจะนับเป็นจำนวนเต็มในการคำนวณเบี้ยเลี้ยง</p>
              </div>
              <div className="flex items-start gap-2.5">
                <CheckCircle2 className="size-4 text-emerald-500 shrink-0 mt-0.5" />
                <p>การขออนุมัติย้อนหลังควรดำเนินการส่งฟอร์มภายใน 3 วันทำการหลังจากการปฏิบัติงานจริง</p>
              </div>
              <div className="flex items-start gap-2.5">
                <CheckCircle2 className="size-4 text-emerald-500 shrink-0 mt-0.5" />
                <p>หากมีชั่วโมงทำงานเกิน 4 ชั่วโมงในวันหยุด บริษัทจะมีเบี้ยเลี้ยงอาหารกลางวันเพิ่มเติมให้</p>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  )
}
